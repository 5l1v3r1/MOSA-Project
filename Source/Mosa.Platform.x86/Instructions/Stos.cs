﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations of the x86 stos instruction.
	/// </summary>
	public sealed class Stos : X86Instruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Stos"/>.
		/// </summary>
		public Stos() :
			base(0, 1)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="emitter">The emitter.</param>
		internal override void EmitLegacy(InstructionNode node, X86CodeEmitter emitter)
		{
			emitter.WriteByte(0xAB);
		}

		#endregion Methods
	}
}
