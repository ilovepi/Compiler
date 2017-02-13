namespace compiler.middleend.ir
{
    public class JoinNode : Node
    {
        public JoinNode(BasicBlock pBB) : base(pBB, NodeTypes.JoinB)
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

        public virtual void CheckEnqueue(CFG cfg)
        {
            cfg.BFSCheckEnqueue(this, Child);
        }
    }
}