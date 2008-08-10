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

namespace Mosa.Runtime.CompilerFramework
{
    /// <summary>
    /// Specifies the CLI stack type of a type reference.
    /// </summary>
	public enum StackTypeCode {
        /// <summary>
        /// Unknown stack type. This most likely hasn't been processed yet.
        /// </summary>
		Unknown = 0,

		/// <summary>
		/// CLI stack type is int32.
		/// </summary>
		Int32 = 1,

		/// <summary>
		/// CLI stack type is int64.
		/// </summary>
		Int64 = 2,

		/// <summary>
		/// CLI stack type is native int.
		/// </summary>
		N = 3,

		/// <summary>
		/// CLI stack type is native floating point.
		/// </summary>
		F = 4,

		/// <summary>
		/// CLI Stack type managed ptr.
		/// </summary>
		Ptr = 5,

		/// <summary>
		/// CLI stack type is object reference.
		/// </summary>
		O = 6,
	}
}
