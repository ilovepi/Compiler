using System;
using System.Collections.Generic;
using compiler.middleend.ir;

namespace compiler
{
    class DominatorNode
    {
        public BasicBlock BB { get; set; }

		public DominatorNode Parent { get; set; }

        public List<DominatorNode> Children;


        public DominatorNode(BasicBlock pBB)
        {
            BB = pBB;
            Parent = null;
            Children = new List<DominatorNode>();
        }


        public bool IsRoot()
        {
            return (Parent == null);
        }


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


        public bool RemoveChild(DominatorNode other)
        {
            return Children.Remove(other);
        }

    }
}