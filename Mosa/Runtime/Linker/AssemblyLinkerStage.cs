﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Mosa.Runtime.CompilerFramework;
using Mosa.Runtime.Metadata;
using Mosa.Runtime.Vm;

namespace Mosa.Runtime.Linker
{
    /// <summary>
    /// This compilation stage links all external labels together, which
    /// were previously registered.
    /// </summary>
    public abstract class AssemblyLinkerStageBase : IAssemblyCompilerStage, IAssemblyLinker
    {
        #region Data members

        /// <summary>
        /// Holds the base virtualAddress of the link result.
        /// </summary>
        private long baseAddress;

        /// <summary>
        /// Holds the entry point of the compiled binary.
        /// </summary>
        private LinkerSymbol entryPoint;

        /// <summary>
        /// Holds all unresolved link requests.
        /// </summary>
        private Dictionary<string, List<LinkRequest>> _linkRequests;

        /// <summary>
        /// Holds the output file of the linker.
        /// </summary>
        private string outputFile;

        /// <summary>
        /// A dictionary containing all symbol seen in the assembly.
        /// </summary>
        private Dictionary<string, LinkerSymbol> symbols;

        #endregion // Data members

        #region Construction

        /// <summary>
        /// Initializes a new instance of <see cref="AssemblyLinkerStageBase"/>.
        /// </summary>
        protected AssemblyLinkerStageBase()
        {
            this.baseAddress = 0x00400000; // Use the Win32 default for now, FIXME
            _linkRequests = new Dictionary<string, List<LinkRequest>>();
            this.symbols = new Dictionary<string, LinkerSymbol>();
        }

        #endregion // Construction

        #region IAssemblyCompilerStage Members

        /// <summary>
        /// Retrieves the name of the compilation stage.
        /// </summary>
        /// <value>The name of the compilation stage.</value>
        public abstract string Name { get; }

        /// <summary>
        /// Performs stage specific processing on the compiler context.
        /// </summary>
        /// <param name="compiler">The compiler context to perform processing in.</param>
        public virtual void Run(AssemblyCompiler compiler)
        {
            long address;

            // Check if we have unresolved requests and try to link them
            List<string> members = new List<string>(_linkRequests.Keys);
            foreach (string member in members)
            {
                // Is the runtime member resolved?
                if (true == IsResolved(member, out address))
                {
                    // Yes, patch up the method
                    List<LinkRequest> link = _linkRequests[member];
                    PatchRequests(address, link);
                    _linkRequests.Remove(member);
                }
            }

            Debug.Assert(0 == _linkRequests.Count, @"AssemblyLinker has found unresolved symbols.");
            if (0 != _linkRequests.Count)
                throw new LinkerException(@"Unresolved symbols.");
        }

        #endregion // IAssemblyCompilerStage Members

        #region Methods

        /// <summary>
        /// A request to patch already emitted code by storing the calculated virtualAddress value.
        /// </summary>
        /// <param name="linkType">Type of the link.</param>
        /// <param name="methodAddress">The virtual virtualAddress of the method whose code is being patched.</param>
        /// <param name="methodOffset">The value to store at the position in code.</param>
        /// <param name="methodRelativeBase">The method relative base.</param>
        /// <param name="targetAddress">The position in code, where it should be patched.</param>
        protected abstract void ApplyPatch(LinkType linkType, long methodAddress, long methodOffset, long methodRelativeBase, long targetAddress);

        #endregion // Methods

        #region IAssemblyLinker Members

        /// <summary>
        /// Gets the base virtualAddress.
        /// </summary>
        /// <value>The base virtualAddress.</value>
        public long BaseAddress
        {
            get { return this.baseAddress; }
        }

        /// <summary>
        /// Gets the entry point symbol.
        /// </summary>
        /// <value>The entry point symbol.</value>
        public LinkerSymbol EntryPoint
        {
            get { return this.entryPoint; }
            set { this.entryPoint = value; }
        }

        /// <summary>
        /// Gets the load alignment of sections.
        /// </summary>
        /// <value>The load alignment.</value>
        public abstract long LoadSectionAlignment
        {
            get;
        }

        /// <summary>
        /// Gets or sets the output file of the linker.
        /// </summary>
        /// <value>The output file.</value>
        public string OutputFile
        {
            get { return this.outputFile; }
            set
            {
                this.outputFile = value;
            }
        }

