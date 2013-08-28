﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Simon Wollwage (rootnode) <rootnode@mosa-project.org>
 */

using Mosa.Compiler.Framework.IR;
using Mosa.Compiler.Framework.Platform;
using System;
using System.Diagnostics;
using System.IO;

namespace Mosa.Compiler.Framework.Stages
{
	/// <summary>
	/// Base class for code generation stages.
	/// </summary>
	public class CodeGenerationStage : BaseMethodCompilerStage, IMethodCompilerStage, IPipelineStage
	{
		#region Data members

		/// <summary>
		/// Holds the stream, where code is emitted to.
		/// </summary>
		protected static Stream codeStream;

		/// <summary>
		///
		/// </summary>
		protected ICodeEmitter codeEmitter;

		#endregion Data members

		#region Properties

		public ICodeEmitter CodeEmitter { get { return codeEmitter; } }

		public bool EmitBinary { get; set; }

		#endregion Properties

		#region Construction

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeGenerationStage"/> class.
		/// </summary>
		public CodeGenerationStage()
		{
			EmitBinary = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CodeGenerationStage" /> class.
		/// </summary>
		/// <param name="emitBinary">if set to <c>true</c> [emit binary].</param>
		public CodeGenerationStage(bool emitBinary)
		{
			EmitBinary = emitBinary;
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Performs stage specific processing on the compiler context.
		/// </summary>
		void IMethodCompilerStage.Run()
		{
			if (!EmitBinary)
				return;

			// Retrieve a stream to place the code into
			using (codeStream = methodCompiler.RequestCodeStream())
			{
				// HINT: We need seeking to resolve labels.
				Debug.Assert(codeStream.CanSeek, @"Can't seek code output stream.");
				Debug.Assert(codeStream.CanWrite, @"Can't write to code output stream.");

				if (!codeStream.CanSeek || !codeStream.CanWrite)
					throw new NotSupportedException(@"Code stream doesn't support seeking or writing.");

				// Emit method prologue
				BeginGenerate();

				// Emit all instructions
				EmitInstructions();

				// Emit the method epilogue
				EndGenerate();
			}
		}

		/// <summary>
		/// Called to emit a list of instructions offered by the instruction provider.
		/// </summary>
		protected virtual void EmitInstructions()
		{
			var trace = CreateTrace();

			foreach (BasicBlock block in basicBlocks)
			{
				BlockStart(block);

				for (Context context = new Context(instructionSet, block); !context.IsBlockEndInstruction; context.GotoNext())
				{
					if (context.IsEmpty || context.Instruction == IRInstruction.BlockStart)
						continue;

					BasePlatformInstruction instruction = context.Instruction as BasePlatformInstruction;

					if (instruction != null)
					{
						instruction.Emit(context, codeEmitter);
					}
					else
					{
						if (architecture.PlatformName != "Null")
						{
							trace.Log(InternalTrace.CompilerEvent.Error, "Missing Code Transformation: " + context.ToString());
						}
					}
				}

				BlockEnd(block);
			}
		}

		/// <summary>
		/// Begins the generate.
		/// </summary>
		protected virtual void BeginGenerate()
		{
			codeEmitter = architecture.GetCodeEmitter();
			codeEmitter.Initialize(methodCompiler.Method.FullName, methodCompiler.Linker, codeStream);
		}

		/// <summary>
		/// Start of code generation for a block.
		/// </summary>
		/// <param name="block">The started block.</param>
		protected virtual void BlockStart(BasicBlock block)
		{
			codeEmitter.Label(block.Label);
		}

		/// <summary>
		/// Completion of code generation for a block.
		/// </summary>
		/// <param name="block">The completed block.</param>
		protected virtual void BlockEnd(BasicBlock block)
		{
			// TODO: Adjust ICodeEmitter interface to mark the end of label sections, rather than create this special label:
			codeEmitter.Label(block.Label + 0x0F000000);
		}

		/// <summary>
		/// Code generation completed.
		/// </summary>
		protected virtual void EndGenerate()
		{
			codeEmitter.ResolvePatches();
			codeEmitter.Dispose();
		}

		#endregion Methods
	}
}