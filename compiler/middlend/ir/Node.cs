using System.Collections.Generic;
using compiler.middleend.ir;
namespace compiler
{
	public class Node
	{
		public BasicBlock BB { get; set; }

        /// <summary>
        /// Parent from a true branch(left)
        /// </summary>
		public Node TrueParent { get; set; }

        /// <summary>
        /// Parent from a false branch (right)
        /// </summary>
		public Node FalseParent { get; set; }

        /// <summary>
        /// Child on the true branch (left)
        /// </summary>
		public Node TrueChild { get; set; }


        /// <summary>
        /// Child on the false branch (right)
        /// </summary>
		public Node FalseChild { get; set; }



		public Node(BasicBlock pBB)
		{
			BB = pBB;
			TrueChild = null;
			FalseChild = null;
			TrueParent = null;
			FalseParent = null;
		}


	    public bool IsRoot()
	    {
	        return (TrueParent == null) && (FalseParent == null);
	    }





	}
}
