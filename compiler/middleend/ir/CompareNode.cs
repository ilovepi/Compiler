using System.Collections.Generic;

namespace compiler.middleend.ir
{
    public class CompareNode : Node
    {

        public CompareNode(BasicBlock pBB) : base(pBB)
        {
            FalseNode = null;
        }
        public Node FalseNode { get; set; }

        public int NodeType = (int)NodeTypes.CompareB;

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

        public override List<Node> GetAllChildren()
        {
            List<Node> ret = base.GetAllChildren();
            ret.Add(FalseNode);
            return ret;
        }
    }
}