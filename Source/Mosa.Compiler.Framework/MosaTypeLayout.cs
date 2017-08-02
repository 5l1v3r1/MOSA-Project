// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.MosaTypeSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mosa.Compiler.Framework
{
	/// <summary>
	/// Performs memory layout of a type for compilation.
	/// </summary>
	public class MosaTypeLayout
	{
		#region Data members

		/// <summary>
		/// Holds a set of types
		/// </summary>
		private readonly HashSet<MosaType> typeSet = new HashSet<MosaType>();

		/// <summary>
		/// Holds a list of interfaces
		/// </summary>
		private readonly List<MosaType> interfaces = new List<MosaType>();

		/// <summary>
		/// Holds the method table offsets value for each method
		/// </summary>
		private readonly Dictionary<MosaMethod, int> methodTableOffsets = new Dictionary<MosaMethod, int>(new MosaMethodFullNameComparer());

		/// <summary>
		/// Holds the slot value for each interface
		/// </summary>
		private readonly Dictionary<MosaType, int> interfaceSlots = new Dictionary<MosaType, int>();

		/// <summary>
		/// Holds the size of each type
		/// </summary>
		private readonly Dictionary<MosaType, int> typeSizes = new Dictionary<MosaType, int>();

		/// <summary>
		/// Holds the size of each field
		/// </summary>
		private readonly Dictionary<MosaField, int> fieldSizes = new Dictionary<MosaField, int>();

		/// <summary>
		/// Holds the offset for each field
		/// </summary>
		public readonly Dictionary<MosaField, int> fieldOffsets = new Dictionary<MosaField, int>();

		/// <summary>
		/// Holds a list of methods for each type
		/// </summary>
		private readonly Dictionary<MosaType, List<MosaMethod>> typeMethodTables = new Dictionary<MosaType, List<MosaMethod>>();

		/// <summary>
		/// The method stack sizes
		/// </summary>
		private readonly Dictionary<MosaMethod, int> methodStackSizes = new Dictionary<MosaMethod, int>(new MosaMethodFullNameComparer());

		/// <summary>
		/// The parameter offsets
		/// </summary>
		public Dictionary<MosaMethod, List<int>> parameterOffsets = new Dictionary<MosaMethod, List<int>>(); // fixme: change to private

		/// <summary>
		/// The parameter stack size
		/// </summary>
		public Dictionary<MosaMethod, int> parameterStackSize = new Dictionary<MosaMethod, int>(); // fixme: change to private

		/// <summary>
		/// The parameter stack size
		/// </summary>
		public Dictionary<MosaMethod, int> methodReturnSize = new Dictionary<MosaMethod, int>(); // fixme: change to private

		#endregion Data members

		#region Properties

		/// <summary>
		/// Gets the type system associated with this instance.
		/// </summary>
		/// <value>The type system.</value>
		public TypeSystem TypeSystem { get; }

		/// <summary>
		/// Gets the size of the native pointer.
		/// </summary>
		/// <value>The size of the native pointer.</value>
		public int NativePointerSize { get; }

		/// <summary>
		/// Gets the native pointer alignment.
		/// </summary>
		/// <value>The native pointer alignment.</value>
		public int NativePointerAlignment { get; }

		/// <summary>
		/// Get a list of interfaces
		/// </summary>
		public IList<MosaType> Interfaces { get { return interfaces; } }

		#endregion Properties

		/// <summary>
		/// Initializes a new instance of the <see cref="MosaTypeLayout" /> class.
		/// </summary>
		/// <param name="typeSystem">The type system.</param>
		/// <param name="nativePointerSize">Size of the native pointer.</param>
		/// <param name="nativePointerAlignment">The native pointer alignment.</param>
		public MosaTypeLayout(TypeSystem typeSystem, int nativePointerSize, int nativePointerAlignment)
		{
			Debug.Assert(nativePointerSize == 4 || nativePointerSize == 8);

			NativePointerAlignment = nativePointerAlignment;
			NativePointerSize = nativePointerSize;
			TypeSystem = typeSystem;

			ResolveLayouts();
		}

		/// <summary>
		/// Gets the method table offset.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <returns></returns>
		public int GetMethodTableOffset(MosaMethod method)
		{
			ResolveType(method.DeclaringType);
			return methodTableOffsets[method];
		}

		/// <summary>
		/// Gets the interface slot offset.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public int GetInterfaceSlotOffset(MosaType type)
		{
			ResolveType(type);
			return interfaceSlots[type];
		}

		/// <summary>
		/// Gets the size of the type.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public int GetTypeSize(MosaType type)
		{
			ResolveType(type);

			typeSizes.TryGetValue(type, out int size);

			return size;
		}

		/// <summary>
		/// Gets the size of the field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public int GetFieldSize(MosaField field)
		{
			//FIXME: This is not thread safe!
			if (fieldSizes.TryGetValue(field, out int size))
			{
				return size;
			}
			else
			{
				ResolveType(field.DeclaringType);
			}

			// If the field is another struct, we have to dig down and compute its size too.
			if (field.FieldType.IsValueType)
			{
				size = GetTypeSize(field.FieldType);
			}
			else
			{
				size = GetMemorySize(field.FieldType);
			}

			fieldSizes.Add(field, size);

			return size;
		}

		/// <summary>
		/// Gets the size of the field.
		/// </summary>
		/// <param name="field">The field.</param>
		/// <returns></returns>
		public int GetFieldOffset(MosaField field)
		{
			ResolveType(field.DeclaringType);

			fieldOffsets.TryGetValue(field, out int offset);

			return offset;
		}

		/// <summary>
		/// Gets the type methods.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns></returns>
		public IList<MosaMethod> GetMethodTable(MosaType type)
		{
			if (type.IsModule)
				return null;

			if (type.BaseType == null && !type.IsInterface && type.FullName != "System.Object")   // ghost types like generic params, function ptr, etc.
				return null;

			if (type.Modifier != null)   // For types having modifiers, use the method table of element type instead.
				return GetMethodTable(type.ElementType);

			ResolveType(type);

			return typeMethodTables[type];
		}

		/// <summary>
		/// Gets the interface table.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="interfaceType">Type of the interface.</param>
		/// <returns></returns>
		public MosaMethod[] GetInterfaceTable(MosaType type, MosaType interfaceType)
		{
			if (type.Interfaces.Count == 0)
				return null;

			ResolveType(type);

			var methodTable = new MosaMethod[interfaceType.Methods.Count];

			// Implicit Interface Methods
			for (int slot = 0; slot < interfaceType.Methods.Count; slot++)
			{
				methodTable[slot] = FindInterfaceMethod(type, interfaceType.Methods[slot]);
			}

			// Explicit Interface Methods
			ScanExplicitInterfaceImplementations(type, interfaceType, methodTable);

			return methodTable;
		}

		public bool IsCompoundType(MosaType type)
		{
			// i.e. whether copying of the type requires multiple move
			int? primitiveSize = type.GetPrimitiveSize(NativePointerSize);

			if (primitiveSize != null && primitiveSize > 8)
				return true;

			if (!type.IsUserValueType)
				return false;

			int typeSize = GetTypeSize(type);

			if (typeSize > NativePointerSize)
				return true;

			return false;
		}

		public void SetMethodStackSize(MosaMethod method, int size)
		{
			lock (methodStackSizes)
			{
				methodStackSizes.Remove(method);
				methodStackSizes.Add(method, size);
			}
		}

		public int GetMethodStackSize(MosaMethod method)
		{
			int size = 0;

			lock (methodStackSizes)
			{
				if (!methodStackSizes.TryGetValue(method, out size))
				{
					if ((method.MethodAttributes & MosaMethodAttributes.Abstract) == MosaMethodAttributes.Abstract)
						return 0;

					//throw new InvalidCompilerException();
					return 0;
				}
			}

			return size;
		}

		public int GetMethodParameterStackSize(MosaMethod method)
		{
			lock (parameterStackSize)
			{
				if (parameterStackSize.TryGetValue(method, out int value))
				{
					return value;
				}

				ResolveMethodParameters(method);

				return parameterStackSize[method];
			}
		}

		#region Internal - Layout

		/// <summary>
		/// Resolves the layouts.
		/// </summary>
		private void ResolveLayouts()
		{
			// Enumerate all types and do an appropriate type layout
			foreach (var type in TypeSystem.AllTypes)
			{
				ResolveType(type);
			}

			foreach (var type in TypeSystem.AllTypes)
			{
				foreach (var method in type.Methods)
				{
					ResolveMethodParameters(method);
				}
			}
		}

		/// <summary>
		/// Resolves the type.
		/// </summary>
		/// <param name="type">The type.</param>
		private void ResolveType(MosaType type)
		{
			if (type.IsModule)
				return;

			if (type.BaseType == null && !type.IsInterface && type.FullName != "System.Object")   // ghost types like generic params, function ptr, etc.
				return;

			if (type.Modifier != null)   // For types having modifiers, resolve the element type instead.
			{
				ResolveType(type.ElementType);
				return;
			}

			// FIXME: This is not threadsafe!!!!!
			if (typeSet.Contains(type))
				return;

			typeSet.Add(type);

			if (type.BaseType != null)
			{
				ResolveType(type.BaseType);
			}

			if (type.IsInterface)
			{
				ResolveInterfaceType(type);
				CreateMethodTable(type);
				return;
			}

			foreach (var interfaceType in type.Interfaces)
			{
				ResolveInterfaceType(interfaceType);
			}

			int? size = type.GetPrimitiveSize(NativePointerSize);
			if (size != null)
			{
				typeSizes[type] = size.Value;
			}
			else if (type.IsExplicitLayout)
			{
				ComputeExplicitLayout(type);
			}
			else
			{
				ComputeSequentialLayout(type);
			}

			CreateMethodTable(type);
		}

		/// <summary>
		/// Resolves the method parameters.
		/// </summary>
		/// <param name="method">The method.</param>
		private void ResolveMethodParameters(MosaMethod method)
		{
			var parameters = method.Signature.Parameters;
			int stacksize = 0;

			var offsets = new List<int>(parameters.Count + ((method.HasThis) ? 1 : 0));

			if (method.HasThis)
			{
				offsets.Add(0);
				stacksize = NativePointerSize;  // already aligned
			}

			foreach (var parameter in parameters)
			{
				var size = parameter.ParameterType.IsValueType ? GetTypeSize(parameter.ParameterType) : NativePointerAlignment;

				var sizeAligned = Alignment.AlignUp(size, NativePointerAlignment);

				offsets.Add(stacksize);

				stacksize += sizeAligned;
			}

			int returnSize = 0; //todo

			if (IsStoredOnStack(method.Signature.ReturnType))
			{
				returnSize = GetTypeSize(method.Signature.ReturnType);
			}

			parameterOffsets.Add(method, offsets);
			parameterStackSize.Add(method, stacksize);
			methodReturnSize.Add(method, returnSize);
		}

		/// <summary>
		/// Builds a list of interfaces and assigns interface a unique index number
		/// </summary>
		/// <param name="type">The type.</param>
		private void ResolveInterfaceType(MosaType type)
		{
			Debug.Assert(type.IsInterface);

			if (type.IsModule)
				return;

			if (interfaces.Contains(type))
				return;

			interfaces.Add(type);
			interfaceSlots.Add(type, interfaceSlots.Count);
		}

		private void ComputeSequentialLayout(MosaType type)
		{
			Debug.Assert(type != null, "No type given.");

			if (typeSizes.ContainsKey(type))
				return;

			// Instance size
			int typeSize = 0;

			// Receives the size/alignment
			int packingSize = type.PackingSize ?? NativePointerAlignment;

			if (type.BaseType != null)
			{
				if (!type.IsValueType)
				{
					typeSize = GetTypeSize(type.BaseType);
				}
			}

			foreach (var field in type.Fields)
			{
				if (!field.IsStatic)
				{
					// Set the field address
					fieldOffsets.Add(field, typeSize);

					int fieldSize = GetFieldSize(field);
					typeSize += fieldSize;

					// Pad the field in the type
					if (packingSize != 0)
					{
						int padding = (packingSize - (typeSize % packingSize)) % packingSize;
						typeSize += padding;
					}
				}
			}

			typeSizes.Add(type, typeSize);
		}

		/// <summary>
		/// Applies the explicit layout to the given type.
		/// </summary>
		/// <param name="type">The type.</param>
		private void ComputeExplicitLayout(MosaType type)
		{
			Debug.Assert(type != null, "No type given.");

			//Debug.Assert(type.BaseType.LayoutSize != 0, @"Type size not set for explicit layout.");

			int size = 0;
			foreach (var field in type.Fields)
			{
				if (field.Offset == null)
					continue;

				int offset = (int)field.Offset.Value;
				fieldOffsets.Add(field, offset);
				size = Math.Max(size, offset + GetFieldSize(field));

				// Explicit layout assigns a physical offset from the start of the structure
				// to the field. We just assign this offset.
				Debug.Assert(fieldSizes[field] != 0, "Non-static field doesn't have layout!");
			}

			typeSizes.Add(type, (type.ClassSize == null || type.ClassSize == -1) ? size : (int)type.ClassSize);
		}

		#endregion Internal - Layout

		#region Internal - Interface

		private void ScanExplicitInterfaceImplementations(MosaType type, MosaType interfaceType, MosaMethod[] methodTable)
		{
			foreach (var method in type.Methods)
			{
				foreach (var overrideTarget in method.Overrides)
				{
					// Get clean name for overrideTarget
					var cleanOverrideTargetName = GetCleanMethodName(overrideTarget.Name);

					int slot = 0;
					foreach (var interfaceMethod in interfaceType.Methods)
					{
						// Get clean name for interfaceMethod
						var cleanInterfaceMethodName = GetCleanMethodName(interfaceMethod.Name);

						// Check that the signatures match, the types and the names
						if (overrideTarget.Equals(interfaceMethod) && overrideTarget.DeclaringType.Equals(interfaceType) && cleanOverrideTargetName.Equals(cleanInterfaceMethodName))
						{
							methodTable[slot] = method;
						}
						slot++;
					}
				}
			}
		}

		private MosaMethod FindInterfaceMethod(MosaType type, MosaMethod interfaceMethod)
		{
			MosaMethod methodFound = null;

			if (type.BaseType != null)
			{
				methodFound = FindImplicitInterfaceMethod(type.BaseType, interfaceMethod);
			}

			var cleanInterfaceMethodName = GetCleanMethodName(interfaceMethod.Name);

			foreach (var method in type.Methods)
			{
				if (IsExplicitInterfaceMethod(method.FullName) && methodFound != null)
					continue;

				string cleanMethodName = GetCleanMethodName(method.Name);

				if (cleanInterfaceMethodName.Equals(cleanMethodName))
				{
					if (interfaceMethod.Equals(method))
					{
						return method;
					}
				}
			}

			if (type.BaseType != null)
			{
				methodFound = FindInterfaceMethod(type.BaseType, interfaceMethod);
			}

			if (methodFound != null)
				return methodFound;

			throw new InvalidOperationException("Failed to find implicit interface implementation for type " + type + " and interface method " + interfaceMethod);
		}

		private MosaMethod FindImplicitInterfaceMethod(MosaType type, MosaMethod interfaceMethod)
		{
			MosaMethod methodFound = null;

			var cleanInterfaceMethodName = GetCleanMethodName(interfaceMethod.Name);

			foreach (var method in type.Methods)
			{
				if (IsExplicitInterfaceMethod(method.FullName))
					continue;

				string cleanMethodName = GetCleanMethodName(method.Name);

				if (cleanInterfaceMethodName.Equals(cleanMethodName))
				{
					if (interfaceMethod.Equals(method))
					{
						return method;
					}
				}
			}

			if (type.BaseType != null)
			{
				methodFound = FindImplicitInterfaceMethod(type.BaseType, interfaceMethod);
			}

			return methodFound;
		}

		private string GetCleanMethodName(string fullName)
		{
			if (!fullName.Contains("."))
				return fullName;
			return fullName.Substring(fullName.LastIndexOf(".") + 1);
		}

		private bool IsExplicitInterfaceMethod(string fullname)
		{
			if (fullname.Contains(":"))
				fullname = fullname.Substring(fullname.LastIndexOf(":") + 1);

			return fullname.Contains(".");
		}

		#endregion Internal - Interface

		#region Internal

		private List<MosaMethod> CreateMethodTable(MosaType type)
		{
			if (typeMethodTables.TryGetValue(type, out List<MosaMethod> methodTable))
			{
				return methodTable;
			}

			methodTable = GetMethodTableFromBaseType(type);

			foreach (var method in type.Methods)
			{
				if (method.IsVirtual)
				{
					if (method.IsNewSlot)
					{
						int slot = methodTable.Count;
						methodTable.Add(method);
						methodTableOffsets.Add(method, slot);
					}
					else
					{
						int slot = FindOverrideSlot(methodTable, method);
						if (slot != -1)
						{
							methodTable[slot] = method;
							methodTableOffsets.Add(method, slot);
						}
						else
						{
							slot = methodTable.Count;
							methodTable.Add(method);
							methodTableOffsets.Add(method, slot);
						}
					}
				}
				else
				{
					if (method.IsStatic && method.IsRTSpecialName)
					{
						int slot = methodTable.Count;
						methodTable.Add(method);
						methodTableOffsets.Add(method, slot);
					}
					else if (!method.IsInternal && method.ExternMethod == null)
					{
						int slot = methodTable.Count;
						methodTable.Add(method);
						methodTableOffsets.Add(method, slot);
					}
				}
			}

			typeMethodTables.Add(type, methodTable);

			return methodTable;
		}

		private List<MosaMethod> GetMethodTableFromBaseType(MosaType type)
		{
			var methodTable = new List<MosaMethod>();

			if (type.BaseType != null)
			{
				if (!typeMethodTables.TryGetValue(type.BaseType, out List<MosaMethod> baseMethodTable))
				{
					// Method table for the base type has not been create yet, so create it now
					baseMethodTable = CreateMethodTable(type.BaseType);
				}

				methodTable = new List<MosaMethod>(baseMethodTable);
			}

			return methodTable;
		}

		private int FindOverrideSlot(IList<MosaMethod> methodTable, MosaMethod method)
		{
			int slot = -1;

			foreach (var baseMethod in methodTable)
			{
				if (baseMethod.Name.Equals(method.Name) && baseMethod.Equals(method))
				{
					if (baseMethod.GenericArguments.Count == 0)
						return methodTableOffsets[baseMethod];
					else
						slot = methodTableOffsets[baseMethod];
				}
			}

			if (slot >= 0) // non generic methods are more exact
				return slot;

			//throw new InvalidOperationException(@"Failed to find override method slot.");
			return -1;
		}

		/// <summary>
		/// Gets the type memory requirements.
		/// </summary>
		/// <param name="type">The signature type.</param>
		/// <returns></returns>
		private int GetMemorySize(MosaType type)
		{
			Debug.Assert(!type.IsValueType);
			return NativePointerSize;
		}

		#endregion Internal

		public static bool IsStoredOnStack(MosaType type)
		{
			if (type.IsReferenceType)
				return false;

			if (type.IsUserValueType)
			{
				if (type.Fields != null)
				{
					var nonStaticFields = type.Fields.Where(x => !x.IsStatic).ToList();

					if (nonStaticFields.Count == 1)
					{
						return nonStaticFields[0].FieldType.IsUserValueType;
					}
				}
			}

			return type.IsUserValueType;
		}
	}
}
