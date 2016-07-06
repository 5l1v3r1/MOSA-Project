﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using System;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 xor instruction.
	/// </summary>
	public sealed class Xor : TwoOperandInstruction
	{
		#region Data Members

		private static readonly OpCode R_C = new OpCode(new byte[] { 0x81 }, 6);
		private static readonly OpCode R_M = new OpCode(new byte[] { 0x33 });
		private static readonly OpCode R_R = new OpCode(new byte[] { 0x33 });
		private static readonly OpCode M_R = new OpCode(new byte[] { 0x31 });
		private static readonly OpCode M_C = new OpCode(new byte[] { 0x81 }, 6);

		#endregion Data Members

		#region Methods

		/// <summary>
		/// Computes the opcode.
		/// </summary>
		/// <param name="destination">The destination operand.</param>
		/// <param name="source">The source operand.</param>
		/// <param name="third">The third operand.</param>
		/// <returns></returns>
		protected override OpCode ComputeOpCode(Operand destination, Operand source, Operand third)
		{
			if (destination.IsRegister && third.IsConstant) return R_C;
			if (destination.IsRegister && third.IsRegister) return R_R;

			throw new ArgumentException(@"No opcode for operand type.");
		}

		#endregion Methods
	}
}
