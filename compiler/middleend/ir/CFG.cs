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
            // TODO: Modify to check visited
            return GetLeaf(Root);
	    }

	    public Node GetLeaf(Node child)
	    {
	        return Node.Leaf(child);
	    }


        // TODO: create visitor function that recursively clears 'visited' flags

		//TODO: create BFS method to walk the CFG
        
	}
}
