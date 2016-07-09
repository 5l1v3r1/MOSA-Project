// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Common;
using Mosa.Compiler.Framework;
using Mosa.Compiler.Framework.Stages;
using Mosa.Compiler.Linker.Elf;
using Mosa.Compiler.MosaTypeSystem;
using Mosa.Platform.x86.Stages;
using System.Diagnostics;

namespace Mosa.Platform.x86
{
	/// <summary>
	/// This class provides a common base class for architecture
	/// specific operations.
	/// </summary>
	public class Architecture : BaseArchitecture
	{
		/// <summary>
		/// Gets the endianness of the target architecture.
		/// </summary>
		/// <value>
		/// The endianness.
		/// </value>
		public override Endianness Endianness { get { return Endianness.Little; } }

		/// <summary>
		/// Gets the type of the elf machine.
		/// </summary>
		/// <value>
		/// The type of the elf machine.
		/// </value>
		public override MachineType MachineType { get { return MachineType.Intel386; } }

		/// <summary>
		/// Defines the register set of the target architecture.
		/// </summary>
		private static readonly Register[] registers = new Register[]
		{
			////////////////////////////////////////////////////////
			// 32-bit general purpose registers
			////////////////////////////////////////////////////////
			GeneralPurposeRegister.EAX,
			GeneralPurposeRegister.ECX,
			GeneralPurposeRegister.EDX,
			GeneralPurposeRegister.EBX,
			GeneralPurposeRegister.ESP,
			GeneralPurposeRegister.EBP,
			GeneralPurposeRegister.ESI,
			GeneralPurposeRegister.EDI,

			////////////////////////////////////////////////////////
			// SSE 128-bit floating point registers
			////////////////////////////////////////////////////////
			SSE2Register.XMM0,
			SSE2Register.XMM1,
			SSE2Register.XMM2,
			SSE2Register.XMM3,
			SSE2Register.XMM4,
			SSE2Register.XMM5,
			SSE2Register.XMM6,
			SSE2Register.XMM7,

			////////////////////////////////////////////////////////
			// Segmentation Registers
			////////////////////////////////////////////////////////
			//SegmentRegister.CS,
			//SegmentRegister.DS,
			//SegmentRegister.ES,
			//SegmentRegister.FS,
			//SegmentRegister.GS,
			//SegmentRegister.SS
		};

		/// <summary>
		/// Specifies the architecture features to use in generated code.
		/// </summary>
		private ArchitectureFeatureFlags architectureFeatures;

		/// <summary>
		/// Initializes a new instance of the <see cref="Architecture"/> class.
		/// </summary>
		/// <param name="architectureFeatures">The features this architecture supports.</param>
		private Architecture(ArchitectureFeatureFlags architectureFeatures)
		{
			this.architectureFeatures = architectureFeatures;
			CallingConvention = new DefaultCallingConvention(this);
		}

		/// <summary>
		/// Retrieves the native integer size of the x86 platform.
		/// </summary>
		/// <value>This property always returns 32.</value>
		public override int NativeIntegerSize
		{
			get { return 32; }
		}

		/// <summary>
		/// Gets the native alignment of the architecture in bytes.
		/// </summary>
		/// <value>This property always returns 4.</value>
		public override int NativeAlignment
		{
			get { return 4; }
		}

		/// <summary>
		/// Gets the native size of architecture in bytes.
		/// </summary>
		/// <value>This property always returns 4.</value>
		public override int NativePointerSize
		{
			get { return 4; }
		}

		/// <summary>
		/// Retrieves the register set of the x86 platform.
		/// </summary>
		public override Register[] RegisterSet
		{
			get { return registers; }
		}

		/// <summary>
		/// Retrieves the stack frame register of the x86.
		/// </summary>
		public override Register StackFrameRegister
		{
			get { return GeneralPurposeRegister.EBP; }
		}

		/// <summary>
		/// Retrieves the stack pointer register of the x86.
		/// </summary>
		public override Register StackPointerRegister
		{
			get { return GeneralPurposeRegister.ESP; }
		}

		/// <summary>
		/// Retrieves the exception register of the architecture.
		/// </summary>
		public override Register ExceptionRegister
		{
			get { return GeneralPurposeRegister.EDI; }
		}

		/// <summary>
		/// Gets the finally return block register.
		/// </summary>
		public override Register LeaveTargetRegister
		{
			get { return GeneralPurposeRegister.ESI; }
		}

		/// <summary>
		/// Retrieves the program counter register of the x86.
		/// </summary>
		public override Register ProgramCounter
		{
			get { return null; }
		}

		/// <summary>
		/// Gets the name of the platform.
		/// </summary>
		/// <value>
		/// The name of the platform.
		/// </value>
		public override string PlatformName { get { return "x86"; } }

