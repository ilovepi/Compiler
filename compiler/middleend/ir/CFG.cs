using System;
using System.Collections.Generic;
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

        // External data for BFS
        int BlockCount = 0;
        Queue<Node> q = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();
        public string DOTOutput = "";


        // Checks whether to enqueue a child and do so if appropriate
        /* Validity check *must* be done before enqueue, since output
         * is generated for all parent-children pairs at parent. */
        private void BFSCheckEnqueue(Node parent, Node child)
	    {
            // TODO: Fix to account for cycles/join blocks
            if ((child != null) && (!visited.Contains(child)))
            {
                BlockCount++;
                child.BlockNumber = BlockCount;
                q.Enqueue(child);
                visited.Add(child);
                DOTOutput += parent.BB.Name + parent.BlockNumber.ToString() + " -> " + child.BB.Name + child.BlockNumber.ToString() + "\n";
            }
        }

        
	    public void GenerateDOTOutput()
	    {
            // Resets external BFS data on each run
            BlockCount = 0;
            q = new Queue<Node>();
            visited = new HashSet<Node>();
	        DOTOutput = String.Empty;

            q.Enqueue(Root);
	        while (q.Count > 0)
	        {
                Node current = q.Dequeue();
	            current.BlockNumber = BlockCount;

	            switch (current.NodeType)
	            {
                    case (int)Node.NodeTypes.BB:
	                    BFSCheckEnqueue(current, current.Child);
	                    break;
                    case (int)Node.NodeTypes.CompareB:
                        BFSCheckEnqueue(current, current.Child);
						BFSCheckEnqueue(current, (CompareNode)current.FalseNode);
                        break;
                    case (int)Node.NodeTypes.JoinB:
                        BFSCheckEnqueue(current, current.Child);
                        break;
                    case (int)Node.NodeTypes.WhileB:
                        BFSCheckEnqueue(current, (CompareNode)current.FalseNode);
                        DOTOutput += current.Child.BB.Name + current.BlockNumber.ToString() + " -> " + current.Child.BB.Name + current.Child.BlockNumber.ToString() + "\n";
                        break;
                }
            }
	        DOTOutput = "digraph {{\n" + DOTOutput + "}}";
	    }

    }
}
