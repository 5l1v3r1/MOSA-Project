// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework;

namespace Mosa.Platform.ARMv6.Instructions
{
	/// <summary>
	/// Mul instruction:
	/// Performs a 32x32 multiply that generates a 32-bit result.The instruction can operate on signed or unsigned quantities.
	/// </summary>
	public class Mul : ARMv6Instruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Mul"/>.
		/// </summary>
		public Mul() :
			base(1, 3)
		{
		}

		#endregion Construction

		#region Methods

		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <param name="emitter">The emitter.</param>
		protected override void Emit(InstructionNode node, ARMv6CodeEmitter emitter)
		{
			EmitMultiplyInstruction(node, emitter);
		}

		#endregion Methods
	}
}
