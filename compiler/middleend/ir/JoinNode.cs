using System;

namespace compiler.middleend.ir
{
    public class JoinNode : Node
    {
        public JoinNode(BasicBlock pBb) : base(pBb, NodeTypes.JoinB)
        {
            FalseParent = null;
        }

        public Node FalseParent { get; set; }

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

        public override void CheckEnqueue(Cfg cfg)
        {
            cfg.BfsCheckEnqueue(this, Child);
        }


        public override void Consolidate()
        {
            if (ReferenceEquals(this, Child))
            {
                throw new Exception("Circular reference in basic block!!");
            }

            // consolidate children who exist
            Child?.Consolidate();
        }


    }
}