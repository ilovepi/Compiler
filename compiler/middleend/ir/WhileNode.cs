using System;
using System.Collections.Generic;
using System.Linq;
using compiler.middleend.ir;


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
            cfg.BFSCheckEnqueue(this, Child);
            cfg.BFSCheckEnqueue(this, FalseNode);
            cfg.DOTOutput += Child.BB.Name + BlockNumber + " -> " + Child.BB.Name + Child.BlockNumber + "\n";
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
                if(FalseNode == null)
                    {
                        FalseNode = other;
                        other.UpdateParent(this);
                    }
                else
                    {
                        FalseNode.Insert(other);
                    }
            }


        public override void InsertJoinTrue(JoinNode other)
        {
            FalseNode = other;
            other.Parent = this;
        }

        public override void InsertJoinFalse(JoinNode other)
        {
            other.FalseParent = this;
            FalseNode = other;
        }

        public override void Consolidate()
        {
            if (ReferenceEquals(this, Child))
            {
                throw new Exception("Circular reference in basic block!!");
            }

            // consolidate children who exist
            //Child?.Consolidate();
            FalseNode?.Consolidate();
        }


        public override Instruction GetLastInstruction()
        {
            if (FalseNode == null)
            {
                if (BB.Instructions.Count == 0)
                {
                    return null;
                }
                else
                {
                    return BB.Instructions.Last();
                }
            }
            else
            {
                var ret = FalseNode.GetLastInstruction();
                if (ret == null)
                {
                    if (BB.Instructions.Count == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return BB.Instructions.Last();
                    }
                }
                else
                {
                    return ret;
                }
            }
        }

    }
}
