﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.x86.Intrinsic
{
	/// <summary>
	/// Calculates the remainder of a floating-point division. Only works with valid data.
	/// </summary>
	internal sealed class Remainder : IIntrinsicPlatformMethod
	{
		#region Methods

		/// <summary>
		/// Replaces the intrinsic call site
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="methodCompiler">The method compiler.</param>
		void IIntrinsicPlatformMethod.ReplaceIntrinsicCall(Context context, BaseMethodCompiler methodCompiler)
		{
			var result = context.Result;
			var dividend = context.Operand1;
			var divisor = context.Operand2;

			if (result.IsR8)
			{
				var xmm1 = methodCompiler.CreateVirtualRegister(methodCompiler.TypeSystem.BuiltIn.R8);
				var xmm2 = methodCompiler.CreateVirtualRegister(methodCompiler.TypeSystem.BuiltIn.R8);
				var xmm3 = methodCompiler.CreateVirtualRegister(methodCompiler.TypeSystem.BuiltIn.R8);
				const InstructionSize size = InstructionSize.Size64;

				context.SetInstruction(X86.Divsd, size, xmm1, dividend, divisor);
				context.AppendInstruction(X86.Roundsd, size, xmm2, xmm1, Operand.CreateConstant(methodCompiler.TypeSystem.BuiltIn.U1, 0x3));
				context.AppendInstruction(X86.Mulsd, size, xmm3, divisor, xmm2);
				context.AppendInstruction(X86.SubSD, size, result, dividend, xmm3);
			}
			else
			{
				var xmm1 = methodCompiler.CreateVirtualRegister(methodCompiler.TypeSystem.BuiltIn.R4);
				var xmm2 = methodCompiler.CreateVirtualRegister(methodCompiler.TypeSystem.BuiltIn.R4);
				var xmm3 = methodCompiler.CreateVirtualRegister(methodCompiler.TypeSystem.BuiltIn.R4);
				const InstructionSize size = InstructionSize.Size32;

				context.SetInstruction(X86.Divss, size, xmm1, dividend, divisor);
				context.AppendInstruction(X86.Roundss, size, xmm2, xmm1, Operand.CreateConstant(methodCompiler.TypeSystem.BuiltIn.U1, 0x3));
				context.AppendInstruction(X86.Mulss, size, xmm3, divisor, xmm2);
				context.AppendInstruction(X86.SubSS, size, result, dividend, xmm3);
			}
		}

		#endregion Methods
	}
}
