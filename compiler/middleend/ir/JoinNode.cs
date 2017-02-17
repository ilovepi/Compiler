using System;
using NUnit.Framework.Constraints;

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


        public override Instruction AnchorSearch(Instruction ins)
        {
            Instruction trueBranch = null;
            Instruction falseBranch = null;

            var res = Bb.Search(ins);

            if (res != null)
                return res;


            if (Parent != null)
            {
                trueBranch = Parent.AnchorSearch(ins);
            }

            if (FalseParent != null)
            {
                falseBranch = FalseParent.AnchorSearch(ins);
            }

            if (falseBranch == trueBranch)
                return trueBranch;
            else
            {
                //TODO: this is wrong we need to figure out how to do this for a join block.
                return falseBranch;
            }
            
           
        }


    }
}