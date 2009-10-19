/*
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
using Mosa.Runtime.Vm;
using System.IO;

namespace Mosa.Runtime.Linker
{
    /// <summary>
    /// Collects linker requests for processing in the AssemblyLinkerStage.
    /// </summary>
    /// <remarks>
    /// The assembly linker collector performs runtime specific requests in order to resolve a metadata object
    /// to its physical virtualAddress in memory. All link requests require the metadata object, the request virtualAddress
    /// and a relative flag. These are used to either resolve the request immediately or patch the code during
    /// a later linker stage, when all methods and fields have been compiled.
    /// <para/>
    /// The methods return a long instead of IntPtr to allow cross-compilation for 64-bit on a 32-bit system.
    /// </remarks>
    public interface IAssemblyLinker
    {
        #region Properties

        /// <summary>
        /// Gets the base virtualAddress.
        /// </summary>
        /// <value>The base virtualAddress.</value>
        long BaseAddress { get; }

        /// <summary>
        /// Gets or sets the entry point symbol.
        /// </summary>
        /// <value>The entry point symbol.</value>
        LinkerSymbol EntryPoint { get; set; }

        /// <summary>
        /// Gets the load alignment of sections.
        /// </summary>
        /// <value>The load alignment.</value>
        long LoadSectionAlignment { get; }

        /// <summary>
        /// Retrieves the collection of sections created during compilation.
        /// </summary>
        /// <value>The sections collection.</value>
        ICollection<LinkerSection> Sections { get; }

        /// <summary>
        /// Retrieves the collection of symbols known by the linker.
        /// </summary>
        /// <value>The symbol collection.</value>
        ICollection<LinkerSymbol> Symbols { get; }

        /// <summary>
        /// Gets or sets the output file of the linker.
        /// </summary>
        /// <value>The output file.</value>
        string OutputFile { get; set; }

        /// <summary>
        /// Gets the time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        DateTime TimeStamp { get; }

        /// <summary>
        /// Gets the virtual alignment of sections.
        /// </summary>
        /// <value>The virtual section alignment.</value>
        long VirtualSectionAlignment { get; }

        #endregion // Properties

        #region Methods

        /// <summary>
        /// Allocates memory in the specified section.
        /// </summary>
        /// <param name="symbol">The metadata member to allocate space for.</param>
        /// <param name="section">The executable section to allocate From.</param>
        /// <param name="size">The number of bytes to allocate. If zero, indicates an unknown amount of memory is required.</param>
        /// <param name="alignment">The alignment. A value of zero indicates the use of a default alignment for the section.</param>
        /// <returns>A stream, which can be used to populate the section.</returns>
        Stream Allocate(RuntimeMember symbol, SectionKind section, int size, int alignment);

        /// <summary>
        /// Allocates a symbol of the given name in the specified section.
        /// </summary>
        /// <param name="name">The name of the symbol.</param>
        /// <param name="section">The executable section to allocate From.</param>
        /// <param name="size">The number of bytes to allocate. If zero, indicates an unknown amount of memory is required.</param>
        /// <param name="alignment">The alignment. A value of zero indicates the use of a default alignment for the section.</param>
        /// <returns>A stream, which can be used to populate the section.</returns>
        Stream Allocate(string name, SectionKind section, int size, int alignment);

        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <param name="sectionKind">Kind of the section.</param>
        /// <returns>The section of the requested kind.</returns>
        LinkerSection GetSection(SectionKind sectionKind);

        /// <summary>
        /// Retrieves a linker symbol.
        /// </summary>
        /// <param name="member">The runtime member to retrieve a symbol for.</param>
        /// <returns>A linker symbol, which represents the runtime member.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="member"/> is null.</exception>
        /// <exception cref="System.ArgumentException">There's no symbol for the given member.</exception>
        LinkerSymbol GetSymbol(RuntimeMember member);

        /// <summary>
        /// Retrieves a linker symbol.
        /// </summary>
        /// <param name="symbolName">The name of the symbol to retrieve.</param>
        /// <returns>The named linker symbol.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="symbolName"/> is null.</exception>
        /// <exception cref="System.ArgumentException">There's no symbol of the given name.</exception>
        LinkerSymbol GetSymbol(string symbolName);

        /// <summary>
        /// Issues a linker request for the given runtime method.
        /// </summary>
        /// <param name="linkType">The type of link required.</param>
        /// <param name="method">The method the patched code belongs to.</param>
        /// <param name="methodOffset">The offset inside the method where the patch is placed.</param>
        /// <param name="methodRelativeBase">The base virtualAddress, if a relative link is required.</param>
        /// <param name="target">The method or static field to link against.</param>
        /// <param name="offset">An offset to apply to the link target.</param>
        /// <returns>
        /// The return value is the preliminary virtualAddress to place in the generated machine 
        /// code. On 32-bit systems, only the lower 32 bits are valid. The above are not used. An implementation of
        /// IAssemblyLinker may not rely on 64-bits being stored in the memory defined by position.
        /// </returns>
        long Link(LinkType linkType, RuntimeMethod method, int methodOffset, int methodRelativeBase, RuntimeMember target, IntPtr offset);

        /// <summary>
        /// Issues a linker request for the given runtime method.
        /// </summary>
        /// <param name="linkType">The type of link required.</param>
        /// <param name="method">The method the patched code belongs to.</param>
        /// <param name="methodOffset">The offset inside the method where the patch is placed.</param>
        /// <param name="methodRelativeBase">The base virtualAddress, if a relative link is required.</param>
        /// <param name="symbolName">The linker symbol name to link against.</param>
        /// <param name="offset">An offset to apply to the link target.</param>
        /// <returns>
        /// The return value is the preliminary virtualAddress to place in the generated machine 
        /// code. On 32-bit systems, only the lower 32 bits are valid. The above are not used. An implementation of
        /// IAssemblyLinker may not rely on 64-bits being stored in the memory defined by position.
        /// </returns>
        long Link(LinkType linkType, RuntimeMethod method, int methodOffset, int methodRelativeBase, string symbolName, IntPtr offset);

        /// <summary>
        /// Issues a linker request for the given runtime method.
        /// </summary>
        /// <param name="linkType">The type of link required.</param>
        /// <param name="symbolName">The method the patched code belongs to.</param>
        /// <param name="methodOffset">The offset inside the method where the patch is placed.</param>
        /// <param name="methodRelativeBase">The base virtualAddress, if a relative link is required.</param>
        /// <param name="targetSymbol">The linker symbol name to link against.</param>
        /// <param name="offset">An offset to apply to the link target.</param>
        /// <returns>
        /// The return value is the preliminary virtualAddress to place in the generated machine 
        /// code. On 32-bit systems, only the lower 32 bits are valid. The above are not used. An implementation of
        /// IAssemblyLinker may not rely on 64-bits being stored in the memory defined by position.
        /// </returns>
        long Link(LinkType linkType, string symbolName, int methodOffset, int methodRelativeBase, string targetSymbol, IntPtr offset);

        /// <summary>
        /// Creates a canonical symbol name for the given runtime member
        /// </summary>
        /// <param name="member">The runtime member to create a symbol name for</param>
        /// <returns>A string containing the canonical symbol name for the given runtime member</returns>
        string CreateSymbolName(RuntimeMember member);

        #endregion // Methods
    }
}
