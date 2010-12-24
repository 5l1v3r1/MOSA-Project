﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 */

namespace Mosa.Runtime.Metadata.Signatures
{
	public class VariableSignature : Signature
	{
		private CustomMod[] customMods;
		private CilElementType modifier;
		private SigType type;

		/// <summary>
		/// Initializes a new instance of the <see cref="VariableSignature"/> class.
		/// </summary>
		/// <param name="reader">The reader.</param>
		public VariableSignature(SignatureReader reader)
			: base(reader)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="VariableSignature"/> class.
		/// </summary>
		/// <param name="provider">The provider.</param>
		/// <param name="token">The token.</param>
		public VariableSignature(IMetadataProvider provider, TokenTypes token)
			: base(provider, token)
		{
		}

		/// <summary>
		/// Gets the custom mods.
		/// </summary>
		/// <value>The custom mods.</value>
		public CustomMod[] CustomMods
		{
			get { return this.customMods; }
		}

		public CilElementType Modifier
		{
			get { return this.modifier; }
		}

		/// <summary>
		/// Gets the type.
		/// </summary>
		/// <value>The type.</value>
		public SigType Type
		{
			get { return this.type; }
			protected set { this.type = value; }
		}

		protected override void ParseSignature(SignatureReader reader)
		{
			this.ParseModifier(reader);

			this.customMods = CustomMod.ParseCustomMods(reader);
			this.type = SigType.ParseTypeSignature(reader);
		}

		private void ParseModifier(SignatureReader reader)
		{
			CilElementType value = (CilElementType)reader.PeekByte();
			if (value == CilElementType.Pinned)
			{
				this.modifier = value;
				reader.SkipByte();
			}
		}

		public void ApplyGenericType(SigType[] genericArguments)
		{
			if (this.Type is VarSigType)
			{
				this.Type = genericArguments[(Type as VarSigType).Index];
			}
		}

	}
}