/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using System;
using System.Diagnostics;
using System.IO;

using Mosa.Runtime.CompilerFramework;
using Mosa.Runtime.Linker;
using Mosa.Runtime.Metadata;
using Mosa.Runtime.Metadata.Signatures;
using Mosa.Runtime.Vm;

using CIL = Mosa.Runtime.CompilerFramework.CIL;
using IR = Mosa.Runtime.CompilerFramework.IR;

namespace Mosa.Platforms.x86
{
	/// <summary>
	/// Transforms CIL instructions into their appropriate 
	/// </summary>
	/// <remarks>
	/// This transformation stage transforms CIL instructions into their equivalent X86 sequences.
	/// </remarks>
	public sealed class CILTransformationStage :
		BaseTransformationStage,
		CIL.ICILVisitor,
		IMethodCompilerStage,
		IPlatformTransformationStage
	{
		#region IMethodCompilerStage Members

		/// <summary>
		/// Retrieves the name of the compilation stage.
		/// </summary>
		/// <value>The name of the compilation stage.</value>
		public override string Name
		{
			get { return @"X86.CILTransformationStage"; }
		}

		/// <summary>
		/// Adds this stage to the given pipeline.
		/// </summary>
		/// <param name="pipeline">The pipeline to add this stage to.</param>
		public override void AddToPipeline(CompilerPipeline<IMethodCompilerStage> pipeline)
		{
			pipeline.InsertAfter<LongOperandTransformationStage>(this);
		}

		#endregion // IMethodCompilerStage Members

		#region ICILVisitor

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Break"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Break(Context ctx)
		{
			ctx.SetInstruction(CPUx86.Instruction.BreakInstruction);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldarga"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldarga(Context ctx)
		{
			Operand result = ctx.Result;
			ctx.SetInstruction(CPUx86.Instruction.MovInstruction, result, new RegisterOperand(new SigType(CilElementType.Ptr), GeneralPurposeRegister.EBP));
			ctx.InsertInstructionAfter(CPUx86.Instruction.AddInstruction, result, new ConstantOperand(new SigType(CilElementType.Ptr), ctx.Label));
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldloca"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldloca(Context ctx)
		{
			Operand result = ctx.Result;
			ctx.SetInstruction(CPUx86.Instruction.MovInstruction, result, new RegisterOperand(ctx.Result.Type, GeneralPurposeRegister.EBP));
			ctx.InsertInstructionAfter(CPUx86.Instruction.AddInstruction, result, new ConstantOperand(ctx.Result.Type, ctx.Label));
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Call"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Call(Context ctx)
		{
			HandleInvokeInstruction(ctx.Clone());

			return;

			// FIXME PG

			// Move the this pointer to the right place, if this is an object instance
			//RuntimeMethod method = ctx.InvokeTarget;
			//if (method.Signature.HasThis) {
			//    // FIXME PG - 
			//    //_codeEmitter.Mov(new RegisterOperand(new SigType(Mosa.Runtime.Metadata.CilElementType.Object), GeneralPurposeRegister.ECX), instruction.ThisReference);
			//    //ctx.SetInstruction(CPUx86.Instruction.MoveInstruction, new RegisterOperand(new SigType(Mosa.Runtime.Metadata.CilElementType.Object), GeneralPurposeRegister.ECX), instruction.ThisReference);

			//    throw new NotImplementedException();
			//}

			/*
			 * HINT: Microsoft seems not to use vtables/itables in .NET v2/v3/v3.5 anymore. They allocate
			 * trampolines for virtual calls and rewrite them without indirect lookups if the object type
			 * changes. This way they don't perform indirect lookups and the performance is drastically
			 * improved.
			 * 
			 */

			// Do we need to emit a call with vtable lookup?
			//Debug.Assert(MethodAttributes.Virtual != (MethodAttributes.Virtual & method.Attributes), @"call to a virtual function?");

			// A static call to the right address :)

			//_codeEmitter.Call(method); // FIXME PG

			// This is what is in Call method above
			//_codeStream.WriteByte(0xE8);
			//_codeStream.Write(new byte[4], 0, 4);
			//long address = _linker.Link(
			//    LinkType.RelativeOffset | LinkType.I4,
			//    _compiler.Method,
			//    (int)(_codeStream.Position - _codeStreamBasePosition) - 4,
			//    (int)(_codeStream.Position - _codeStreamBasePosition),
			//    target,
			//    IntPtr.Zero
			//);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ret"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ret(Context ctx)
		{
			bool eax = false;

			if (ctx.OperandCount != 0 && ctx.Operand1 != null) {
				Operand retval = ctx.Operand1;
				if (retval.IsRegister) {
					// Do not move, if return value is already in EAX
					RegisterOperand rop = (RegisterOperand)retval;
					if (System.Object.ReferenceEquals(rop.Register, GeneralPurposeRegister.EAX))
						eax = true;
				}

				if (!eax)
					ctx.SetInstruction(CPUx86.Instruction.MovInstruction, new RegisterOperand(new SigType(CilElementType.I), GeneralPurposeRegister.EAX), retval);
			}
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Branch"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Branch(Context ctx)
		{
			ctx.ReplaceInstructionOnly(CPUx86.Instruction.JmpInstruction);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.UnaryBranch"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.UnaryBranch(Context ctx)
		{
			IBranch branch = ctx.Branch;
			CIL.OpCode opcode = (ctx.Instruction as CIL.ICILInstruction).OpCode;

			SigType I4 = new SigType(CilElementType.I4);
			ctx.SetInstruction(CPUx86.Instruction.CmpInstruction, new RegisterOperand(I4, GeneralPurposeRegister.EAX), new ConstantOperand(I4, 0));

			if (opcode == CIL.OpCode.Brtrue || opcode == CIL.OpCode.Brtrue_s) {
				ctx.InsertInstructionAfter(CPUx86.Instruction.JneInstruction);
				ctx.SetBranch(branch.Targets[0]);
				ctx.InsertInstructionAfter(CPUx86.Instruction.JeInstruction);
				ctx.SetBranch(branch.Targets[1]);
			}
			else {
				ctx.InsertInstructionAfter(CPUx86.Instruction.JneInstruction);
				ctx.SetBranch(branch.Targets[1]);
				ctx.InsertInstructionAfter(CPUx86.Instruction.JeInstruction);
				ctx.SetBranch(branch.Targets[0]);
			}

		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.BinaryBranch"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.BinaryBranch(Context ctx)
		{
			// FIXME PG

			//bool swap = ctx.Operand1 is ConstantOperand;
			//int[] targets = ctx.Branch.Targets;
			//if (swap) {
			//    int tmp = targets[0];
			//    targets[0] = targets[1];
			//    targets[1] = tmp;

			//    _codeEmitter.Cmp(ctx.Operand2, ctx.Operand1);

			//    CIL.OpCode opcode = ((ctx.Instruction) as CIL.ICILInstruction).OpCode;

			//    switch (opcode) {
			//        // Signed
			//        case CIL.OpCode.Beq_s: _codeEmitter.Jne(targets[0]); break;
			//        case CIL.OpCode.Bge_s: _codeEmitter.Jl(targets[0]); break;
			//        case CIL.OpCode.Bgt_s: _codeEmitter.Jle(targets[0]); break;
			//        case CIL.OpCode.Ble_s: _codeEmitter.Jg(targets[0]); break;
			//        case CIL.OpCode.Blt_s: _codeEmitter.Jge(targets[0]); break;

			//        // Unsigned
			//        case CIL.OpCode.Bne_un_s: _codeEmitter.Je(targets[0]); break;
			//        case CIL.OpCode.Bge_un_s: _codeEmitter.Jb(targets[0]); break;
			//        case CIL.OpCode.Bgt_un_s: _codeEmitter.Jbe(targets[0]); break;
			//        case CIL.OpCode.Ble_un_s: _codeEmitter.Ja(targets[0]); break;
			//        case CIL.OpCode.Blt_un_s: _codeEmitter.Jae(targets[0]); break;

			//        // Long form signed
			//        case CIL.OpCode.Beq: goto case CIL.OpCode.Beq_s;
			//        case CIL.OpCode.Bge: goto case CIL.OpCode.Bge_s;
			//        case CIL.OpCode.Bgt: goto case CIL.OpCode.Bgt_s;
			//        case CIL.OpCode.Ble: goto case CIL.OpCode.Ble_s;
			//        case CIL.OpCode.Blt: goto case CIL.OpCode.Blt_s;

			//        // Long form unsigned
			//        case CIL.OpCode.Bne_un: goto case CIL.OpCode.Bne_un_s;
			//        case CIL.OpCode.Bge_un: goto case CIL.OpCode.Bge_un_s;
			//        case CIL.OpCode.Bgt_un: goto case CIL.OpCode.Bgt_un_s;
			//        case CIL.OpCode.Ble_un: goto case CIL.OpCode.Ble_un_s;
			//        case CIL.OpCode.Blt_un: goto case CIL.OpCode.Blt_un_s;

			//        default:
			//            throw new NotImplementedException();
			//    }
			//}
			//else {
			//    _codeEmitter.Cmp(ctx.Operand1, ctx.Operand2);
			//    CIL.OpCode opcode = ((ctx.Instruction) as CIL.ICILInstruction).OpCode;

			//    switch (opcode) {
			//        // Signed
			//        case CIL.OpCode.Beq_s: _codeEmitter.Je(targets[0]); break;
			//        case CIL.OpCode.Bge_s: _codeEmitter.Jge(targets[0]); break;
			//        case CIL.OpCode.Bgt_s: _codeEmitter.Jg(targets[0]); break;
			//        case CIL.OpCode.Ble_s: _codeEmitter.Jle(targets[0]); break;
			//        case CIL.OpCode.Blt_s: _codeEmitter.Jl(targets[0]); break;

			//        // Unsigned
			//        case CIL.OpCode.Bne_un_s: _codeEmitter.Jne(targets[0]); break;
			//        case CIL.OpCode.Bge_un_s: _codeEmitter.Jae(targets[0]); break;
			//        case CIL.OpCode.Bgt_un_s: _codeEmitter.Ja(targets[0]); break;
			//        case CIL.OpCode.Ble_un_s: _codeEmitter.Jbe(targets[0]); break;
			//        case CIL.OpCode.Blt_un_s: _codeEmitter.Jb(targets[0]); break;

			//        // Long form signed
			//        case CIL.OpCode.Beq: goto case CIL.OpCode.Beq_s;
			//        case CIL.OpCode.Bge: goto case CIL.OpCode.Bge_s;
			//        case CIL.OpCode.Bgt: goto case CIL.OpCode.Bgt_s;
			//        case CIL.OpCode.Ble: goto case CIL.OpCode.Ble_s;
			//        case CIL.OpCode.Blt: goto case CIL.OpCode.Blt_s;

			//        // Long form unsigned
			//        case CIL.OpCode.Bne_un: goto case CIL.OpCode.Bne_un_s;
			//        case CIL.OpCode.Bge_un: goto case CIL.OpCode.Bge_un_s;
			//        case CIL.OpCode.Bgt_un: goto case CIL.OpCode.Bgt_un_s;
			//        case CIL.OpCode.Ble_un: goto case CIL.OpCode.Ble_un_s;
			//        case CIL.OpCode.Blt_un: goto case CIL.OpCode.Blt_un_s;

			//        default:
			//            throw new NotImplementedException();
			//    }
			//}

			//// Emit a regular jump for the second case
			//_codeEmitter.Jmp(targets[1]);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Switch"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Switch(Context ctx)
		{
			// FIXME PG

			//for (int i = 0; i < ctx.Branch.Targets.Length - 1; i++) {
			//    _codeEmitter.Cmp(ctx.Operand1, new ConstantOperand(new SigType(CilElementType.I), i));
			//    _codeEmitter.Je(ctx.Branch.Targets[i]);
			//}
			//_codeEmitter.Jmp(ctx.Branch.Targets[ctx.Branch.Targets.Length - 1]);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Calli"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Calli(Context ctx)
		{
			HandleInvokeInstruction(ctx);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Callvirt"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Callvirt(Context ctx)
		{
			HandleInvokeInstruction(ctx);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.BinaryComparison"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.BinaryComparison(Context ctx)
		{
			throw new NotSupportedException();
			//HandleComparisonInstruction(ctx, instruction);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Add"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Add(Context ctx)
		{
			if (ctx.Operand1.StackType == StackTypeCode.F || ctx.Operand2.StackType == StackTypeCode.F)
				HandleCommutativeOperation(ctx, CPUx86.Instruction.SseAddInstruction);
			else
				HandleCommutativeOperation(ctx, CPUx86.Instruction.AddInstruction);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Sub"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Sub(Context ctx)
		{
			if (ctx.Operand1.StackType == StackTypeCode.F || ctx.Operand2.StackType == StackTypeCode.F)
				HandleNonCommutativeOperation(ctx, CPUx86.Instruction.SseSubInstruction);
			else
				HandleNonCommutativeOperation(ctx, CPUx86.Instruction.SubInstruction);

		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Mul"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Mul(Context ctx)
		{
			if (ctx.Operand1.StackType == StackTypeCode.F)
				HandleCommutativeOperation(ctx, CPUx86.Instruction.SseMulInstruction);
			else
				HandleCommutativeOperation(ctx, CPUx86.Instruction.MulInstruction);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Div"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Div(Context ctx)
		{
			if (IsUnsigned(ctx.Operand1) || IsUnsigned(ctx.Operand2))
				HandleCommutativeOperation(ctx, IR.Instruction.UDivInstruction);
			else if (ctx.Operand1.StackType == StackTypeCode.F)
				HandleCommutativeOperation(ctx, CPUx86.Instruction.SseDivInstruction);
			else
				HandleCommutativeOperation(ctx, CPUx86.Instruction.DivInstruction);
		}

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Rem"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Rem(Context ctx)
		{
			ctx.InsertInstructionAfter(CPUx86.Instruction.MovInstruction, new RegisterOperand(ctx.Operand1.Type, GeneralPurposeRegister.EAX), ctx.Operand1);

			if (IsUnsigned(ctx.Operand1))
				ctx.SetInstruction(IR.Instruction.ZeroExtendedMoveInstruction, new RegisterOperand(ctx.Operand1.Type, GeneralPurposeRegister.EAX), new RegisterOperand(ctx.Operand1.Type, GeneralPurposeRegister.EAX));
			else
				ctx.SetInstruction(IR.Instruction.SignExtendedMoveInstruction, new RegisterOperand(ctx.Operand1.Type, GeneralPurposeRegister.EAX), new RegisterOperand(ctx.Operand1.Type, GeneralPurposeRegister.EAX));

			if (IsUnsigned(ctx.Operand1) && IsUnsigned(ctx.Operand2))
				ctx.InsertInstructionAfter(IR.Instruction.UDivInstruction, ctx.Operand1, ctx.Operand2);
			else
				ctx.InsertInstructionAfter(CPUx86.Instruction.DivInstruction, ctx.Operand1, ctx.Operand2);

			ctx.InsertInstructionAfter(CPUx86.Instruction.MovInstruction, ctx.Result, new RegisterOperand(ctx.Operand1.Type, GeneralPurposeRegister.EDX));
		}

		#endregion // Members

		#region ICILVisitor - Unused

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldarg"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldarg(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldloc"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldloc(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldc"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldc(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldobj"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldobj(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldstr"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldstr(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldfld"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldfld(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldflda"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldflda(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldsfld"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldsfld(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldsflda"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldsflda(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldftn"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldftn(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldvirtftn"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldvirtftn(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldtoken"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldtoken(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Stloc"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Stloc(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Starg"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Starg(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Stobj"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Stobj(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Stfld"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Stfld(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Stsfld"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Stsfld(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Dup"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Dup(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Pop"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Pop(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Jmp"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Jmp(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.BinaryLogic"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.BinaryLogic(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Shift"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Shift(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Neg"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Neg(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Not"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Not(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Conversion"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Conversion(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Cpobj"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Cpobj(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Newobj"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Newobj(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Castclass"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Castclass(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Isinst"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Isinst(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Unbox"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Unbox(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Throw"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Throw(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Box"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Box(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Newarr"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Newarr(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldlen"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldlen(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldelema"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldelema(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Ldelem"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Ldelem(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Stelem"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Stelem(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.UnboxAny"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.UnboxAny(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Refanyval"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Refanyval(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.UnaryArithmetic"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.UnaryArithmetic(Context ctx) { }

		/// <summary>
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Mkrefany(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.ArithmeticOverflow"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.ArithmeticOverflow(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Endfinally"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Endfinally(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Leave"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Leave(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Arglist"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Arglist(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Localalloc"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Localalloc(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Endfilter"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Endfilter(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.InitObj"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.InitObj(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Cpblk"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Cpblk(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Initblk"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Initblk(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Prefix"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Prefix(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Rethrow"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Rethrow(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Sizeof"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Sizeof(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Refanytype"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Refanytype(Context ctx) { }

		/// <summary>
		/// Visitation function for <see cref="CIL.ICILVisitor.Nop"/>.
		/// </summary>
		/// <param name="ctx">The context.</param>
		void CIL.ICILVisitor.Nop(Context ctx) { }

		#endregion // ICILVisitor - Unused

		#region Internals

		/// <summary>
		/// Special handling for commutative operations.
		/// </summary>
		/// <param name="ctx">The transformation context.</param>
		/// <param name="instruction">The instruction.</param>
		/// <remarks>
		/// Commutative operations are reordered by moving the constant to the second operand,
		/// which allows the instruction selection in the code generator to use a instruction
		/// format with an immediate operand.
		/// </remarks>
		private void HandleCommutativeOperation(Context ctx, IInstruction instruction)
		{
			EmitOperandConstants(ctx);

			// If the first operand is a constant, move it to the second operand
			if (ctx.Operand1 is ConstantOperand) {
				// Yes, swap the operands...
				Operand t = ctx.Operand1;
				ctx.Operand1 = ctx.Operand2;
				ctx.Operand2 = t;
			}

			// In order for mul to work out, the first operand must be equal to the destination operand -
			// if it is not (e.g. c = a + b) then transform it to c = a, c = c + b.
			ThreeTwoAddressConversion(ctx, instruction);
		}

		/// <summary>
		/// Handles the non commutative operation.
		/// </summary>
		/// <param name="ctx">The context.</param>
		/// <param name="instruction">The instruction.</param>
		private void HandleNonCommutativeOperation(Context ctx, IInstruction instruction)
		{
			EmitResultConstants(ctx);
			EmitOperandConstants(ctx);

			// In order for mul to work out, the first operand must be equal to the destination operand -
			// if it is not (e.g. c = a + b) then transform it to c = a, c = c + b.
			ThreeTwoAddressConversion(ctx, instruction);
		}

		/// <summary>
		/// Special handling for shift operations, which require the shift amount in the ECX or as a constant register.
		/// </summary>
		/// <param name="ctx">The transformation context.</param>
		private void HandleShiftOperation(Context ctx)
		{
			HandleShiftOperation(ctx, ctx.Instruction);
		}

		/// <summary>
		/// Special handling for shift operations, which require the shift amount in the ECX or as a constant register.
		/// </summary>
		/// <param name="ctx">The transformation context.</param>
		/// <param name="instruction">The instruction to transform.</param>
		private void HandleShiftOperation(Context ctx, IInstruction instruction)
		{
			Operand opRes = ctx.Result;
			Operand op1 = ctx.Operand1;
			Operand op2 = ctx.Operand2;
			EmitOperandConstants(ctx);

			RegisterOperand ecx = new RegisterOperand(op2.Type, GeneralPurposeRegister.ECX);

			ctx.SetInstruction(CPUx86.Instruction.MovInstruction, ecx, op2);
			ctx.InsertInstructionAfter(CPUx86.Instruction.MovInstruction, opRes, op1);
			ctx.InsertInstructionAfter(instruction, opRes, ecx);


			// FIXME
			// Commented part causes an access violation!
			/*
			if (ops[1] is ConstantOperand)
			{
				Replace(ctx, new Instruction[] {
					Architecture.CreateInstruction(typeof(Instructions.MoveInstruction), opRes, ops[0]),
					Architecture.CreateInstruction(replacementType, opRes, ops[1])
				});
			}
			else
			{*/

			//if (ops[0].Type.Type == CilElementType.Char) {
			//    RegisterOperand ecx = new RegisterOperand(ops[1].Type, GeneralPurposeRegister.ECX);
			//    Replace(ctx, new LegacyInstruction[] {
			//        Architecture.CreateInstruction(typeof(Instructions.MoveInstruction), ecx, ops[1]),
			//        Architecture.CreateInstruction(typeof(Instructions.MoveInstruction), opRes, ops[0]),
			//        Architecture.CreateInstruction(replacementType, opRes, ecx),
			//    });
			//}
			//else {
			//    RegisterOperand ecx = new RegisterOperand(ops[1].Type, GeneralPurposeRegister.ECX);
			//    Replace(ctx, new LegacyInstruction[] {
			//        Architecture.CreateInstruction(typeof(Instructions.MoveInstruction), ecx, ops[1]),
			//        Architecture.CreateInstruction(typeof(Instructions.MoveInstruction), opRes, ops[0]),
			//        Architecture.CreateInstruction(replacementType, opRes, ecx),
			//    });
			//}
		}

		/// <summary>
		/// Processes a method call instruction.
		/// </summary>
		/// <param name="ctx">The transformation context.</param>
		private void HandleInvokeInstruction(Context ctx)
		{
			ICallingConvention cc = Architecture.GetCallingConvention(ctx.InvokeTarget.Signature.CallingConvention);
			Debug.Assert(null != cc, @"Failed to retrieve the calling convention.");
			cc.Expand(ctx);
		}

		/// <summary>
		/// Handles the comparison instruction.
		/// </summary>
		/// <param name="ctx">The context.</param>
		private void HandleComparisonInstruction(Context ctx)
		{
			Operand op1 = ctx.Operand1;
			Operand op2 = ctx.Operand2;
			EmitOperandConstants(ctx);

			if (op1 is MemoryOperand && op2 is RegisterOperand) {
				SwapComparisonOperands(ctx, op1, op2);
			}
			else if (op1 is MemoryOperand && op2 is MemoryOperand) {
				RegisterOperand eax = new RegisterOperand(op1.Type, GeneralPurposeRegister.EAX);

				ctx.InsertBefore().SetInstruction(CPUx86.Instruction.MovInstruction, eax, op1);
				ctx.Operand1 = eax;
			}

			ThreeTwoAddressConversion(ctx, null);
		}

		/// <summary>
		/// Swaps the comparison operands.
		/// </summary>
		/// <param name="ctx">The context.</param>
		/// <param name="op1">The op1.</param>
		/// <param name="op2">The op2.</param>
		private static void SwapComparisonOperands(Context ctx, Operand op1, Operand op2)
		{
			// Swap the operands
			ctx.Operand1 = op2;
			ctx.Operand2 = op1;

			// Negate the condition code if necessary...
			switch (ctx.ConditionCode) {
				case IR.ConditionCode.Equal:
					break;

				case IR.ConditionCode.GreaterOrEqual:
					ctx.ConditionCode = IR.ConditionCode.LessThan;
					break;

				case IR.ConditionCode.GreaterThan:
					ctx.ConditionCode = IR.ConditionCode.LessOrEqual;
					break;

				case IR.ConditionCode.LessOrEqual:
					ctx.ConditionCode = IR.ConditionCode.GreaterThan;
					break;

				case IR.ConditionCode.LessThan:
					ctx.ConditionCode = IR.ConditionCode.GreaterOrEqual;
					break;

				case IR.ConditionCode.NotEqual:
					break;

				case IR.ConditionCode.UnsignedGreaterOrEqual:
					ctx.ConditionCode = IR.ConditionCode.UnsignedLessThan;
					break;

				case IR.ConditionCode.UnsignedGreaterThan:
					ctx.ConditionCode = IR.ConditionCode.UnsignedLessOrEqual;
					break;

				case IR.ConditionCode.UnsignedLessOrEqual:
					ctx.ConditionCode = IR.ConditionCode.UnsignedGreaterThan;
					break;

				case IR.ConditionCode.UnsignedLessThan:
					ctx.ConditionCode = IR.ConditionCode.UnsignedGreaterOrEqual;
					break;
			}
		}

		/// <summary>
		/// Converts the given instruction from two address format to a one address format.
		/// </summary>
		/// <param name="ctx">The conversion context.</param>
		private void TwoOneAddressConversion(Context ctx)
		{
			Operand opRes = ctx.Result;
			RegisterOperand eax = new RegisterOperand(opRes.Type, GeneralPurposeRegister.EAX);
			ctx.Operand1 = EmitConstant(ctx.Operand1);
			ctx.Result = eax;

			ctx.InsertBefore().SetInstruction(IR.Instruction.MoveInstruction, eax, ctx.Operand2);
			ctx.InsertInstructionAfter(IR.Instruction.MoveInstruction, opRes, eax);

		}
		/// <summary>
		/// Converts the given instruction from three address format to a two address format.
		/// </summary>
		/// <param name="ctx">The conversion context.</param>
		private static void ThreeTwoAddressConversion(Context ctx)
		{
			ThreeTwoAddressConversion(ctx, ctx.Instruction);
		}

		/// <summary>
		/// Converts the given instruction from three address format to a two address format.
		/// </summary>
		/// <param name="ctx">The conversion context.</param>
		/// <param name="instruction">The instruction.</param>
		private static void ThreeTwoAddressConversion(Context ctx, IInstruction instruction)
		{
			Operand opRes = ctx.Result;
			Operand op1 = ctx.Operand1;
			Operand op2 = ctx.Operand2;

			// Create registers for different data types
			RegisterOperand eax = new RegisterOperand(opRes.Type, opRes.StackType == StackTypeCode.F ? (Register)SSE2Register.XMM0 : GeneralPurposeRegister.EAX);
			RegisterOperand eaxL = new RegisterOperand(op1.Type, GeneralPurposeRegister.EAX);

			if (instruction != null)
				ctx.SetInstruction(instruction, eax, op2);
			else {
				ctx.Result = eax;
				ctx.Operand1 = eax;
			}

			// Check if we have to sign-extend the operand that's being loaded
			if (IsSigned(op1) && !(op1 is ConstantOperand)) {
				// Sign extend it
				ctx.InsertBefore().SetInstruction(IR.Instruction.SignExtendedMoveInstruction, eaxL, op1);
			}
			// Check if the operand has to be zero-extended
			else if (IsUnsigned(op1) && !(op1 is ConstantOperand) && op1.StackType != StackTypeCode.F) {
				ctx.InsertBefore().SetInstruction(IR.Instruction.ZeroExtendedMoveInstruction, eaxL, op1);
			}
			// In any other case: Just load it
			else
				ctx.InsertBefore().SetInstruction(IR.Instruction.MoveInstruction, eax, op1);

			ctx.InsertInstructionAfter(IR.Instruction.MoveInstruction, opRes, eax);
		}

		#endregion // Internals
	}
}
