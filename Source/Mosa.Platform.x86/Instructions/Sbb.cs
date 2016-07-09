﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using System;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 sbb instruction.
	/// </summary>
	public sealed class Sbb : TwoOperandInstruction
	{
		#region Data Members

		private static readonly OpCode RM_C = new OpCode(new byte[] { 0x81 }, 3);
		private static readonly OpCode R_RM = new OpCode(new byte[] { 0x1B });
		private static readonly OpCode M_R = new OpCode(new byte[] { 0x19 });

		#endregion Data Members

		#region Methods

		/// <summary>
		/// Computes the opcode.
		/// </summary>
		/// <param name="destination">The destination operand.</param>
		/// <param name="source">The source operand.</param>
		/// <param name="third">The third operand.</param>
		/// <returns></returns>
		/// <exception cref="System.ArgumentException"></exception>
		protected override OpCode ComputeOpCode(Operand destination, Operand source, Operand third)
		{
			if (destination.IsCPURegister && third.IsConstant) return RM_C;
			if (destination.IsCPURegister && third.IsCPURegister) return R_RM;

			throw new ArgumentException(@"No opcode for operand type.");
		}

		#endregion Methods
	}
}