		/// <summary>
		/// Factory method for the Architecture class.
		/// </summary>
		/// <returns>The created architecture instance.</returns>
		/// <param name="architectureFeatures">The features available in the architecture and code generation.</param>
		/// <remarks>
		/// This method creates an instance of an appropriate architecture class, which supports the specific
		/// architecture features.
		/// </remarks>
		public static BaseArchitecture CreateArchitecture(ArchitectureFeatureFlags architectureFeatures)
		{
			if (architectureFeatures == ArchitectureFeatureFlags.AutoDetect)
				architectureFeatures = ArchitectureFeatureFlags.MMX | ArchitectureFeatureFlags.SSE | ArchitectureFeatureFlags.SSE2 | ArchitectureFeatureFlags.SSE3 | ArchitectureFeatureFlags.SSE4;

			return new Architecture(architectureFeatures);
		}

		/// <summary>
		/// Extends the pre-compiler pipeline with x86 compiler stages.
		/// </summary>
		/// <param name="compilerPipeline">The pipeline to extend.</param>
		public override void ExtendPreCompilerPipeline(CompilerPipeline compilerPipeline)
		{
			compilerPipeline.InsertAfterFirst<ICompilerStage>(
				new InterruptVectorStage()
			);

			compilerPipeline.InsertAfterLast<ICompilerStage>(
				new SSESetupStage()
			);
		}

		/// <summary>
		/// Extends the post-compiler pipeline with x86 compiler stages.
		/// </summary>
		/// <param name="compilerPipeline">The pipeline to extend.</param>
		public override void ExtendPostCompilerPipeline(CompilerPipeline compilerPipeline)
		{
		}

		/// <summary>
		/// Extends the method compiler pipeline with x86 specific stages.
		/// </summary>
		/// <param name="methodCompilerPipeline">The method compiler pipeline to extend.</param>
		public override void ExtendMethodCompilerPipeline(CompilerPipeline methodCompilerPipeline)
		{
			methodCompilerPipeline.InsertAfterLast<PlatformStubStage>(
				new IMethodCompilerStage[]
				{
					//new CheckOperandCountStage(),
					new PlatformIntrinsicStage(),
					new LongOperandTransformationStage(),
					new IRTransformationStage(),
					new TweakTransformationStage(),

					new FixedRegisterAssignmentStage(),
					new SimpleDeadCodeRemovalStage(),
					new AddressModeConversionStage(),
					new FloatingPointStage(),
				});

			methodCompilerPipeline.InsertAfterLast<StackLayoutStage>(
				new BuildStackStage()
			);

			methodCompilerPipeline.InsertBefore<CodeGenerationStage>(
				new FinalTweakTransformationStage()
			);

			methodCompilerPipeline.InsertBefore<CodeGenerationStage>(
				new JumpOptimizationStage()
			);

			methodCompilerPipeline.InsertBefore<CodeGenerationStage>(
				new FixMovStage()
			);

			methodCompilerPipeline.InsertBefore<CodeGenerationStage>(
				new StopStage()
			);
		}

		/// <summary>
		/// Gets the type memory requirements.
		/// </summary>
		/// <param name="typeLayout">The type layouts.</param>
		/// <param name="type">The type.</param>
		/// <param name="size">Receives the memory size of the type.</param>
		/// <param name="alignment">Receives alignment requirements of the type.</param>
		public override void GetTypeRequirements(MosaTypeLayout typeLayout, MosaType type, out int size, out int alignment)
		{
			alignment = 4;

			size = type.IsValueType ? typeLayout.GetTypeSize(type) : 4;
		}

		/// <summary>
		/// Gets the code emitter.
		/// </summary>
		/// <returns></returns>
		public override BaseCodeEmitter GetCodeEmitter()
		{
			return new MachineCodeEmitter();
		}

		/// <summary>
		/// Create platform move.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="source">The source.</param>
		public override void InsertMoveInstruction(Context context, Operand destination, Operand source)
		{
			var instruction = BaseTransformationStage.GetMove(destination, source);
			context.AppendInstruction(instruction, /*size,*/ destination, source);
		}

		public override void InsertStoreInstruction(Context context, Operand destination, Operand offset, Operand value)
		{
			BaseInstruction instruction = X86.MovStore;
			InstructionSize size = InstructionSize.Size32;

			if (destination.Type.IsR4)
			{
				instruction = X86.MovssStore;
			}
			else
			if (destination.Type.IsR8)
			{
				instruction = X86.MovsdStore;
				size = InstructionSize.Size64;
			}

			context.AppendInstruction(instruction, size, null, destination, offset, value);
		}

