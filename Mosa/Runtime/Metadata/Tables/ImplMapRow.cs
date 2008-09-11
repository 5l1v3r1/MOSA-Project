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
	public struct ImplMapRow {
		#region Data members

        /// <summary>
        /// 
        /// </summary>
        private PInvokeAttributes _mappingFlags;

        /// <summary>
        /// 
        /// </summary>
        private TokenTypes _memberForwardedTableIdx;

        /// <summary>
        /// 
        /// </summary>
        private TokenTypes _importNameStringIdx;

        /// <summary>
        /// 
        /// </summary>
        private TokenTypes _importScopeTableIdx;

		#endregion // Data members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="ImplMapRow"/> struct.
        /// </summary>
        /// <param name="mappingFlags">The mapping flags.</param>
        /// <param name="memberForwardedTableIdx">The member forwarded table idx.</param>
        /// <param name="importNameStringIdx">The import name string idx.</param>
        /// <param name="importScopeTableIdx">The import scope table idx.</param>
        public ImplMapRow(PInvokeAttributes mappingFlags, TokenTypes memberForwardedTableIdx, 
            TokenTypes importNameStringIdx, TokenTypes importScopeTableIdx)
        {
            _mappingFlags = mappingFlags;
            _memberForwardedTableIdx = memberForwardedTableIdx;
            _importNameStringIdx = importNameStringIdx;
            _importScopeTableIdx = importScopeTableIdx;
        }

        #endregion // Construction

        #region Properties

        /// <summary>
        /// Gets the mapping flags.
        /// </summary>
        /// <value>The mapping flags.</value>
        public PInvokeAttributes MappingFlags
        {
            get { return _mappingFlags; }
        }

        /// <summary>
        /// Gets the member forwarded table idx.
        /// </summary>
        /// <value>The member forwarded table idx.</value>
        public TokenTypes MemberForwardedTableIdx
        {
            get { return _memberForwardedTableIdx; }
        }

        /// <summary>
        /// Gets the import name string idx.
        /// </summary>
        /// <value>The import name string idx.</value>
        public TokenTypes ImportNameStringIdx
        {
            get { return _importNameStringIdx; }
        }

        /// <summary>
        /// Gets the import scope table idx.
        /// </summary>
        /// <value>The import scope table idx.</value>
        public TokenTypes ImportScopeTableIdx
        {
            get { return _importScopeTableIdx; }
        }

        #endregion // Properties
	}
}
