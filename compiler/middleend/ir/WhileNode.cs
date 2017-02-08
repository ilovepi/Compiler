namespace compiler.middleend.ir
{
    public class WhileNode : CompareNode
    {
        //public Node FalseNode { get; set; }


        public Node LoopParent { get; set; }

        //todo: rightnow we insert on the false node, but we need to fix that
        public WhileNode(BasicBlock pBB) : base(pBB)
        {
            base.NodeType = NodeTypes.WhileB;
            FalseNode = null;
            LoopParent = null;
        }


        public override void CheckEnqueue(CFG cfg)
        {
            cfg.BFSCheckEnqueue(this, FalseNode);
            cfg.DOTOutput += Child.BB.Name + BlockNumber.ToString() + " -> " + Child.BB.Name + Child.BlockNumber.ToString() + "\n";
        }

    }
}