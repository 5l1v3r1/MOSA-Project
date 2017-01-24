﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representation the x86 jump instruction.
	/// </summary>
	public sealed class FarJmp : X86Instruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="FarJmp" /> class.
		/// </summary>
		public FarJmp()
			: base(0, 1)
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
			emitter.EmitFarJumpToNextInstruction();
		}

		#endregion Methods
	}
}
