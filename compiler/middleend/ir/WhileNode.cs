namespace compiler.middleend.ir
{
    public class WhileNode : CompareNode
    {
        //public Node FalseNode { get; set; }


        public Node LoopParent { get; set; }

        public WhileNode(BasicBlock pBB) : base(pBB)
        {
            FalseNode = null;
            LoopParent = null;
        }

        


    }
}