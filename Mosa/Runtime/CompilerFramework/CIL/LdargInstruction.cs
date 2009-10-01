﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Mosa.Runtime.CompilerFramework.CIL
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class LdargInstruction : LoadInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="LdargInstruction"/> class.
		/// </summary>
		public LdargInstruction(OpCode opCode)
			: base(opCode)
		{
		}

		#endregion // Construction

		#region Methods

		/// <summary>
		/// Decodes the specified CIL instruction.
		/// </summary>
		/// <param name="ctx">The context.</param>
		/// <param name="decoder">The instruction decoder, which holds the code stream.</param>
		/// <remarks>
		/// This method is used by instructions to retrieve immediate operands
		/// From the instruction stream.
		/// </remarks>
		public override void Decode(Context ctx, IInstructionDecoder decoder)
		{
			// Decode base classes first
			base.Decode(ctx, decoder);

			ushort argIdx;

			// Opcode specific handling
			switch (_opcode) {
				case OpCode.Ldarg:
					decoder.Decode(out argIdx);
					break;

				case OpCode.Ldarg_s: {
						byte arg;
						decoder.Decode(out arg);
						argIdx = arg;
					}
					break;

				case OpCode.Ldarg_0:
					argIdx = 0;
					break;

				case OpCode.Ldarg_1:
					argIdx = 1;
					break;

				case OpCode.Ldarg_2:
					argIdx = 2;
					break;

				case OpCode.Ldarg_3:
					argIdx = 3;
					break;

				default:
					throw new NotImplementedException();
			}

			// Push the loaded value onto the evaluation stack
			ctx.Result = decoder.Compiler.GetParameterOperand(argIdx);
			ctx.Ignore = true;
		}

		/// <summary>
		/// Allows visitor based dispatch for this instruction object.
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		/// <param name="context">The context.</param>
		public override void Visit(ICILVisitor visitor, Context context)
		{
			visitor.Ldarg(context);
		}

		#endregion Methods
	}
}
