using System;
using System.Collections.Generic;
using compiler.middleend.ir;

namespace compiler
{
    class DominatorNode
    {
        /// <summary>
        /// The basic block of the node
        /// </summary>
        public BasicBlock BB { get; set; }

        /// <summary>
        /// The parent of this node
        /// </summary>
		public DominatorNode Parent { get; set; }

        /// <summary>
        /// The Nodes which this basic block directly dominates
        /// </summary>
        public List<DominatorNode> Children;


        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="pBB">The basic block of the node</param>
        public DominatorNode(BasicBlock pBB)
        {
            BB = pBB;
            Parent = null;
            Children = new List<DominatorNode>();
        }


        /// <summary>
        /// Checks if this node is a root node
        /// </summary>
        /// <returns>True if this node is a root, with no parents</returns>
        public bool IsRoot()
        {
            return (Parent == null);
        }


        /// <summary>
        /// Adds a new subtree to this node.
        /// </summary>
        /// <param name="other"></param>
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
        

        //TODO: Do we need this method?
        virtual public void UpdateParent(DominatorNode other)
        {
            Parent = other;
        }


        /// <summary>
        /// Removes a node from this subtree
        /// </summary>
        /// <param name="other"> the root of the subtree to remove</param>
        /// <returns>True if a node was removed, false if the node was not found</returns>
        public bool RemoveChild(DominatorNode other)
        {
            return Children.Remove(other);
        }

    }
}