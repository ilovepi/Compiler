using System;
using compiler.middleend.ir;

namespace compiler
{
	public class CFG
	{
		public Node Root { get; set; }



		// may not need these
		//public Node Curr { get; set; }
		//public Node Accsessor { get; set; }

		public CFG()
		{
			Root = null;
		}


	    public void Insert(Node subTree, bool truePath)
	    {
	        if (Root == null)
	        {
	            Root = subTree;
	        }
	        else
	        {
                Root.Insert(subTree, truePath);
	        }
	    }



		//TODO: create methods to walk the CFG


		// preorder

		// post order

		// inorder



	}
}
