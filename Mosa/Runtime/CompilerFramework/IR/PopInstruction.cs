/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.CompilerFramework.IR
{
    /// <summary>
    /// Intermediate representation of a pop operation, that removes the topmost element From the stack and
    /// places it in the destination operand.
    /// </summary>
    public sealed class PopInstruction: BaseInstruction
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="PopInstruction"/>.
        /// </summary>
        public PopInstruction() :
            base(0, 1)
        {
        }

        #endregion // Construction

        #region Instruction

		/// <summary>
		/// Returns a string representation of the <see cref="PopInstruction"/>.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
        public override string ToString(Context context)
        {
            return String.Format(@"IR.pop {0}", context.Result);
        }

		/// <summary>
		/// Allows visitor based dispatch for this instruction object.
		/// </summary>
		/// <param name="visitor">The visitor object.</param>
		/// <param name="context">The context.</param>
        public override void Visit(IIRVisitor visitor, Context context)
        {
			visitor.PopInstruction(context);
		}

        #endregion // Instruction
    }
}
