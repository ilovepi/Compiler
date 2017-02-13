using System.Collections.Generic;

namespace compiler.middleend.ir
{
    public class CompareNode : Node
    {
        public CompareNode(BasicBlock pBB) : base(pBB, NodeTypes.CompareB)
        {
            FalseNode = null;
        }


        public Node FalseNode { get; set; }


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
            Insert(other, true);
        }

        public override List<Node> GetAllChildren()
        {
            List<Node> ret = base.GetAllChildren();
            ret.Add(FalseNode);
            return ret;
        }

        public override void CheckEnqueue(CFG cfg)
        {
            cfg.BFSCheckEnqueue(this, Child);
            cfg.BFSCheckEnqueue(this, FalseNode);
        }
    }
}