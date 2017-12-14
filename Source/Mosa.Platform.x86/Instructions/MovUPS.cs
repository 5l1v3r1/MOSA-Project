﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using System;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 movaps instruction.
	/// </summary>
	public sealed class Movups : X86Instruction
	{
		#region Data Members

		private static readonly LegacyOpCode R_RM = new LegacyOpCode(new byte[] { 0x0F, 0x10 });
		private static readonly LegacyOpCode RM_R = new LegacyOpCode(new byte[] { 0x0F, 0x11 });

		#endregion Data Members

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Movaps"/>.
		/// </summary>
		public Movups() :
			base(1, 1)
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
			if (destination.IsCPURegister) return R_RM;

			throw new ArgumentException("No opcode for operand type. [" + destination.GetType() + ", " + source.GetType() + ")");
		}

		#endregion Methods
	}
}
