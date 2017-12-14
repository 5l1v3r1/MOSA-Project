﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 lock instruction.
	/// </summary>
	public sealed class Lock : X86Instruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Lock"/>.
		/// </summary>
		public Lock() :
			base(0, 0)
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
			emitter.WriteByte(0xF0);
		}

		#endregion Methods
	}
}
