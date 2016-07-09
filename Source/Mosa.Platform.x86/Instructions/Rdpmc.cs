﻿// Copyright (c) MOSA Project. Licensed under the New BSD License.

namespace Mosa.Platform.x86.Instructions
{
	/// <summary>
	/// Representations the x86 rdpmc instruction.
	/// </summary>
	public sealed class Rdpmc : X86Instruction
	{
		#region Construction

		/// <summary>
		/// Initializes a new instance of <see cref="Rdtsc"/>.
		/// </summary>
		public Rdpmc() :
			base(1, 0)
		{
		}

		#endregion Construction
	}
}
