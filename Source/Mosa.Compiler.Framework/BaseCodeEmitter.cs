// Copyright (c) MOSA Project. Licensed under the New BSD License.

using Mosa.Compiler.Framework.Platform;
using Mosa.Compiler.Linker;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Mosa.Compiler.Framework
{
	/// <summary>
	/// Base code emitter.
	/// </summary>
	public abstract class BaseCodeEmitter
	{
		#region Types

		/// <summary>
		/// Patch
		/// </summary>
		protected struct Patch
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="Patch"/> struct.
			/// </summary>
			/// <param name="label">The label.</param>
			/// <param name="position">The position.</param>
			public Patch(int label, long position)
			{
				Label = label;
				Position = position;
			}

			/// <summary>
			/// Patch label
			/// </summary>
			public int Label;

			/// <summary>
			/// The patch's position in the stream
			/// </summary>
			public long Position;

			/// <summary>
			/// Returns a <see cref="System.String"/> that represents this instance.
			/// </summary>
			/// <returns>
			/// A <see cref="System.String"/> that represents this instance.
			/// </returns>
			public override string ToString()
			{
				return "[@" + Position.ToString() + " -> " + Label.ToString() + "]";
			}
		}

		#endregion Types

		#region Data members

		/// <summary>
		/// The stream used to write machine code bytes to.
		/// </summary>
		protected Stream codeStream;

		/// <summary>
		/// Holds the linker used to resolve externals.
		/// </summary>
		protected BaseLinker linker;

		/// <summary>
		/// List of labels that were emitted.
		/// </summary>
		private readonly Dictionary<int, int> labels = new Dictionary<int, int>();

		/// <summary>
		/// Patches we need to perform.
		/// </summary>
		protected readonly List<Patch> patches = new List<Patch>();

		#endregion Data members

		#region Properties

		/// <summary>
		/// Gets the name of the method.
		/// </summary>
		protected string MethodName { get; private set; }

		#endregion Properties

		#region BaseCodeEmitter Members

		/// <summary>
		/// Initializes a new instance of <see cref="BaseCodeEmitter" />.
		/// </summary>
		/// <param name="methodName">Name of the method.</param>
		/// <param name="linker">The linker.</param>
		/// <param name="codeStream">The stream the machine code is written to.</param>
		public void Initialize(string methodName, BaseLinker linker, Stream codeStream)
		{
			Debug.Assert(codeStream != null);
			Debug.Assert(linker != null);

			MethodName = methodName;
			this.linker = linker;
			this.codeStream = codeStream;

			// only necessary if method is being recompiled (due to inline optimization, for example)
			var symbol = linker.GetSymbol(MethodName, SectionKind.Text);
			symbol.RemovePatches();
		}

		/// <summary>
		/// Emits a label into the code stream.
		/// </summary>
		/// <param name="label">The label name to emit.</param>
		public void Label(int label)
		{
			/*
			 * Labels are used to resolve branches inside a procedure. Branches outside
			 * of procedures are handled differently, t.b.d.
			 *
			 * So we store the current instruction offset with the label info to be able to
			 * resolve jumps to this location.
			 *
			 */

			Debug.Assert(!labels.ContainsKey(label));

			// Add this label to the label list, so we can resolve the jump later on
			labels.Add(label, (int)codeStream.Position);

			//Debug.WriteLine("LABEL: " + label.ToString() + " @" + codeStream.Position.ToString());
		}

		/// <summary>
		/// Gets the position.
		/// </summary>
		/// <param name="label">The label.</param>
		/// <returns></returns>
		public int GetPosition(int label)
		{
			return labels[label];
		}

		/// <summary>
		/// Gets the current position.
		/// </summary>
		/// <value>The current position.</value>
		public int CurrentPosition { get { return (int)codeStream.Position; } }

		#endregion BaseCodeEmitter Members

		#region Code Generation Members

		/// <summary>
		/// Writes the byte.
		/// </summary>
		/// <param name="data">The data.</param>
		public void WriteByte(byte data)
		{
			codeStream.WriteByte(data);
		}

		/// <summary>
		/// Writes the byte.
		/// </summary>
		/// <param name="buffer">The buffer.</param>
		/// <param name="offset">The offset.</param>
		/// <param name="count">The count.</param>
		public void Write(byte[] buffer, int offset, int count)
		{
			codeStream.Write(buffer, offset, count);
		}

		#endregion Code Generation Members

		#region New Code Generation Methods

		/// <summary>
		/// Emits the specified opcode.
		/// </summary>
		/// <param name="opcode">The opcode.</param>
		public void Emit(OpcodeEncoder opcode)
		{
			opcode.WriteTo(codeStream);
		}

		public void Emit(OpcodeEncoder opcode, Operand symbolOperand, int symbolOffset)
		{
			int pos = (int)codeStream.Position + symbolOffset;

			Emit(opcode);

			if (symbolOperand.IsLabel)
			{
				linker.Link(LinkType.AbsoluteAddress, PatchType.I4, MethodName, SectionKind.Text, pos, 0, symbolOperand.Name, SectionKind.ROData, 0);
			}
			else if (symbolOperand.IsField)
			{
				var section = symbolOperand.Field.Data != null ? SectionKind.ROData : SectionKind.BSS;

				linker.Link(LinkType.AbsoluteAddress, PatchType.I4, MethodName, SectionKind.Text, pos, 0, symbolOperand.Field.FullName, section, 0);
			}
			else if (symbolOperand.IsSymbol)
			{
				var section = symbolOperand.Method != null ? SectionKind.Text : SectionKind.ROData;

				var symbol = linker.GetSymbol(symbolOperand.Name, section);

				if (symbol == null)
				{
					symbol = linker.FindSymbol(symbolOperand.Name);
				}

				linker.Link(LinkType.AbsoluteAddress, PatchType.I4, MethodName, SectionKind.Text, pos, 0, symbol, 0);
			}
		}

		#endregion New Code Generation Methods

		protected bool TryGetLabel(int label, out int position)
		{
			return labels.TryGetValue(label, out position);
		}

		protected void AddPatch(int label, int position)
		{
			patches.Add(new Patch(label, position));
		}

		public abstract void ResolvePatches();
	}
}
