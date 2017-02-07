using System;
using System.Collections.Generic;
using compiler.middleend.ir;

namespace compiler
{
    class DominatorNode : Node
    {
        public DominatorNode(BasicBlock pBB) : base(pBB)
        {
            Parent = null;
            Child = null;
        }
    }
}
