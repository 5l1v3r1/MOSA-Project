﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.Vm
{
    /// <summary>
    /// Base class of all runtime type system objects.
    /// </summary>
    public abstract class RuntimeObject
    {
        #region Data members

        /// <summary>
        /// Holds the token of the object.
        /// </summary>
        private int _token;

        #endregion // Data members

        #region Construction

        /// <summary>
        /// Initializes a new instance of <see cref="RuntimeObject"/>.
        /// </summary>
        /// <param name="token">The runtime token of this metadata.</param>
        protected RuntimeObject(int token)
        {
            _token = token;
        }

        #endregion // Construction

        #region Properties

        /// <summary>
        /// Retrieves the token of the object.
        /// </summary>
        public int Token
        {
            get { return _token; }
        }

        #endregion // Properties
    }
}
