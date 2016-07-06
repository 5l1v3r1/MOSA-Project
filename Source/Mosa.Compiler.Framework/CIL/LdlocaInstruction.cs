﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.MosaTypeSystem;

namespace Mosa.Compiler.Framework.CIL
{
	/// <summary>
	///
	/// </summary>
	public sealed class LdlocaInstruction : LoadInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="LdlocaInstruction"/> class.
		/// </summary>
		public LdlocaInstruction(OpCode opCode)
			: base(opCode)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Decodes the specified instruction.
		/// </summary>
		/// <param name="ctx">The context.</param>
		/// <param name="decoder">The instruction decoder, which holds the code stream.</param>
		public override void Decode(InstructionNode ctx, IInstructionDecoder decoder)
		{
			// Decode base classes first
			base.Decode(ctx, decoder);

			// Opcode specific handling

			Operand local = decoder.Compiler.LocalVariables[(int)decoder.Instruction.Operand];

			local = decoder.ConvertVirtualRegisterToStackLocal(local);

			ctx.Operand1 = local;
			ctx.Result = decoder.Compiler.CreateVirtualRegister(local.Type.ToManagedPointer());
		}

		#endregion Methods
	}
}
