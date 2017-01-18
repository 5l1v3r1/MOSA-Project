﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using System.Diagnostics;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	///
	/// </summary>
	public abstract class TwoOperandInstruction : X86Instruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="TwoOperandInstruction"/>.
		/// </summary>
		protected TwoOperandInstruction() :
			base(1, 2)
		{
		}

		#endregion Construction

		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="node">The context.</param>
		/// <param name="emitter">The emitter.</param>
		internal override void EmitLegacy(InstructionNode node, X86CodeEmitter emitter)
		{
			Debug.Assert(node.Result == node.Operand1);
			LegacyOpCode opCode = ComputeOpCode(node.Result, node.Operand1, node.Operand2);
			emitter.Emit(opCode, node.Result, node.Operand2);
		}
	}
}
