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
            Children.Add(other);
        }
        

        virtual public void UpdateParent(DominatorNode other)
        {
            Parent = other;
        }
    }
}