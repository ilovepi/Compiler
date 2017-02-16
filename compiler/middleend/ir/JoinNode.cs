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

        
        public override void CheckEnqueue(Cfg cfg)
        {
            cfg.BfsCheckEnqueue(this, Child);
        }


        public override void Consolidate()
        {
            base.CircularRef(Child);

            // consolidate children who exist
            Child?.Consolidate();
        }


    }
}