﻿namespace compiler.middleend.ir
{
    public class JoinNode : Node
    {
        public Node FalseParent { get; set; }

        public JoinNode(BasicBlock pBB) : base(pBB)
        {
        }


        public void UpdateParent(Node other, bool trueParent)
        {
            if (trueParent)
            {
                other.Child = this;
                Parent = other;
            }
            else
            {
                FalseParent = other;
                other.Child = this;
            }
            
        }

    }
}