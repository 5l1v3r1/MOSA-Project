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
using System.Diagnostics;
using System.IO;

using Mosa.Runtime.CompilerFramework;
using Mosa.Runtime.Linker;
using Mosa.Runtime.Metadata;
using Mosa.Runtime.Metadata.Signatures;
using Mosa.Runtime.Vm;
using IR = Mosa.Runtime.CompilerFramework.IR;

namespace Mosa.Platforms.x86
{
	/// <summary>
	/// x86 OpCode
	/// </summary>
	public struct OpCode
	{
		/// <summary>
		/// Byte code
		/// </summary>
		public byte[] Code;

		/// <summary>
		/// Register field to extend the operation
		/// </summary>
		public byte? RegField;

		/// <summary>
		/// Initializes a new instance of the <see cref="OpCode"/> struct.
		/// </summary>
		/// <param name="code">The corresponding opcodes</param>
		/// <param name="regField">Additonal parameter field</param>
		public OpCode(byte[] code, byte? regField)
		{
			this.Code = code;
			this.RegField = regField;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="OpCode"/> struct.
		/// </summary>
		/// <param name="code">The corresponding opcodes</param>
		public OpCode(byte[] code)
		{
			this.Code = code;
			this.RegField = null;
		}
	}
}
