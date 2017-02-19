using System.Collections.Generic;

namespace compiler.middleend.ir
{
    public class DominatorNode
    {
        /// <summary>
        ///     The Nodes which this basic block directly dominates
        /// </summary>
        public List<DominatorNode> Children;


		public static SortedSet<Node> Visited;


        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="pBb">The basic block of the node</param>
        public DominatorNode(BasicBlock pBb)
        {
            Bb = pBb;
            Parent = null;
            Children = new List<DominatorNode>();
        }

        /// <summary>
        ///     The basic block of the node
        /// </summary>
        public BasicBlock Bb { get; set; }

        /// <summary>
        ///     The parent of this node
        /// </summary>
        public DominatorNode Parent { get; set; }


        /// <summary>
        ///     Checks if this node is a root node
        /// </summary>
        /// <returns>True if this node is a root, with no parents</returns>
        public bool IsRoot()
        {
            return Parent == null;
        }


        /// <summary>
        ///     Adds a new subtree to this node.
        /// </summary>
        /// <param name="other">The root of the subtree to insert</param>
        public virtual void InsertChild(DominatorNode other)
        {
            // detatch other's subtree from any tree it previously belonged to
            if (!other.IsRoot())
            {
                other.Parent.RemoveChild(other);
            }

            // add other to this subtree
            Children.Add(other);
            other.Parent = this;
        }


        /// <summary>
        ///     Removes a node from this subtree
        /// </summary>
        /// <param name="other"> the root of the subtree to remove</param>
        /// <returns>True if a node was removed, false if the node was not found</returns>
        public bool RemoveChild(DominatorNode other)
        {
            return Children.Remove(other);
        }


		public DomTree convertCfg(Cfg controlFlow)
		{
			Visited = new SortedSet<Node>();
			DomTree d = new DomTree();
			d.Root = convertNode(controlFlow.Root);

			return d;
			
		}

		public DominatorNode convertNode(Node n)
		{
			DominatorNode d = new DominatorNode(n.Bb);
			d.testInsert(n.Child);

			return d;
		}


		public void testInsert(Node n)
		{
			if (!Visited.Contains(n))
			{
				Visited.Add(n);
				InsertChild(convertNode(n.Child));
			}
		}

		public DominatorNode convertNode(CompareNode n)
		{
			DominatorNode d = new DominatorNode(n.Bb);
			d.testInsert(n.Join);
			d.testInsert(n.Child);
			d.testInsert(n.FalseNode);

			foreach (var child in Children)
			{
				child.Parent = this;
			}

			return d;
		}

		public DominatorNode convertNode(WhileNode n)
		{
			DominatorNode d = new DominatorNode(n.Bb);
			d.testInsert(n.FalseNode);
			d.testInsert(n.Child);

			return d;
		}

    }
}