﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.CompilerFramework.Linker
{
    /// <summary>
    /// Specifies the type of link to perform by the linker.
    /// </summary>
    [Flags]
    public enum LinkType : byte
    {
        /// <summary>
        /// Specifies the kind of link to perform.
        /// </summary>
        KindMask = 0xF0,

        /// <summary>
        /// The link destination receives a relative address.
        /// </summary>
        RelativeOffset = 0x80,

        /// <summary>
        /// The link destination receives the absolute address.
        /// </summary>
        AbsoluteAddress = 0x40,

        /// <summary>
        /// Mask to retrieve the size of the address to store.
        /// </summary>
        SizeMask = 0x0F,

        /// <summary>
        /// An 8-bit offset link.
        /// </summary>
        I1 = 0x01,

        /// <summary>
        /// A 16-bit offset link.
        /// </summary>
        I2 = 0x02,

        /// <summary>
        /// A 32-bit offset link.
        /// </summary>
        I4 = 0x04,

        /// <summary>
        /// A 64-bit offset link.
        /// </summary>
        I8 = 0x08,
    }
}
