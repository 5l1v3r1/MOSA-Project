﻿/*
 * (c) 2008 MOSA - The Managed Operating System Alliance
 *
 * Licensed under the terms of the New BSD License.
 *
 * Authors:
 *  Phil Garcia (tgiphil) <phil@thinkedge.com>
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Mosa.Runtime.CompilerFramework
{
	/// <summary>
	/// Interface to a platform instruction
	/// </summary>
	public interface IPlatformInstruction : IInstruction
	{
		/// <summary>
		/// Emits the specified platform instruction.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="emitter">The emitter.</param>
		void Emit(Context context, ICodeEmitter emitter);

		/// <summary>
		/// Gets the instruction latency.
		/// </summary>
		/// <value>The latency.</value>
		int Latency { get; }
	}
}
