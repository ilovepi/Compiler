namespace compiler.middleend.ir
{
    public class WhileNode : CompareNode
    {
        //public Node FalseNode { get; set; }


        public Node FalseParent { get; set; }

        public WhileNode(BasicBlock pBB) : base(pBB)
        {
            FalseNode = null;
        }

        


    }
}