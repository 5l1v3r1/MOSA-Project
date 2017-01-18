﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Intermediate representation of the SSE addition instruction.
	/// </summary>
	public sealed class Addsd : TwoOperandInstruction
	{
		#region Data Members

		private static readonly LegacyOpCode opcode = new LegacyOpCode(new byte[] { 0xF2, 0x0F, 0x58 });

		#endregion Data Members

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
			return opcode;
		}

		#endregion Methods
	}
}
