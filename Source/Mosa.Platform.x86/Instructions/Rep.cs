// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Rep
	/// </summary>
	/// <seealso cref="Mosa.Platform.x86.X86Instruction" />
	public sealed class Rep : X86Instruction
	{
		public static readonly byte[] opcode = new byte[] { 0xF3 };

		public Rep()
			: base(0, 0)
		{
		}

		public override bool HasIRUnspecifiedSideEffect { get { return true; } }

		public override void Emit(InstructionNode node, BaseCodeEmitter emitter)
		{
			System.Diagnostics.Debug.Assert(node.ResultCount == DefaultResultCount || VariableOperands);
			System.Diagnostics.Debug.Assert(node.OperandCount == DefaultOperandCount || VariableOperands);

			emitter.Write(opcode);
		}

		// The following is used by the automated code generator.

		public override byte[] __opcode { get { return opcode; } }
	}
}