        /// <summary>
        /// Gets the linker time stamp.
        /// </summary>
        /// <value>The time stamp.</value>
        public DateTime TimeStamp
        {
            get { return DateTime.Now; }
        }

        /// <summary>
        /// Retrieves the collection of sections created during compilation.
        /// </summary>
        /// <value>The sections collection.</value>
        public abstract ICollection<Linker.LinkerSection> Sections
        {
            get;
        }

        /// <summary>
        /// Retrieves the collection of symbols known by the linker.
        /// </summary>
        /// <value>The symbol collection.</value>
        public ICollection<LinkerSymbol> Symbols
        {
            get { return this.symbols.Values; }
        }

        /// <summary>
        /// Gets the virtual alignment of sections.
        /// </summary>
        /// <value>The virtual section alignment.</value>
        public abstract long VirtualSectionAlignment
        {
            get;
        }

        /// <summary>
        /// Allocates memory in the specified section.
        /// </summary>
        /// <param name="member">The metadata member to allocate space for.</param>
        /// <param name="section">The executable section to allocate from.</param>
        /// <param name="size">The number of bytes to allocate. If zero, indicates an unknown amount of memory is required.</param>
        /// <param name="alignment">The alignment. A value of zero indicates the use of a default alignment for the section.</param>
        /// <returns>A stream, which can be used to populate the section.</returns>
        public virtual Stream Allocate(RuntimeMember member, SectionKind section, int size, int alignment)
        {
            // Create a canonical symbol name
            string name = CreateSymbolName(member);
            
            // Create a stream
            return Allocate(name, section, size, alignment);
        }

        /// <summary>
        /// Allocates a symbol of the given name in the specified section.
        /// </summary>
        /// <param name="name">The name of the symbol.</param>
        /// <param name="section">The executable section to allocate from.</param>
        /// <param name="size">The number of bytes to allocate. If zero, indicates an unknown amount of memory is required.</param>
        /// <param name="alignment">The alignment. A value of zero indicates the use of a default alignment for the section.</param>
        /// <returns>
        /// A stream, which can be used to populate the section.
        /// </returns>
        public virtual Stream Allocate(string name, SectionKind section, int size, int alignment)
        {
            Stream stream = Allocate(section, size, alignment);

            try
            {
                // Create a linker symbol for the name
                LinkerSymbol symbol = new LinkerSymbol(name, section, stream.Position);

                // Save the symbol for later use
                this.symbols.Add(symbol.Name, symbol);

                // Wrap the stream to catch premature disposal
                stream = new LinkerStream(symbol, stream, size);
            }
            catch (ArgumentException argx)
            {
                throw new LinkerException(String.Format(@"Symbol {0} defined multiple times.", name), argx);
            }

            return stream;
        }

        /// <summary>
        /// Allocates a symbol of the given name in the specified section.
        /// </summary>
        /// <param name="section">The executable section to allocate from.</param>
        /// <param name="size">The number of bytes to allocate. If zero, indicates an unknown amount of memory is required.</param>
        /// <param name="alignment">The alignment. A value of zero indicates the use of a default alignment for the section.</param>
        /// <returns>
        /// A stream, which can be used to populate the section.
        /// </returns>
        protected abstract Stream Allocate(SectionKind section, int size, int alignment);

        /// <summary>
        /// Gets the section.
        /// </summary>
        /// <param name="sectionKind">Kind of the section.</param>
        /// <returns>The section of the requested kind.</returns>
        public abstract LinkerSection GetSection(SectionKind sectionKind);

        /// <summary>
        /// Retrieves a linker symbol.
        /// </summary>
        /// <param name="member">The runtime member to retrieve a symbol for.</param>
        /// <returns>
        /// A linker symbol, which represents the runtime member.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="member"/> is null.</exception>
        /// <exception cref="System.ArgumentException">There's no symbol of the given name.</exception>
        public LinkerSymbol GetSymbol(RuntimeMember member)
        {
            if (member == null)
                throw new ArgumentNullException(@"member");

            string symbolName = CreateSymbolName(member);
            return GetSymbol(symbolName);
        }

