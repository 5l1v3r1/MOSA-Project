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
	public sealed class LeaveInstruction : BranchInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="LeaveInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public LeaveInstruction(OpCode opcode)
			: base(opcode)
		{
		}

		#endregion // Construction

		#region Methods

		/// <summary>
		/// Decodes the specified instruction.
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="decoder">The instruction decoder, which holds the code stream.</param>
		public override void Decode(Context ctx, IInstructionDecoder decoder)
		{
			// Decode base classes first
			base.Decode(ctx, decoder);

			switch (_opcode) {
				case OpCode.Leave_s: {
						sbyte sb;
						decoder.Decode(out sb);
						ctx.SetBranch(sb);
					}
					break;

				case OpCode.Leave: {
						int sb;
						decoder.Decode(out sb);
						ctx.SetBranch(sb);
						break;
					}
			}
		}

		/// <summary>
		/// Allows visitor based dispatch for this instruction object.
		/// </summary>
		/// <param name="vistor">The vistor.</param>
		/// <param name="context">The context.</param>
		public override void Visit(ICILVisitor vistor, Context context)
		{
			vistor.Leave(context);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <param name="ctx">The context.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString(Context ctx)
		{
			return String.Format("leave L_{0:X4}", ctx.Branch.Targets);
		}

		#endregion Methods


	}
}
