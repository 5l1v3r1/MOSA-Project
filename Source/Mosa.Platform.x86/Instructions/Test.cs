﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using System;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 cmp instruction.
	/// </summary>
	public sealed class Test : X86Instruction
	{
		#region Data Member

		private static readonly LegacyOpCode M_R = new LegacyOpCode(new byte[] { 0x85 });

		//private static readonly OpCode R_M = new OpCode(new byte[] { 0x3B });
		private static readonly LegacyOpCode R_R = new LegacyOpCode(new byte[] { 0x85 });

		private static readonly LegacyOpCode M_C = new LegacyOpCode(new byte[] { 0xF7 }, 0);
		private static readonly LegacyOpCode R_C = new LegacyOpCode(new byte[] { 0xF7 }, 0);
		private static readonly LegacyOpCode R_C_8 = new LegacyOpCode(new byte[] { 0xF6 }, 0);
		private static readonly LegacyOpCode R_C_16 = new LegacyOpCode(new byte[] { 0x66, 0xF7 }, 0);
		private static readonly LegacyOpCode M_R_8 = new LegacyOpCode(new byte[] { 0x84 });

		//private static readonly OpCode R_M_8 = new OpCode(new byte[] { 0x3A });
		private static readonly LegacyOpCode M_R_16 = new LegacyOpCode(new byte[] { 0x66, 0x85 });

		//private static readonly OpCode R_M_16 = new OpCode(new byte[] { 0x66, 0x3B });

		#endregion Data Member

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Test"/>.
		/// </summary>
		public Test() :
			base(0, 2)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Computes the opcode.
		/// </summary>
		/// <param name="destination">The destination operand.</param>
		/// <param name="source">The source operand.</param>
		/// <param name="third">The third operand.</param>
		/// <returns></returns>
		internal override LegacyOpCode ComputeOpCode(Operand destination, Operand source, Operand third)
		{
			if (source.IsCPURegister && third.IsCPURegister) return R_R;

			if (source.IsCPURegister && third.IsConstant)
			{
				if (third.IsByte || source.IsByte)
					return R_C_8;
				if (third.IsChar || source.IsChar)
					return R_C_16;
				if (third.IsShort || source.IsShort)
					return R_C_16;
				return R_C;
			}

			throw new ArgumentException(String.Format("x86.Test: No opcode for operand types {0} and {1}.", source, third));
		}

		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="emitter">The emitter.</param>
		internal override void EmitLegacy(InstructionNode node, X86CodeEmitter emitter)
		{
			LegacyOpCode opCode = ComputeOpCode(null, node.Operand1, node.Operand2);
			emitter.Emit(opCode, node.Operand1, node.Operand2);
		}

		#endregion Methods
	}
}
