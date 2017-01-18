﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	///
	/// </summary>
	public class Roundsd : X86Instruction
	{
		private static readonly LegacyOpCode opcode = new LegacyOpCode(new byte[] { 0x66, 0x0F, 0x3A, 0x0B });

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Roundsd"/>.
		/// </summary>
		public Roundsd() :
			base(1, 2)
		{
		}

		#endregion Construction

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
	}
}
