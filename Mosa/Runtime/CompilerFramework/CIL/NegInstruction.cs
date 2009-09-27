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
	public sealed class NegInstruction : UnaryArithmeticInstruction
	{
		#region Data members

		/// <summary>
		/// Holds the typecode validation table From ISO/IEC 23271:2006 (E),
		/// Partition III, §1.5, Table 3.
		/// </summary>
		private static StackTypeCode[] _typeCodes = new StackTypeCode[] {
			StackTypeCode.Unknown,
			StackTypeCode.Int32,
			StackTypeCode.Int64,
			StackTypeCode.N,
			StackTypeCode.Unknown,
			StackTypeCode.Unknown,
			StackTypeCode.Unknown
		};

		#endregion // Data members
		
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="NegInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public NegInstruction(OpCode opcode)
			: base(opcode)
		{
		}

		#endregion // Construction

		#region Methods

		/// <summary>
		/// Validates the instruction operands and creates a matching variable for the result.
		/// </summary>
		/// <param name="ctx"></param>
		/// <param name="compiler">The compiler.</param>
		public override void Validate(Context ctx, IMethodCompiler compiler)
		{
			base.Validate(ctx, compiler);

			// Validate the operand
			StackTypeCode result = _typeCodes[(int)ctx.Operand1.StackType];
			if (StackTypeCode.Unknown == result)
				throw new InvalidOperationException(@"Invalid operand to Neg instruction.");

			ctx.Result = compiler.CreateTemporary(ctx.Operand1.Type);
		}

		/// <summary>
		/// Allows visitor based dispatch for this instruction object.
		/// </summary>
		/// <param name="vistor">The vistor.</param>
		/// <param name="context">The context.</param>
		public override void Visit(ICILVisitor vistor, Context context)
		{
			vistor.Neg(context);
		}

		#endregion Methods

	}
}
