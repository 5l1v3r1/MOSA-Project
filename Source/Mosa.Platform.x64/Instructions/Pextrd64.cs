// Copyright (c) MOSA Project. Licensed under the New BSD License.

// This code was generated by an automated template.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x64.Instructions
{
	/// <summary>
	/// Pextrd64
	/// </summary>
	/// <seealso cref="Mosa.Platform.x64.X64Instruction" />
	public sealed class Pextrd64 : X64Instruction
	{
		internal Pextrd64()
			: base(1, 2)
		{
		}

		public override void Emit(InstructionNode node, BaseCodeEmitter emitter)
		{
			System.Diagnostics.Debug.Assert(node.ResultCount == 1);
			System.Diagnostics.Debug.Assert(node.OperandCount == 2);

			emitter.OpcodeEncoder.SuppressByte(0x40);
			emitter.OpcodeEncoder.Append4Bits(0b0100);
			emitter.OpcodeEncoder.Append1Bit(0b1);
			emitter.OpcodeEncoder.Append1Bit((node.Operand1.Register.RegisterCode >> 3) & 0x1);
			emitter.OpcodeEncoder.Append1Bit(0b0);
			emitter.OpcodeEncoder.Append1Bit((node.Result.Register.RegisterCode >> 3) & 0x1);
			emitter.OpcodeEncoder.Append4Bits(0b0110);
			emitter.OpcodeEncoder.Append4Bits(0b0110);
			emitter.OpcodeEncoder.Append4Bits(0b0000);
			emitter.OpcodeEncoder.Append4Bits(0b1111);
			emitter.OpcodeEncoder.Append4Bits(0b0011);
			emitter.OpcodeEncoder.Append4Bits(0b1010);
			emitter.OpcodeEncoder.Append4Bits(0b0001);
			emitter.OpcodeEncoder.Append4Bits(0b0110);
			emitter.OpcodeEncoder.Append2Bits(0b11);
			emitter.OpcodeEncoder.Append3Bits(node.Operand1.Register.RegisterCode);
			emitter.OpcodeEncoder.Append3Bits(node.Result.Register.RegisterCode);
			emitter.OpcodeEncoder.Append8BitImmediate(node.Operand2);
		}
	}
}
