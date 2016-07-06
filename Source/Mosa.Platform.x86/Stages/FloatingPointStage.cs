﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using System;
using System.Collections.Generic;

// fixme: this stage may not be necessary with the specific load/store instructions

namespace Mosa.Platform.x86.Stages
{
	/// <summary>
	///
	/// </summary>
	public class FloatingPointStage : BaseTransformationStage
	{
		protected override void PopulateVisitationDictionary()
		{
			// Nothing to do
		}

		protected override void Run()
		{
			foreach (var block in BasicBlocks)
			{
				for (var node = block.First; !node.IsBlockEndInstruction; node = node.Next)
				{
					if (node.IsEmpty || !(node.Instruction is X86Instruction))
						continue;

					if (node.Instruction == X86.Jmp || node.Instruction == X86.FarJmp)
						continue;

					if (node.Instruction == X86.MovsdLoad || node.Instruction == X86.MovssLoad ||
						node.Instruction == X86.MovsdStore || node.Instruction == X86.MovssStore)
						continue;

					// Convert any floating point constants into labels
					EmitFloatingPointConstants(node);

					// No floating point opcode allows both the result and operand to be a memory location
					// if necessary, load into register first
					if (node.OperandCount == 1
						&& node.ResultCount == 1
						&& (node.Result.IsR || node.Operand1.IsR))
					{
						LoadFirstOperandIntoRegister(node);
					}
				}
			}
		}

		private void LoadFirstOperandIntoRegister(InstructionNode node)
		{
			// load into a register
			Operand operand = node.Operand1;

			Operand register = AllocateVirtualRegister(operand.Type);
			node.Operand1 = register;

			X86Instruction move = null;
			InstructionSize size = InstructionSize.None;

			if (register.IsR4)
			{
				move = X86.Movss;
				size = InstructionSize.Size32;
			}
			else
			{
				move = X86.Movsd;
				size = InstructionSize.Size64;
			}

			var newNode = new InstructionNode(move, register, operand);
			newNode.Size = size;
			node.Previous.Insert(newNode);
		}

		private bool IsCommutative(BaseInstruction instruction)
		{
			return (instruction == X86.Addsd || instruction == X86.Addss || instruction == X86.Mulsd || instruction == X86.Mulss);
		}
	}
}
