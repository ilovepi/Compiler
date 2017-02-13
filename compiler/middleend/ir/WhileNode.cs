namespace compiler.middleend.ir
{
    public class WhileNode : CompareNode
    {
        //todo: rightnow we insert on the false node, but we need to fix that
        public WhileNode(BasicBlock pBB) : base(pBB)
        {
            NodeType = NodeTypes.WhileB;
            FalseNode = null;
            LoopParent = null;
        }

        //public Node FalseNode { get; set; }


        public Node LoopParent { get; set; }


        public override void CheckEnqueue(CFG cfg)
        {
            cfg.BFSCheckEnqueue(this, FalseNode);
            cfg.DOTOutput += Child.BB.Name + BlockNumber + " -> " + Child.BB.Name + Child.BlockNumber + "\n";
        }
    }
}