namespace compiler.middleend.ir
{
    public class CompareNode : Node
    {
       public Node FalseNode { get; set; }

        public CompareNode(BasicBlock pBB) : base(pBB)
        {
            FalseNode = null;
        }

        public void Insert(Node other, bool trueChild)
        {
            if (trueChild)
            {
                Child = other;
                other.Parent = this;
            }
            else
            {
                FalseNode = other;
                other.Parent = this;
            }

        }

        public void InsertFalse(Node other)
        {
            Insert(other, false);
        }

        public void InsertTrue(Node other)
        {
            Insert(other,true);
        }

        



    }
}