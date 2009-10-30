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

using Mosa.Runtime.Metadata;

namespace Mosa.Runtime.CompilerFramework.CIL
{
	/// <summary>
	/// 
	/// </summary>
	public sealed class LdelemaInstruction : BinaryInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="LdelemaInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public LdelemaInstruction(OpCode opcode)
			: base(opcode, 1)
		{
		}

		#endregion // Construction

		#region Methods

		/// <summary>
		/// Decodes the specified instruction.
		/// </summary>
		/// <param name="ctx">The context.</param>
		/// <param name="decoder">The instruction decoder, which holds the code stream.</param>
		public override void Decode(Context ctx, IInstructionDecoder decoder)
		{
			// Decode base classes first
			base.Decode(ctx, decoder);

			// Load the immediate argument
			// Retrieve the provider token to check against
			TokenTypes token;
			decoder.Decode(out token);
			throw new NotImplementedException();
			/*
				TypeReference targetType = MetadataTypeReference.FromToken(decoder.Metadata, token);
				_results[0] = CreateResultOperand(new ReferenceTypeSpecification(targetType));
			 */
		}

		/// <summary>
		/// Allows visitor based dispatch for this instruction object.
		/// </summary>
		/// <param name="visitor">The visitor.</param>
		/// <param name="context">The context.</param>
		public override void Visit(ICILVisitor visitor, Context context)
		{
			visitor.Ldelema(context);
		}

		#endregion Methods

	}
}