		/// <summary>
		/// Create platform compound move.
		/// </summary>
		/// <param name="compiler">The compiler.</param>
		/// <param name="context">The context.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="destinationOffset">The destination offset.</param>
		/// <param name="source">The source.</param>
		/// <param name="sourceOffset">The source offset.</param>
		/// <param name="size">The size.</param>
		public override void InsertCompoundMoveInstruction(BaseMethodCompiler compiler, Context context, Operand destination, int destinationOffset, Operand source, int sourceOffset, int size)
		{
			Debug.Assert(size > 0);

			const int LargeAlignment = 16;
			int alignedSize = size - (size % NativeAlignment);
			int largeAlignedTypeSize = size - (size % LargeAlignment);

			var srcReg = compiler.CreateVirtualRegister(destination.Type.TypeSystem.BuiltIn.I4);
			var dstReg = compiler.CreateVirtualRegister(destination.Type.TypeSystem.BuiltIn.I4);
			var tmp = compiler.CreateVirtualRegister(destination.Type.TypeSystem.BuiltIn.I4);
			var tmpLarge = Operand.CreateCPURegister(destination.Type.TypeSystem.BuiltIn.Void, SSE2Register.XMM1);

			var destinationOffsetOperand = Operand.CreateConstant(compiler.TypeSystem, destinationOffset);
			var sourceOffsetOperand = Operand.CreateConstant(compiler.TypeSystem, sourceOffset);

			context.AppendInstruction(X86.Lea, srcReg, destination, destinationOffsetOperand);
			context.AppendInstruction(X86.Lea, dstReg, source, sourceOffsetOperand);

			for (int i = 0; i < largeAlignedTypeSize; i += LargeAlignment)
			{
				// Large aligned moves allow 128bits to be copied at a time
				var index = Operand.CreateConstant(destination.Type.TypeSystem.BuiltIn.I4, i);
				context.AppendInstruction(X86.MovupsLoad, InstructionSize.Size128, tmpLarge, srcReg, index);
				context.AppendInstruction(X86.MovupsStore, InstructionSize.Size128, null, dstReg, index, tmpLarge);
			}
			for (int i = largeAlignedTypeSize; i < alignedSize; i += NativeAlignment)
			{
				var index = Operand.CreateConstant(destination.Type.TypeSystem.BuiltIn.I4, i);
				context.AppendInstruction(X86.MovLoad, InstructionSize.Size32, tmp, srcReg, index);
				context.AppendInstruction(X86.MovStore, InstructionSize.Size32, null, dstReg, index, tmp);
			}
			for (int i = alignedSize; i < size; i++)
			{
				var index = Operand.CreateConstant(destination.Type.TypeSystem.BuiltIn.I4, i);
				context.AppendInstruction(X86.MovLoad, InstructionSize.Size8, tmp, srcReg, index);
				context.AppendInstruction(X86.MovStore, InstructionSize.Size8, null, dstReg, index, tmp);
			}
		}

		/// <summary>
		/// Creates the swap.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="source">The source.</param>
		public override void InsertExchangeInstruction(Context context, Operand destination, Operand source)
		{
			if (source.IsR4)
			{
				// TODO
				throw new CompilerException("R4 not implemented in InsertExchangeInstruction method");
			}
			else if (source.IsR8)
			{
				// TODO
				throw new CompilerException("R8 not implemented in InsertExchangeInstruction method");
			}
			else
			{
				context.AppendInstruction2(X86.Xchg, destination, source, source, destination);
			}
		}

		/// <summary>
		/// Inserts the jump instruction.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="destination">The destination.</param>
		/// <param name="source">The source.</param>
		public override void InsertJumpInstruction(Context context, Operand destination)
		{
			context.AppendInstruction(X86.Jmp, destination);
		}

		/// <summary>
		/// Inserts the jump instruction.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="Destination">The destination.</param>
		public override void InsertJumpInstruction(Context context, BasicBlock destination)
		{
			context.AppendInstruction(X86.Jmp, destination);
		}

		/// <summary>
		/// Inserts the call instruction.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="source">The source.</param>
		public override void InsertCallInstruction(Context context, Operand source)
		{
			context.AppendInstruction(X86.Call, null, source);
		}

		/// <summary>
		/// Inserts the add instruction.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="Destination">The destination.</param>
		/// <param name="Source">The source.</param>
		public override void InsertAddInstruction(Context context, Operand destination, Operand source1, Operand source2)
		{
			Debug.Assert(source1 == destination);
			context.AppendInstruction(X86.Add, destination, source1, source2);
		}

		/// <summary>
		/// Inserts the sub instruction.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="Destination">The destination.</param>
		/// <param name="Source">The source.</param>
		public override void InsertSubInstruction(Context context, Operand destination, Operand source1, Operand source2)
		{
			Debug.Assert(source1 == destination);
			context.AppendInstruction(X86.Sub, destination, source1, source2);
		}

		/// <summary>
		/// Determines whether [is instruction move] [the specified instruction].
		/// </summary>
		/// <param name="instruction">The instruction.</param>
		/// <returns></returns>
		public override bool IsInstructionMove(BaseInstruction instruction)
		{
			return (instruction == X86.Mov || instruction == X86.Movsd || instruction == X86.Movss);
		}
	}
}
