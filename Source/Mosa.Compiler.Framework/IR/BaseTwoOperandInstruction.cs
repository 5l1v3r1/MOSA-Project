﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Compiler.Framework.IR
{
	/// <summary>
	/// Abstract base class for IR instructions with two operands.
	/// </summary>
	/// <remarks>
	/// The <see cref="BaseTwoOperandInstruction"/> is the base class for
	/// IR instructions using two operands. It provides properties to
	/// easily access the individual operands.
	/// </remarks>
	public abstract class BaseTwoOperandInstruction : BaseIRInstruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="BaseThreeOperandInstruction"/>.
		/// </summary>
		public BaseTwoOperandInstruction() :
			base(1, 1)
		{
		}

		#endregion Construction
	}
}
