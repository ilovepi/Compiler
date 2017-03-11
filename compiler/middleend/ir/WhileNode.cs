using System.Collections.Generic;
using System.Linq;

namespace compiler.middleend.ir
{
    public class WhileNode : CompareNode
    {
        //todo: rightnow we insert on the false node, but we need to fix that
        public WhileNode(BasicBlock pBb) : base(pBb)
        {
            Colorname = "turquoise";
            NodeType = NodeTypes.WhileB;
            FalseNode = null;
            LoopParent = null;
        }

        //public Node FalseNode { get; set; }


        public Node LoopParent { get; set; }


        public override void CheckEnqueue(Cfg cfg)
        {
            cfg.BfsCheckEnqueue(this, Child);
            cfg.BfsCheckEnqueue(this, FalseNode);
            //cfg.DOTOutput += Child.BB.Name + BlockNumber + " -> " + Child.BB.Name + Child.BlockNumber + "\n";
        }

        public override Node Leaf()
        {
            if (FalseNode == null)
            {
                return this;
            }
            return FalseNode.Leaf();
        }


        public override void Insert(Node other)
        {
            if (FalseNode == null)
            {
                FalseNode = other;
                other.UpdateParent(this);
            }
            else
            {
                FalseNode.Insert(other);
            }
        }

        //TODO: determine if thes are ever called or needed?
        /* public override void InsertJoinTrue(JoinNode other)
        {
            FalseNode = other;
            other.Parent = this;
        }

        public override void InsertJoinFalse(JoinNode other)
        {
            other.FalseParent = this;
            FalseNode = other;
        }*/

        public override void Consolidate()
        {
            CircularRef(FalseNode);

            // consolidate children who exist
            //Child?.Consolidate();
            FalseNode?.Consolidate();
        }


        public override Instruction GetLastInstruction()
        {
            if (FalseNode == null)
            {
                if (Bb.Instructions.Count == 0)
                {
                    return null;
                }
                return Bb.Instructions.Last();
            }
            Instruction ret = FalseNode.GetLastInstruction();
            if (ret == null)
            {
                if (Bb.Instructions.Count == 0)
                {
                    return null;
                }
                ret = Bb.Instructions.Last();
            }
            return ret;
        }


        public override DominatorNode ConvertNode()
        {
            var d = new DominatorNode(Bb);
            d.TestInsert(FalseNode);
            d.TestInsert(Child);
            d.Colorname = Colorname;

            return d;
        }


        public override void InsertBranches(HashSet<Node> visited)
        {
            if (!visited.Contains(this))
            {
                visited.Add(this);
                Bb.Instructions.Last().Arg2 = new Operand(FalseNode.GetNextInstruction());
                LoopParent.Bb.Instructions.Last().Arg1.Inst = Bb.Instructions.First();
                foreach (Node child in GetAllChildren())
                {
                    child?.InsertBranches(visited);
                }
            }
        }


        public override Instruction AnchorSearch(Instruction goal, bool alternate)
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

            if (LoopParent != null)
            {
                falseBranch = LoopParent.AnchorSearch(goal);
            }

            if (falseBranch.ExactMatch(trueBranch))
            {
                return trueBranch;
            }

            //TODO: this is wrong we need to figure out how to do this for a join block.
            return null;
        }
    }
}