﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Platform;
using System.Diagnostics;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 MovLoad instruction.
	/// </summary>
	public sealed class MovLoad : X86Instruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="MovLoad"/>.
		/// </summary>
		public MovLoad() :
			base(1, 2)
		{
		}

		#endregion Construction

		#region Properties

		public override bool ThreeTwoAddressConversion { get { return false; } }

		#endregion Properties

		#region Methods

		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="emitter">The emitter.</param>
		protected override void Emit(InstructionNode node, MachineCodeEmitter emitter)
		{
			MovMemoryToReg(node, emitter);
		}

		private static void MovMemoryToReg(InstructionNode node, MachineCodeEmitter emitter)
		{
			Debug.Assert(node.Result.IsCPURegister);

			var linkreference = node.Operand1.IsLabel || node.Operand1.IsField || node.Operand1.IsSymbol;

			// memory to reg 1000 101w: mod reg r/m
			var opcode = new OpcodeEncoder()
				.AppendConditionalPrefix(0x66, node.Size == InstructionSize.Size16)  // 8:prefix: 16bit
				.AppendNibble(Bits.b1000)                                       // 4:opcode
				.Append3Bits(Bits.b101)                                         // 3:opcode
				.AppendWidthBit(node.Size != InstructionSize.Size8)                  // 1:width
				.ModRegRMSIBDisplacement(node.Result, node.Operand1, node.Operand2) // Mod-Reg-RM-?SIB-?Displacement
				.AppendConditionalIntegerValue(0, linkreference);               // 32:memory

			if (linkreference)
				emitter.Emit(opcode, node.Operand1, (opcode.Size - 32) / 8);
			else
				emitter.Emit(opcode);
		}

		#endregion Methods
	}
}