        /// <summary>
        /// Retrieves a linker symbol.
        /// </summary>
        /// <param name="symbolName">The name of the symbol to retrieve.</param>
        /// <returns>The named linker symbol.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="symbolName"/> is null.</exception>
        /// <exception cref="System.ArgumentException">There's no symbol of the given name.</exception>
        public LinkerSymbol GetSymbol(string symbolName)
        {
            if (symbolName == null)
                throw new ArgumentNullException(@"symbolName");

            LinkerSymbol result;
            if (false == this.symbols.TryGetValue(symbolName, out result))
                throw new ArgumentException(@"Symbol not compiled.", @"member");

            return result;
        }

        /// <summary>
        /// Issues a linker request for the given runtime method.
        /// </summary>
        /// <param name="linkType">The type of link required.</param>
        /// <param name="method">The method the patched code belongs to.</param>
        /// <param name="methodOffset">The offset inside the method where the patch is placed.</param>
        /// <param name="methodRelativeBase">The base virtualAddress, if a relative link is required.</param>
        /// <param name="target">The method or static field to link against.</param>
        public virtual long Link(LinkType linkType, RuntimeMethod method, int methodOffset, int methodRelativeBase, RuntimeMember target)
        {
            Debug.Assert(true == IsValid(target), @"Invalid RuntimeMember passed to IAssemblyLinker.Link");
            if (false == IsValid(target))
                throw new ArgumentException(@"RuntimeMember is not a static field or method.", @"member");

            long address;
            if (false == IsResolved(target, out address))
            {
                address = Link(linkType, method, methodOffset, methodRelativeBase, CreateSymbolName(target));
            }

            return address;
        }

        /// <summary>
        /// Issues a linker request for the given runtime method.
        /// </summary>
        /// <param name="linkType">The type of link required.</param>
        /// <param name="method">The method the patched code belongs to.</param>
        /// <param name="methodOffset">The offset inside the method where the patch is placed.</param>
        /// <param name="methodRelativeBase">The base virtualAddress, if a relative link is required.</param>
        /// <param name="symbol">The linker symbol to link against.</param>
        /// <returns>
        /// The return value is the preliminary virtualAddress to place in the generated machine
        /// code. On 32-bit systems, only the lower 32 bits are valid. The above are not used. An implementation of
        /// IAssemblyLinker may not rely on 64-bits being stored in the memory defined by position.
        /// </returns>
        public virtual long Link(LinkType linkType, RuntimeMethod method, int methodOffset, int methodRelativeBase, string symbol)
        {
            Debug.Assert(null != symbol, @"Symbol can't be null.");
            if (null == symbol)
                throw new ArgumentNullException(@"symbol");

            long result = 0;
            if (true == IsResolved(symbol, out result))
            {
                List<LinkRequest> patchList;
                if (false == _linkRequests.TryGetValue(symbol, out patchList))
                {
                    patchList = new List<LinkRequest>(1);
                    patchList.Add(new LinkRequest(linkType, CreateSymbolName(method), methodOffset, methodRelativeBase, symbol));
                }

                PatchRequests(result, patchList);
            }
            else
            {
                // FIXME: Make this thread safe
                List<LinkRequest> list;
                if (false == _linkRequests.TryGetValue(symbol, out list))
                {
                    list = new List<LinkRequest>();
                    _linkRequests.Add(symbol, list);
                }

                list.Add(new LinkRequest(linkType, CreateSymbolName(method), methodOffset, methodRelativeBase, symbol));
            }

            return result;
        }

        /// <summary>
        /// Issues a linker request for the given runtime method.
        /// </summary>
        /// <param name="linkType">The type of link required.</param>
        /// <param name="symbolName">The method the patched code belongs to.</param>
        /// <param name="methodOffset">The offset inside the method where the patch is placed.</param>
        /// <param name="methodRelativeBase">The base virtualAddress, if a relative link is required.</param>
        /// <param name="targetSymbol">The linker symbol to link against.</param>
        /// <returns>
        /// The return value is the preliminary virtualAddress to place in the generated machine
        /// code. On 32-bit systems, only the lower 32 bits are valid. The above are not used. An implementation of
        /// IAssemblyLinker may not rely on 64-bits being stored in the memory defined by position.
        /// </returns>
        public virtual long Link(LinkType linkType, string symbolName, int methodOffset, int methodRelativeBase, string targetSymbol)
        {
            Debug.Assert(null != symbolName, @"Symbol can't be null.");
            if (null == symbolName)
                throw new ArgumentNullException(@"symbol");

            long result = 0;
            if (true == IsResolved(symbolName, out result))
            {
                List<LinkRequest> patchList;
                if (false == _linkRequests.TryGetValue(targetSymbol, out patchList))
                {
                    patchList = new List<LinkRequest>(1);
                    patchList.Add(new LinkRequest(linkType, symbolName, methodOffset, methodRelativeBase, targetSymbol));
                }

                PatchRequests(result, patchList);
            }
            else
            {
                // FIXME: Make this thread safe
                List<LinkRequest> list;
                if (false == _linkRequests.TryGetValue(targetSymbol, out list))
                {
                    list = new List<LinkRequest>();
                    _linkRequests.Add(targetSymbol, list);
                }

                list.Add(new LinkRequest(linkType, symbolName, methodOffset, methodRelativeBase, targetSymbol));
            }

            return result;
        }

