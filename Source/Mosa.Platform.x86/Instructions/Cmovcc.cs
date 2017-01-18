﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;
using System;

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representation a x86 Cmov instruction.
	/// </summary>
	public sealed class Cmovcc : X86Instruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Cmovcc"/>.
		/// </summary>
		public Cmovcc() :
			base(0, 2)
		{
		}

		#endregion Construction

		#region Data Members

		private static readonly LegacyOpCode CMOVO = new LegacyOpCode(new byte[] { 0x0F, 0x40 });   // Overflow (OF = 1)
		private static readonly LegacyOpCode CMOVNO = new LegacyOpCode(new byte[] { 0x0F, 0x41 });  // NoOverflow (OF = 0)
		private static readonly LegacyOpCode CMOVB = new LegacyOpCode(new byte[] { 0x0F, 0x42 });   // UnsignedLessThan (CF = 1).
		private static readonly LegacyOpCode CMOVC = new LegacyOpCode(new byte[] { 0x0F, 0x42 });   // Carry (CF = 1)
		private static readonly LegacyOpCode CMOVNB = new LegacyOpCode(new byte[] { 0x0F, 0x43 });  // UnsignedGreaterOrEqual (greater or equal) (CF = 0)
		private static readonly LegacyOpCode CMOVNC = new LegacyOpCode(new byte[] { 0x0F, 0x43 });  // NoCarry (CF = 0)
		private static readonly LegacyOpCode CMOVE = new LegacyOpCode(new byte[] { 0x0F, 0x44 });   // Equal (ZF = 1)
		private static readonly LegacyOpCode CMOVZ = new LegacyOpCode(new byte[] { 0x0F, 0x44 });   // Zero (ZF = 1)
		private static readonly LegacyOpCode CMOVNE = new LegacyOpCode(new byte[] { 0x0F, 0x45 });  // NotEqual (ZF = 0)
		private static readonly LegacyOpCode CMOVNZ = new LegacyOpCode(new byte[] { 0x0F, 0x45 });  // NotZero (ZF = 1)
		private static readonly LegacyOpCode CMOVBE = new LegacyOpCode(new byte[] { 0x0F, 0x46 });  // UnsignedLessOrEqual (CF = 1 or ZF = 1).
		private static readonly LegacyOpCode CMOVA = new LegacyOpCode(new byte[] { 0x0F, 0x47 });   // UnsignedGreaterThan (CF = 0 and ZF = 0).
		private static readonly LegacyOpCode CMOVS = new LegacyOpCode(new byte[] { 0x0F, 0x48 });   // Signed (CF = 0 and ZF = 0)
		private static readonly LegacyOpCode CMOVNS = new LegacyOpCode(new byte[] { 0x0F, 0x49 });  // NotSigned (SF = 0)
		private static readonly LegacyOpCode CMOVP = new LegacyOpCode(new byte[] { 0x0F, 0x4A });   // Parity (PF = 1)
		private static readonly LegacyOpCode CMOVNP = new LegacyOpCode(new byte[] { 0x0F, 0x4B });  // NoParity (PF = 0)
		private static readonly LegacyOpCode CMOVL = new LegacyOpCode(new byte[] { 0x0F, 0x4C });   // LessThan (SF <> OF)
		private static readonly LegacyOpCode CMOVGE = new LegacyOpCode(new byte[] { 0x0F, 0x4D });  // GreaterOrEqual (greater or equal) (SF = OF)
		private static readonly LegacyOpCode CMOVLE = new LegacyOpCode(new byte[] { 0x0F, 0x4E });  // LessOrEqual (ZF = 1 or SF <> OF)
		private static readonly LegacyOpCode CMOVG = new LegacyOpCode(new byte[] { 0x0F, 0x4F });   // GreaterThan (ZF = 0 and SF = OF)

		#endregion Data Members

		#region Methods

		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="emitter">The emitter.</param>
		/// <exception cref="System.NotSupportedException"></exception>
		internal override void EmitLegacy(InstructionNode node, X86CodeEmitter emitter)
		{
			LegacyOpCode opcode = null;

			switch (node.ConditionCode)
			{
				case ConditionCode.Equal: opcode = CMOVO; break;
				case ConditionCode.NotEqual: opcode = CMOVNE; break;
				case ConditionCode.Zero: opcode = CMOVZ; break;
				case ConditionCode.NotZero: opcode = CMOVNZ; break;
				case ConditionCode.GreaterOrEqual: opcode = CMOVGE; break;
				case ConditionCode.GreaterThan: opcode = CMOVG; break;
				case ConditionCode.LessOrEqual: opcode = CMOVLE; break;
				case ConditionCode.LessThan: opcode = CMOVL; break;
				case ConditionCode.UnsignedGreaterOrEqual: opcode = CMOVNB; break;
				case ConditionCode.UnsignedGreaterThan: opcode = CMOVA; break;
				case ConditionCode.UnsignedLessOrEqual: opcode = CMOVBE; break;
				case ConditionCode.UnsignedLessThan: opcode = CMOVB; break;
				case ConditionCode.Signed: opcode = CMOVS; break;
				case ConditionCode.NotSigned: opcode = CMOVNS; break;
				case ConditionCode.Carry: opcode = CMOVC; break;
				case ConditionCode.NoCarry: opcode = CMOVNC; break;
				case ConditionCode.Overflow: opcode = CMOVO; break;
				case ConditionCode.NoOverflow: opcode = CMOVNO; break;
				case ConditionCode.Parity: opcode = CMOVP; break;
				case ConditionCode.NoParity: opcode = CMOVNP; break;
				default: throw new NotSupportedException();
			}

			emitter.Emit(opcode, node.Result, node.Operand1);
		}

		#endregion Methods
	}
}
