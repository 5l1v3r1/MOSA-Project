/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using Mosa.Runtime.Metadata;
using Mosa.Runtime.Metadata.Signatures;

namespace Mosa.Runtime.CompilerFramework
{

	/// <summary>
	/// Represent a constant operand.
	/// </summary>
	public sealed class ConstantOperand : Operand
    {
		#region Static data members

        private static SigType _sObject;

		#endregion // Static data members

		#region Data members

		/// <summary>
		/// Constant value.
		/// </summary>
		private object _value;

		#endregion // Data members

		#region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantOperand"/> class.
        /// </summary>
        /// <param name="typeRef">The type ref.</param>
        /// <param name="value">The value of the contant.</param>
		public ConstantOperand(SigType typeRef, object value) 
			: base(typeRef)
		{
			_value = value;
		}

		#endregion // Construction

		#region Properties

		/// <summary>
		/// Returns the value of the constant.
		/// </summary>
		public object Value
		{
			get { return _value; }
		}

		#endregion // Properties

        #region Methods

        /// <summary>
        /// Creates a new <see cref="ConstantOperand"/> for the given integral value.
        /// </summary>
        /// <param name="value">The value to create the constant operand for.</param>
        /// <returns>A new ConstantOperand representing the value <paramref name="value"/>.</returns>
        public static ConstantOperand FromValue(int value)
        {
            return new ConstantOperand(new SigType(CilElementType.I4), value);
        }

        /// <summary>
        /// Retrieves a constant operand to represent the null-value.
        /// </summary>
        /// <returns>A new instance of <see cref="ConstantOperand"/>, that represents the null value.</returns>
        public static ConstantOperand GetNull()
        {
            if (null == _sObject)
                _sObject = new SigType(CilElementType.Object);

            return new ConstantOperand(_sObject, null);
        }

        #endregion // Methods

        #region Operand Overrides

        /// <summary>
        /// Compares with the given operand for equality.
        /// </summary>
        /// <param name="other">The other operand to compare with.</param>
        /// <returns>The return value is true if the operands are equal; false if not.</returns>
        public override bool Equals(Operand other)
        {
            ConstantOperand cop = other as ConstantOperand;
            return (null != cop && null != cop.Value && null != Value && cop.Value.Equals(Value));
        }

        /// <summary>
        /// Returns a string representation of <see cref="ConstantOperand"/>.
        /// </summary>
        /// <returns>A string representation of the operand.</returns>
        public override string ToString()
        {
            return String.Format("const {0} {1}", _value, _type);
        }

        #endregion // Operand Overrides
    }
}
