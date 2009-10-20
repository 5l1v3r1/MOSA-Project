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
using System.Diagnostics;

namespace Mosa.Runtime.CompilerFramework
{
	/// <summary>
	/// This method compiler stage performs constant propagation, e.g. it removes
	/// local variables in favor of constants.
	/// </summary>
	/// <remarks>
	/// Constant propagation has a couple of advantages: First of all it removes
	/// a local variable From the stack and secondly it reduces the register pressure
	/// on systems with only a small number of registers (x86).
	/// <para/>
	/// It is only safe to use this stage on an instruction stream in SSA form.
	/// </remarks>
	public sealed class ConstantPropagationStage : BaseStage, IMethodCompilerStage
	{
		#region IMethodCompilerStage Members

		/// <summary>
		/// Retrieves the name of the compilation stage.
		/// </summary>
		/// <value>The name of the compilation stage.</value>
		public string Name
		{
			get { return @"Constant Propagation"; }
		}

		/// <summary>
		/// Performs stage specific processing on the compiler context.
		/// </summary>
		/// <param name="compiler">The compiler context to perform processing in.</param>
		public override void Run(IMethodCompiler compiler)
		{
			base.Run(compiler);

			bool remove = false;

			foreach (BasicBlock block in BasicBlocks) {
				for (Context ctx = new Context(InstructionSet, block); !ctx.EndOfInstruction; ctx.GotoNext()) {
					if (ctx.Instruction is IR.MoveInstruction || ctx.Instruction is CIL.StlocInstruction) {
						if (ctx.Operand1 is ConstantOperand) {
							// HACK: We can't track a constant through a register, so we keep those moves
							if (ctx.Result is StackOperand) {
								Debug.Assert(ctx.Result.Definitions.Count == 1, @"Operand defined multiple times. Instruction stream not in SSA form!");
								ctx.Result.Replace(ctx.Operand1, InstructionSet);
								remove = true;
							}
						}
					}
					else if (ctx.Instruction is IR.PhiInstruction) {
						IR.PhiInstruction phi = (IR.PhiInstruction)ctx.Instruction;
						ConstantOperand co = ctx.Operand2 as ConstantOperand;
						List<BasicBlock> blocks = ctx.Other as List<BasicBlock>;	// FIXME PG / ctx has moved
						if (co != null && blocks.Count == 1) {
							// We can remove the phi, as it is only defined once
							// HACK: We can't track a constant through a register, so we keep those moves
							if (!ctx.Result.IsRegister) {
								Debug.Assert(ctx.Result.Definitions.Count == 1, @"Operand defined multiple times. Instruction stream not in SSA form!");
								ctx.Result.Replace(co, InstructionSet);
								remove = true;
							}
						}
					}

					// Shall we remove this instruction?
					if (remove) {
						ctx.Remove();
						remove = false;
					}

				}
			}
		}

		/// <summary>
		/// Adds the stage to the pipeline.
		/// </summary>
		/// <param name="pipeline">The pipeline to add to.</param>
		public void AddToPipeline(CompilerPipeline<IMethodCompilerStage> pipeline)
		{
			pipeline.InsertAfter<EnterSSA>(this);
		}

		#endregion // IMethodCompilerStage Members
	}
}
