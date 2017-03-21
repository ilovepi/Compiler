#region Basic header

// MIT License
// 
// Copyright (c) 2016 Paul Kirth
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#region

using System.Collections.Generic;
using System.Linq;

#endregion

namespace compiler.middleend.ir
{
    public class WhileNode : CompareNode
    {
        //public Node FalseNode { get; set; }


        public Node LoopParent { get; set; }
        //todo: rightnow we insert on the false node, but we need to fix that
        public WhileNode(BasicBlock pBb) : base(pBb, NodeTypes.WhileB)
        {
            Colorname = "turquoise";
            NodeType = NodeTypes.WhileB;
            FalseNode = null;
            LoopParent = null;
        }


        public override void CheckEnqueue(Cfg cfg)
        {
            cfg.BfsCheckEnqueue(this, Child);
            cfg.BfsCheckEnqueue(this, FalseNode);
            //cfg.DOTOutput += Child.BB.Name + BlockNumber + " -> " + Child.BB.Name + Child.BlockNumber + "\n";
        }

        public override Node Leaf()
        {
            return FalseNode == null ? this : FalseNode.Leaf();
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

        public override void Consolidate(HashSet<Node> visited)
        {
            if (visited.Contains(this))
            {
                return;
            }
            visited.Add(this);

            CircularRef(FalseNode);

            // consolidate children who exist
            Child?.Consolidate(visited);
            FalseNode?.Consolidate(visited);
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

            if (ret != null)
            {
                return ret;
            }

            if (Bb.Instructions.Count == 0)
            {
                return null;
            }

            return Bb.Instructions.Last();
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
            if (visited.Contains(this))
            {
                return;
            }

            visited.Add(this);
            Bb.Instructions.Last().Arg2 = new Operand(FalseNode.GetNextNonPhi());
            LoopParent.Bb.Instructions.Last().Arg1.Inst = GetNextNonPhi();
            foreach (Node child in GetAllChildren())
            {
                child?.InsertBranches(visited);
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


            if ((falseBranch != null) && falseBranch.ExactMatch(trueBranch))
            {
                return trueBranch;
            }

            //TODO: this is wrong we need to figure out how to do this for a join block.
            return null;
        }
    }
}