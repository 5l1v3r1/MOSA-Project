﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representation the x86 jump instruction.
	/// </summary>
	public sealed class Jmp : X86Instruction
	{
		#region Data Members

		private static readonly byte[] JMP = new byte[] { 0xE9 };
		private static readonly LegacyOpCode JMP_R = new LegacyOpCode(new byte[] { 0xFF }, 4);

		#endregion Data Members

		#region Properties

		public override FlowControl FlowControl { get { return FlowControl.UnconditionalBranch; } }

		#endregion Properties

		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Jmp"/>.
		/// </summary>
		public Jmp() :
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
			if (node.Operand1 == null)
			{
				emitter.EmitRelativeBranch(JMP, node.BranchTargets[0].Label);
			}
			else
			{
				if (node.Operand1.IsSymbol)
				{
					emitter.WriteByte(0xE9);
					emitter.EmitCallSite(node.Operand1);
				}
				else if (node.Operand1.IsCPURegister)
				{
					emitter.Emit(JMP_R, node.Operand1);
				}
			}
		}

		#endregion Methods
	}
}
