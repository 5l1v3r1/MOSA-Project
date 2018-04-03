﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Intrinsic
{
	/// <summary>
	/// Representations the x86 CPUID instruction.
	/// </summary>
	internal sealed class CpuIdEcx : IIntrinsicPlatformMethod
	{
		void IIntrinsicPlatformMethod.ReplaceIntrinsicCall(Context context, MethodCompiler methodCompiler)
		{
			Operand result = context.Result;
			Operand operand = context.Operand1;
			Operand eax = Operand.CreateCPURegister(methodCompiler.TypeSystem.BuiltIn.I4, GeneralPurposeRegister.EAX);
			Operand ecx = Operand.CreateCPURegister(methodCompiler.TypeSystem.BuiltIn.I4, GeneralPurposeRegister.ECX);
			Operand reg = Operand.CreateCPURegister(methodCompiler.TypeSystem.BuiltIn.I4, GeneralPurposeRegister.ECX);
			context.SetInstruction(X86.Mov32, eax, operand);
			context.AppendInstruction(X86.MovConst32, ecx, methodCompiler.ConstantZero);
			context.AppendInstruction(X86.CpuId, eax, eax);
			context.AppendInstruction(X86.Mov32, result, reg);
		}
	}
}
