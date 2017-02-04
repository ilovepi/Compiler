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


	    public void Insert(Node subTree)
	    {
	        if (Root == null)
	        {
	            Root = subTree;
	        }
	        else
	        {
	            GetLeaf(Root).Insert(subTree);
	        }
	    }

	    public void Insert(CFG subtree)
	    {
	        Insert(subtree.Root);
	    }

	    public Node GetLeaf()
	    {
	        return GetLeaf(Root);
	    }

	    public Node GetLeaf(Node child)
	    {
	        return Node.Leaf(child);
	    }



		//TODO: create methods to walk the CFG


		// preorder

		// post order

		// inorder



	}
}
