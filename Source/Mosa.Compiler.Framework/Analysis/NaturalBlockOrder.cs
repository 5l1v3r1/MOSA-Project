// Copyright (c) MOSA Project. Licensed under the New BSD License.

using System.Collections.Generic;

namespace Mosa.Compiler.Framework.Analysis
{
	/// <summary>
	/// Keeps the same block ordering.
	/// </summary>
	public class NaturalBlockOrder : IBlockOrderAnalysis
	{
		#region Data Members

		private BasicBlock[] blockOrder;

		#endregion Data members

		#region IBlockOrderAnalysis

		public IList<BasicBlock> NewBlockOrder { get { return blockOrder; } }

		public int GetLoopDepth(BasicBlock block)
		{
			return 0;
		}

		public int GetLoopIndex(BasicBlock block)
		{
			return 0;
		}

		public void PerformAnalysis(BasicBlocks basicBlocks)
		{
			blockOrder = new BasicBlock[basicBlocks.Count];
			int orderBlockCnt = 0;

			blockOrder[orderBlockCnt++] = basicBlocks.PrologueBlock;

			for (int i = 0; i < basicBlocks.Count; i++)
			{
				if (basicBlocks[i] != basicBlocks.PrologueBlock)
				{
					blockOrder[orderBlockCnt++] = basicBlocks[i];
				}
			}
		}

		#endregion IBlockOrderAnalysis
	}
}