        #endregion // IAssemblyLinker Members

        #region Internals

        /// <summary>
        /// Creates a symbol name.
        /// </summary>
        /// <param name="symbol">The symbol name.</param>
        /// <returns>A string, which represents the symbol name.</returns>
        protected string CreateSymbolName(RuntimeMember symbol)
        {
            if (symbol == null)
                throw new ArgumentNullException(@"symbol");

            string name;
            RuntimeType declaringType = symbol.DeclaringType;
            if (declaringType != null)
            {
                name = String.Format("{0}.{1}", declaringType.FullName, symbol.Name);
            }
            else
            {
                name = symbol.Name;
            }

            return name;
        }

        /// <summary>
        /// Determines whether the specified symbol is resolved.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="virtualAddress">The virtualAddress.</param>
        /// <returns>
        /// 	<c>true</c> if the specified symbol is resolved; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsResolved(string symbol, out long virtualAddress)
        {
            virtualAddress = 0;
            LinkerSymbol linkerSymbol;
            if (true == this.symbols.TryGetValue(symbol, out linkerSymbol))
            {
                virtualAddress = linkerSymbol.VirtualAddress.ToInt64();
            }
            return (0 != virtualAddress);
        }

        /// <summary>
        /// Determines if the given runtime member can be resolved immediately.
        /// </summary>
        /// <param name="member">The runtime member to determine resolution of.</param>
        /// <param name="virtualAddress">Receives the determined virtualAddress of the runtime member.</param>
        /// <returns>
        /// The method returns true, when it was successfully resolved.
        /// </returns>
        protected bool IsResolved(RuntimeMember member, out long virtualAddress)
        {
            // Is this a method?
            RuntimeMethod method = member as RuntimeMethod;
            if (null != method && method.ImplAttributes == MethodImplAttributes.InternalCall)
            {
                virtualAddress = ResolveInternalCall(method);
                return (0 != virtualAddress);
            }

            return IsResolved(CreateSymbolName(member), out virtualAddress);
        }

        /// <summary>
        /// Checks that <paramref name="member"/> is a member, which can be linked.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>
        /// True, if the member is valid for linking.
        /// </returns>
        protected bool IsValid(RuntimeMember member)
        {
            return (member is RuntimeMethod || (member is RuntimeField && FieldAttributes.Static == (FieldAttributes.Static & ((RuntimeField)member).Attributes)));
        }

        /// <summary>
        /// Special resolution for internal calls.
        /// </summary>
        /// <param name="method">The internal call method to resolve.</param>
        /// <returns>The virtualAddress </returns>
        protected virtual long ResolveInternalCall(RuntimeMethod method)
        {
            long address = 0;
            ITypeSystem ts = RuntimeBase.Instance.TypeLoader;
            RuntimeMethod internalImpl = ts.GetImplementationForInternalCall(method);
            if (null != internalImpl)
                address = internalImpl.Address.ToInt64();
            return address;
        }

        /// <summary>
        /// Patches all requests in the given link request list.
        /// </summary>
        /// <param name="virtualAddress">The virtualAddress of the member.</param>
        /// <param name="requests">A list of requests to patch.</param>
        private void PatchRequests(long virtualAddress, IEnumerable<LinkRequest> requests)
        {
            long methodAddress;

            foreach (LinkRequest request in requests)
            {
                if (IsResolved(request.LinkSymbol, out methodAddress) == false)
                    throw new InvalidOperationException(@"Method not compiled - but making link requests??");

                // Patch the code stream
                ApplyPatch(
                    request.LinkType,
                    methodAddress,
                    request.MethodOffset,
                    request.MethodRelativeBase,
                    virtualAddress
                );
            }
        }

        #endregion // Internals
    }
}
