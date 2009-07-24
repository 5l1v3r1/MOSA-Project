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
using System.IO;

namespace Mosa.Runtime.Metadata.Tables
{
    /// <summary>
    /// 
    /// </summary>
	public struct ModuleRow {
		#region Data members

        /// <summary>
        /// 
        /// </summary>
        private ushort _generation;

        /// <summary>
        /// 
        /// </summary>
        private TokenTypes _nameStringIdx;

        /// <summary>
        /// 
        /// </summary>
        private TokenTypes _mvidGuidIdx;

        /// <summary>
        /// 
        /// </summary>
        private TokenTypes _encIdGuidIdx;

        /// <summary>
        /// 
        /// </summary>
        private TokenTypes _encBaseIdGuidIdx;

		#endregion // Data members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleRow"/> struct.
        /// </summary>
        /// <param name="generation">The generation.</param>
        /// <param name="nameStringIdx">The name string idx.</param>
        /// <param name="mvidGuidIdx">The mvid GUID idx.</param>
        /// <param name="encIdGuidIdx">The enc id GUID idx.</param>
        /// <param name="encBaseIdGuidIdx">The enc base id GUID idx.</param>
        public ModuleRow(ushort generation, 
                            TokenTypes nameStringIdx, 
                            TokenTypes mvidGuidIdx, 
                            TokenTypes encIdGuidIdx, 
                            TokenTypes encBaseIdGuidIdx)
        {
            _generation = generation;
            _nameStringIdx = nameStringIdx;
            _mvidGuidIdx = mvidGuidIdx;
            _encIdGuidIdx = encIdGuidIdx;
            _encBaseIdGuidIdx = encBaseIdGuidIdx;
        }

        #endregion // Construction

        #region Properties

        /// <summary>
        /// Gets the enc base id GUID idx.
        /// </summary>
        /// <value>The enc base id GUID idx.</value>
        public TokenTypes EncBaseIdGuidIdx
        {
            get { return _encBaseIdGuidIdx; }
        }

        /// <summary>
        /// Gets the enc id GUID idx.
        /// </summary>
        /// <value>The enc id GUID idx.</value>
        public TokenTypes EncIdGuidIdx
        {
            get { return _encIdGuidIdx; }
        }

        /// <summary>
        /// Gets the generation.
        /// </summary>
        /// <value>The generation.</value>
        public ushort Generation
        {
            get { return _generation; }
        }

        /// <summary>
        /// Gets the mvid GUID idx.
        /// </summary>
        /// <value>The mvid GUID idx.</value>
        public TokenTypes MvidGuidIdx
        {
            get { return _mvidGuidIdx; }
        }

        /// <summary>
        /// Gets the name string idx.
        /// </summary>
        /// <value>The name string idx.</value>
        public TokenTypes NameStringIdx
        {
            get { return _nameStringIdx; }
        }

        #endregion // Properties
	}
}
