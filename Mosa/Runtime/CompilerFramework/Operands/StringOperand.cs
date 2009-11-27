﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.CompilerFramework
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StringOperand : Operand
    {
        private string _string;

		/// <summary>
		/// Gets or sets the string.
		/// </summary>
		/// <value>The string.</value>
        public string String
        {
            get
            {
                return _string;
            }
            set
            {
                _string = value;
            }
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="StringOperand"/> class.
		/// </summary>
		/// <param name="value">The value.</param>
        public StringOperand(string value)
            : base(new Metadata.Signatures.SigType(Metadata.CilElementType.String))
        {
            String = value;
        }

		/// <summary>
		/// Compares with the given operand for equality.
		/// </summary>
		/// <param name="other">The other operand to compare with.</param>
		/// <returns>
		/// The return value is true if the operands are equal; false if not.
		/// </returns>
        public override bool Equals(Operand other)
        {
            if (!(other is StringOperand))
                return false;

            StringOperand stringOp = other as StringOperand;
            return String.Equals(stringOp.String);
        }
    }
}
