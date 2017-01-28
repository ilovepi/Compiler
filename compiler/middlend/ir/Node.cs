using System.Collections.Generic;
using compiler.middleend.ir;
namespace compiler
{
	public class Node
	{
		public BasicBlock BB { get; set; }

		public Node TrueParent { get; set; }

		public Node FalseParent { get; set; }

		public Node TrueChild { get; set; }

		public Node FalseChild { get; set; }



		public Node(BasicBlock pBB)
		{
			BB = pBB;
			TrueChild = null;
			FalseChild = null;
			TrueParent = null;
			FalseParent = null;
		}

	}
}
