// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections.Generic;

namespace Mosa.Compiler.Framework
{
	/// <summary>
	/// Base class for code transformation stages.
	/// </summary>
	public abstract class BaseCodeTransformationStage : BaseMethodCompilerStage
	{
		protected delegate void VisitationDelegate(Context context);

		protected Dictionary<BaseInstruction, VisitationDelegate> visitationDictionary;

		protected override void Setup()
		{
			base.Setup();

			visitationDictionary = new Dictionary<BaseInstruction, VisitationDelegate>();

			PopulateVisitationDictionary();
		}

		protected abstract void PopulateVisitationDictionary();

		protected override void Run()
		{
			for (int index = 0; index < BasicBlocks.Count; index++)
			{
				for (var node = BasicBlocks[index].First; !node.IsBlockEndInstruction; node = node.Next)
				{
					if (node.IsEmpty)
						continue;

					instructionCount++;

					var ctx = new Context(node);

					if (!visitationDictionary.TryGetValue(ctx.Instruction, out VisitationDelegate visitationMethod))
						continue;

					visitationMethod(ctx);
				}
			}
		}
	}
}
