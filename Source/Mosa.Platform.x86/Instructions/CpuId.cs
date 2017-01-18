﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 CPUID instruction.
	/// </summary>
	public sealed class CpuId : X86Instruction
	{
		#region Data Members

		private static readonly LegacyOpCode opcode = new LegacyOpCode(new byte[] { 0x0F, 0xA2 }); // Move imm32 to r/m32

		#endregion Data Members

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="CpuId"/>.
		/// </summary>
		public CpuId() :
			base(1, 1)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="ctx">The context.</param>
		/// <param name="emitter">The emitter.</param>
		internal override void EmitLegacy(InstructionNode node, X86CodeEmitter emitter)
		{
			emitter.Emit(opcode);
		}

		#endregion Methods
	}
}
