namespace compiler.middleend.ir
{
    public class JoinNode : Node
    {
        public JoinNode(BasicBlock pBb) : base(pBb, NodeTypes.JoinB)
        {
            Colorname = "coral";
            FalseParent = null;
        }

        public Node FalseParent { get; set; }


        public override void CheckEnqueue(Cfg cfg)
        {
            cfg.BfsCheckEnqueue(this, Child);
        }


        public override void Consolidate()
        {
            CircularRef(Child);

            // consolidate children who exist
            Child?.Consolidate();
        }


        public override Instruction AnchorSearch(Instruction goal)
        {
            Instruction trueBranch = null;
            Instruction falseBranch = null;

            Instruction res = Bb.Search(goal);

            if (res != null)
            {
                return res;
            }


            if (Parent != null)
            {
                trueBranch = Parent.AnchorSearch(goal);
            }

            if (FalseParent != null)
            {
                falseBranch = FalseParent.AnchorSearch(goal);
            }

            if (falseBranch == trueBranch)
            {
                return trueBranch;
            }
            //TODO: this is wrong we need to figure out how to do this for a join block.
            return falseBranch;
        }
    }
}