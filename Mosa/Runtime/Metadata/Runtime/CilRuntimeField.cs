﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 */

using System;
using System.Diagnostics;

using Mosa.Runtime.Loader;
using Mosa.Runtime.Metadata.Signatures;
using Mosa.Runtime.Metadata.Tables;
using Mosa.Runtime.Vm;

namespace Mosa.Runtime.Metadata.Runtime
{
    /// <summary>
    /// A CIL specialization of <see cref="RuntimeField"/>.
    /// </summary>
    sealed class CilRuntimeField : RuntimeField
    {
        #region Data Members

        /// <summary>
        /// Holds the name index of the RuntimeField.
        /// </summary>
        private TokenTypes nameIdx;

        /// <summary>
        /// Holds the signature token of the field.
        /// </summary>
        private TokenTypes signature;

        #endregion // Data Members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="CilRuntimeField"/> class.
        /// </summary>
        /// <param name="module">The module.</param>
        /// <param name="field">The field.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="rva">The rva.</param>
        /// <param name="declaringType">Type of the declaring.</param>
        public CilRuntimeField(IMetadataModule module, ref FieldRow field, IntPtr offset, IntPtr rva, RuntimeType declaringType) :
            base(module, declaringType)
        {
            this.nameIdx = field.NameStringIdx;
            this.signature = field.SignatureBlobIdx;
            base.Attributes = field.Flags;
            base.RVA = rva;
            //base.Offset = offset; ?
        }

        #endregion // Construction

        #region RuntimeField Overrides

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public override bool Equals(RuntimeField other)
        {
            CilRuntimeField crf = other as CilRuntimeField;
            return (crf != null && 
                    this.nameIdx == crf.nameIdx && 
                    this.signature == crf.signature && 
                    base.Equals(other) == true);
        }

        /// <summary>
        /// Gets the type of the field.
        /// </summary>
        /// <returns>The type of the field.</returns>
        protected override SigType GetFieldType()
        {
            FieldSignature fsig = new FieldSignature();
            fsig.LoadSignature(this.Module.Metadata, this.signature);
            return fsig.Type;
        }

        /// <summary>
        /// Called to retrieve the name of the type.
        /// </summary>
        /// <returns>The name of the type.</returns>
        protected override string GetName()
        {
            string name;
            this.Module.Metadata.Read(this.nameIdx, out name);
            Debug.Assert(name != null, @"Failed to retrieve CilRuntimeMethod name.");
            return name;
        }

        #endregion // RuntimeField Overrides
    }
}
