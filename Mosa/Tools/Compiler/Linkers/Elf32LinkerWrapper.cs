﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License
 * with restrictions to the license beneath, concering
 * the use of the CommandLine assembly.
 *
 * Authors:
 *  Michael Ruck (<mailto:sharpos@michaelruck.de>)
 */

using Mosa.Runtime.Linker.Elf;

using NDesk.Options;

namespace Mosa.Tools.Compiler.Linkers
{
    /// <summary>
    /// Wraps the ELF32 linker in the MOSA runtime and adds various command line options to it.
    /// </summary>
    public sealed class Elf32LinkerWrapper : AssemblyCompilerStageWrapper<Elf32Linker>
    {
        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="Elf32LinkerWrapper"/> class.
        /// </summary>
        public Elf32LinkerWrapper()
        {
        }

        #endregion // Construction

        #region AssemblyCompilerStageWrapper Overrides

        /// <summary>
        /// Adds the additional options for the parsing process to the given OptionSet.
        /// </summary>
        /// <param name="optionSet">A given OptionSet to add the options to.</param>
        public override void AddOptions(OptionSet optionSet)
        {
            // FIXME: Add ELF32 specific command line options here.
        }

        #endregion // AssemblyCompilerStageWrapper Overrides
    }
}
