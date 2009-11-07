﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Michael Ruck (grover) <sharpos@michaelruck.de>
 */

using System;
using System.IO;
using System.Text;

using Mosa.Runtime.CompilerFramework;
using Mosa.Runtime.Linker;

using NDesk.Options;

namespace Mosa.Tools.Compiler
{
    /// <summary>
    /// Wraps the map file generation stage and adds options to configure it.
    /// </summary>
	public sealed class MapFileGeneratorWrapper : IAssemblyCompilerStage, IHasOptions, IPipelineStage
    {
        #region Data Members

        /// <summary>
        /// Holds the name of the map file to generate.
        /// </summary>
        private string mapFile;

        #endregion // Data Members

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="MapFileGenerationStage"/> class.
        /// </summary>
        public MapFileGeneratorWrapper()
        {
        }

        #endregion // Construction

		#region IPipelineStage members

		string IPipelineStage.Name { get { return @"Map File Generator Wrapper"; } }

		/// <summary>
		/// Gets the pipeline stage order.
		/// </summary>
		/// <value>The pipeline stage order.</value>
		PipelineStageOrder[] IPipelineStage.PipelineStageOrder { get { return null; } }

		#endregion // IPipelineStage members

        #region IAssemblyCompilerStage Members

		/// <summary>
        /// Performs stage specific processing on the compiler context.
        /// </summary>
        /// <param name="compiler">The compiler context to perform processing in.</param>
        public void Run(AssemblyCompiler compiler)
        {
            if (null != this.mapFile)
            {
                try
                {
                    using (FileStream fs = new FileStream(this.mapFile, FileMode.Create, FileAccess.Write, FileShare.Read))
                    using (StreamWriter writer = new StreamWriter(fs, Encoding.UTF8))
                    {
                        MapFileGenerationStage mapGenerator = new MapFileGenerationStage(writer);
                        mapGenerator.Run(compiler);
                    }
                }
                catch (Exception x)
                {
                    Console.WriteLine(@"Failed to generate map file.");
                    Console.WriteLine(x);
                }
            }
        }

        #endregion // IAssemblyCompilerStage Members

        #region IHasOptions Members

        /// <summary>
        /// Adds the additional options for the parsing process to the given OptionSet.
        /// </summary>
        /// <param name="optionSet">A given OptionSet to add the options to.</param>
        public void AddOptions(OptionSet optionSet)
        {
            optionSet.Add(
                "map=",
                "Generate a map {file} of the produced binary.",
                delegate(string file)
                {
                    this.mapFile = file;
                }
               );
        }

        #endregion // IHasOptions Members
    }
}
