﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Text;
using Mosa.Runtime.Loader;
using Mosa.Runtime.Vm;
using System.IO;
using System.Diagnostics;
using Mosa.Runtime.Metadata.Signatures;

namespace Mosa.Runtime.Metadata.Blobs
{
	/// <summary>
	/// Parses and instantiates custom attributes in assembly metadata blobs.
	/// </summary>
	public static class CustomAttributeParser
	{
		#region Constants

		/// <summary>
		/// The prologue id for a custom attribute blob.
		/// </summary>
		private const ushort ATTRIBUTE_BLOB_PROLOGUE = 0x0001;

		/// <summary>
		/// Length of a null string in a custom attribute blob.
		/// </summary>
		private const byte ATTRIBUTE_NULL_STRING_LEN = 0xFF;

		/// <summary>
		/// Length of an empty string in a custom attribute blob.
		/// </summary>
		private const byte ATTRIBUTE_EMPTY_STRING_LEN = 0x00;

		#endregion // Constants

		#region Methods

		/// <summary>
		/// Parses the specified attribute blob and instantiates the attribute.
		/// </summary>
		/// <param name="module">The metadata module, which contains the attribute blob.</param>
		/// <param name="attributeBlob">The attribute blob token.</param>
		/// <param name="attributeCtor">The constructor of the attribute.</param>
		/// <returns>The fully instantiated and initialized attribute.</returns>
		/// <exception cref="System.ArgumentException"><paramref name="attributeBlob"/> is invalid.</exception>
		/// <exception cref="System.ArgumentNullException"><paramref name="module"/> is null or <paramref name="attributeCtor"/> is null.</exception>
		public static object Parse(IMetadataModule module, TokenTypes attributeBlob, RuntimeMethod attributeCtor)
		{
			// Return value
			object result;
			// The blob data
			byte[] blob = null;

			// Try to load the blob from the module
			module.Metadata.Read(attributeBlob, out blob);
			if (null != blob) {
				if (0 != blob.Length) {
					// Create a binary reader for the blob
					using (BinaryReader reader = new BinaryReader(new MemoryStream(blob), Encoding.UTF8)) {
						ushort prologue = reader.ReadUInt16();
						Debug.Assert(ATTRIBUTE_BLOB_PROLOGUE == prologue, @"Attribute prologue doesn't match.");
						if (prologue != ATTRIBUTE_BLOB_PROLOGUE)
							throw new ArgumentException(@"Invalid custom attribute blob.", "attributeBlob");

						// Fixed argument list of the ctor
						SigType[] paramSig = attributeCtor.Signature.Parameters;
						IList<RuntimeParameter> parameters = attributeCtor.Parameters;
						object[] args = new object[parameters.Count];
						for (int idx = 0; idx < parameters.Count; idx++)
							args[idx] = ParseFixedArg(module, reader, paramSig[idx]);

						// Create the attribute instance
						result = CreateAttribute(attributeCtor, args);

						// Are there any named args?
						ushort numNamed = reader.ReadUInt16();
						for (ushort idx = 0; idx < numNamed; idx++) {
							// FIXME: Process the named arguments
							Trace.WriteLine(@"Skipping named argument of an attribute.");
						}
					}
				}
				else {
					result = CreateAttribute(attributeCtor, null);
				}
			}
			else {
				throw new ArgumentException(@"Invalid attribute blob token.", @"attributeBlob");
			}

			return result;
		}

		#endregion // Methods

		#region Internals

		/// <summary>
		/// Creates the attribute.
		/// </summary>
		/// <param name="attributeCtor">The attribute constructor to invoke.</param>
		/// <param name="args">The arguments to pass to the constructor.</param>
		/// <returns>The newly constructed attribute.</returns>
		private static object CreateAttribute(RuntimeMethod attributeCtor, object[] args)
		{
			RuntimeType rt = attributeCtor.DeclaringType;
			Type attributeType = Type.GetType(String.Format("{0}.{1}", rt.Namespace, rt.Name));
			return Activator.CreateInstance(attributeType, args, null);
		}

		/// <summary>
		/// Parses a fixed argument in an attribute blob definition.
		/// </summary>
		/// <param name="module">The metadata module, which contains the attribute blob.</param>
		/// <param name="reader">The binary reader to read it from.</param>
		/// <param name="sigType">The signature type of the value to read.</param>
		/// <returns></returns>
		private static object ParseFixedArg(IMetadataModule module, BinaryReader reader, SigType sigType)
		{
			// Return value
			object result = null;

			// A vector?
			SZArraySigType arraySigType = sigType as SZArraySigType;
			if (arraySigType != null)
				result = ParseSZArrayArg(module, reader, arraySigType);
			else
				result = ParseElem(module, reader, sigType);

			return result;
		}

		/// <summary>
		/// Parses an SZArray attribute value.
		/// </summary>
		/// <param name="module">The metadata module, which contains the attribute blob.</param>
		/// <param name="reader">The binary reader used to read from the attribute blob.</param>
		/// <param name="sigType">Type of the SZArray.</param>
		/// <returns>An Array, which represents the SZArray definition.</returns>
		private static object ParseSZArrayArg(IMetadataModule module, BinaryReader reader, SZArraySigType sigType)
		{
			// Return value
			Array result;

			// Determine the number of elements
			int numElements = reader.ReadInt32();
			if (-1 == numElements)
				return null;

