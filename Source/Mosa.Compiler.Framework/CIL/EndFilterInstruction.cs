﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.CIL
{
	/// <summary>
	///
	/// </summary>
	public sealed class EndFilterInstruction : UnaryInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="EndFilterInstruction"/> class.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public EndFilterInstruction(OpCode opcode)
			: base(opcode)
		{
		}

		#endregion Construction

		public override FlowControl FlowControl { get { return FlowControl.EndFilter; } }

		#region Methods

		/// <summary>
		/// Validates the instruction operands and creates a matching variable for the result.
		/// </summary>
		/// <param name="ctx">The context.</param>
		/// <param name="compiler">The compiler.</param>
		public override void Resolve(Context ctx, BaseMethodCompiler compiler)
		{
			base.Resolve(ctx, compiler);
		}

		#endregion Methods
	}
}
