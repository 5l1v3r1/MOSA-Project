﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Simon Wollwage (rootnode) <kintaro@think-in-co.de>
 */

using System;
using Mosa.Runtime.CompilerFramework;

namespace Mosa.Platforms.x86.CPUx86
{
    /// <summary>
    /// Intermediate representation of the div instruction.
    /// </summary>
    public sealed class DirectDivisionInstruction : TwoOperandInstruction
    {
        #region Data Members

        private static readonly OpCode DIV = new OpCode(new byte[] { 0xF7 }, 6);

        #endregion // Data Members

        #region Properties

        /// <summary>
        /// Gets the instruction latency.
        /// </summary>
        /// <value>The latency.</value>
        public override int Latency { get { return 22; } }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Computes the opcode.
        /// </summary>
        /// <param name="destination">The destination operand.</param>
        /// <param name="source">The source operand.</param>
        /// <param name="third">The third operand.</param>
        /// <returns></returns>
        protected override OpCode ComputeOpCode(Operand destination, Operand source, Operand third)
        {
            if (destination == null || destination is RegisterOperand || destination is MemoryOperand) return DIV;

            throw new ArgumentException(@"No opcode for operand type.");
        }

        /// <summary>
        /// Emits the specified platform instruction.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="emitter">The emitter.</param>
        public override void Emit(Context ctx, MachineCodeEmitter emitter)
        {
            OpCode opCode = ComputeOpCode(ctx.Result, ctx.Operand1, ctx.Operand2);
            emitter.Emit(opCode, ctx.Operand1, null);
        }

        /// <summary>
        /// Allows visitor based dispatch for this instruction object.
        /// </summary>
        /// <param name="visitor">The visitor object.</param>
        /// <param name="context">The context.</param>
        public override void Visit(IX86Visitor visitor, Context context)
        {
            visitor.DirectDivision(context);
        }

        #endregion // Methods
    }
}