			// Retrieve the array element type
			Type elementType = GetTypeFromSigType(sigType);
			Debug.Assert(null != elementType, @"Failed to get System.Type for SigType.");
			result = Array.CreateInstance(elementType, numElements);
			for (int idx = 0; idx < numElements; idx++) {
				object item = ParseElem(module, reader, sigType.ElementType);
				result.SetValue(item, idx);
			}

			return result;
		}

		/// <summary>
		/// Parses an elementary field, parameter or property definition.
		/// </summary>
		/// <param name="module">The metadata module, which contains the attribute blob.</param>
		/// <param name="reader">The binary reader to read data from.</param>
		/// <param name="sigType">The signature type of the field, parameter or property to read.</param>
		/// <returns>An object, which represents the value read from the attribute blob.</returns>
		/// <exception cref="System.NotSupportedException"><paramref name="sigType"/> is not yet supported.</exception>
		private static object ParseElem(IMetadataModule module, BinaryReader reader, SigType sigType)
		{
			object result;

			switch (sigType.Type) {
				case CilElementType.Boolean:
					result = (1 == reader.ReadByte());
					break;

				case CilElementType.Char:
					result = (char)reader.ReadUInt16();
					break;

				case CilElementType.I1:
					result = reader.ReadSByte();
					break;

				case CilElementType.I2:
					result = reader.ReadInt16();
					break;

				case CilElementType.I4:
					result = reader.ReadInt32();
					break;

				case CilElementType.I8:
					result = reader.ReadInt64();
					break;

				case CilElementType.U1:
					result = reader.ReadByte();
					break;

				case CilElementType.U2:
					result = reader.ReadUInt16();
					break;

				case CilElementType.U4:
					result = reader.ReadUInt32();
					break;

				case CilElementType.U8:
					result = reader.ReadUInt64();
					break;

				case CilElementType.R4:
					result = reader.ReadSingle();
					break;

				case CilElementType.R8:
					result = reader.ReadDouble();
					break;

				case CilElementType.String:
					result = ParseSerString(reader);
					break;

				case CilElementType.Type: {
						string typeName = ParseSerString(reader);
						result = Type.GetType(typeName);
					}
					break;

				case CilElementType.Class: {
						string typeName = ParseSerString(reader);
						result = Type.GetType(typeName + ", " + module.Name);
						//result = Type.GetType(typeName);
					}
					break;

				case CilElementType.ValueType: {
						ValueTypeSigType vtSigType = sigType as ValueTypeSigType;
						ITypeSystem ts = RuntimeBase.Instance.TypeLoader;
						RuntimeType type = ts.GetType(module, vtSigType.Token);
						RuntimeType baseType = type.BaseType;
						if (@"System" == baseType.Namespace && "Enum" == baseType.Name) {
							// Retrieve the value__ field to get the enums integer type
							Debug.Assert(type.Fields.Count == 1, @"More than one field in the enum.");
							RuntimeField value = type.Fields[0];
							Debug.Assert(value.Name == @"value__", @"First field of enum not named value__");
							result = ParseElem(module, reader, value.Type);
							Type enumType = Type.GetType(type.Namespace + "." + type.Name);
							result = Enum.ToObject(enumType, result);
						}
						else {
							throw new NotSupportedException();
						}
					}
					break;

				case CilElementType.Object:
					throw new NotSupportedException();

				default:
					throw new NotSupportedException();
			}

			return result;
		}

		/// <summary>
		/// Parses a string definition for an attribute field, parameter or property definition.
		/// </summary>
		/// <param name="reader">The binary reader to read from.</param>
		/// <returns>A string, which represents the value read.</returns>
		private static string ParseSerString(BinaryReader reader)
		{
			// Read the length
			byte packedLen = reader.ReadByte();
			if (ATTRIBUTE_NULL_STRING_LEN == packedLen)
				return null;
			if (ATTRIBUTE_EMPTY_STRING_LEN == packedLen)
				return String.Empty;

			// Read the string
			char[] buffer = reader.ReadChars(packedLen);
			return new String(buffer);
		}

		/// <summary>
		/// Gets the type from the signature type.
		/// </summary>
		/// <param name="sigType">The signature type.</param>
		/// <returns>The System.Type represented by the signature type.</returns>
		private static Type GetTypeFromSigType(SigType sigType)
		{
			Type result = null;

			switch (sigType.Type) {
				case CilElementType.I: result = typeof(IntPtr); break;
				case CilElementType.I1: result = typeof(SByte); break;
				case CilElementType.I2: result = typeof(Int16); break;
				case CilElementType.I4: result = typeof(Int32); break;
				case CilElementType.I8: result = typeof(Int64); break;
				case CilElementType.U: result = typeof(UIntPtr); break;
				case CilElementType.U1: result = typeof(Byte); break;
				case CilElementType.U2: result = typeof(UInt16); break;
				case CilElementType.U4: result = typeof(UInt32); break;
				case CilElementType.U8: result = typeof(UInt64); break;
				case CilElementType.R4: result = typeof(Single); break;
				case CilElementType.R8: result = typeof(Double); break;
				case CilElementType.String: result = typeof(String); break;
				case CilElementType.Object: result = typeof(System.Object); break;
				default:
					throw new NotSupportedException();
			}

			return result;
		}

		#endregion // Internals
	}
}
