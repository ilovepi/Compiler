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
    public class CompareNode : Node
    {
        public CompareNode(BasicBlock pBb) : base(pBb, NodeTypes.CompareB)
        {
            Colorname = "cornflowerblue";
            FalseNode = null;
        }


        public CompareNode(BasicBlock pBb, NodeTypes n) : base(pBb, n)
        {
            Colorname = "cornflowerblue";
            FalseNode = null;
        }


        public Node FalseNode { get; set; }
        public JoinNode Join { get; set; }


        public void Insert(Node other, bool trueChild)
        {
            if (trueChild)
            {
                Child = other;
                other.UpdateParent(this);
            }
            else
            {
                FalseNode = other;
                other.UpdateParent(this);
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

        public override void CheckEnqueue(Cfg cfg)
        {
            cfg.BfsCheckEnqueue(this, Child);
            cfg.BfsCheckEnqueue(this, FalseNode);
        }

        public override void Consolidate(HashSet<Node> visited)
        {
            if (visited.Contains(this))
            {
                return;
            }

            visited.Add(this);

            CircularRef(Child);
            CircularRef(FalseNode);

            // consolidate children who exist
            Child?.Consolidate(visited);
            FalseNode?.Consolidate(visited);
        }


        public override DominatorNode ConvertNode()
        {
            var d = new DominatorNode(Bb);
            d.TestInsert(Join);
            d.TestInsert(Child);
            d.TestInsert(FalseNode);
            d.Colorname = Colorname;

            foreach (DominatorNode child in d.Children)
            {
                child.Parent = d;
            }

            return d;
        }

        public override void InsertBranches(HashSet<Node> visited)
        {
            if (!visited.Contains(this))
            {
                // visited.Add(this);
                Bb.Instructions.Last().Arg2 = new Operand(FalseNode.GetNextNonPhi());
                if (FalseNode != Join)
                {
                    Join.Parent.Bb.AddInstruction(new Instruction(IrOps.Bra,
                        new Operand(Join.GetNextNonPhi()), null));
                }
                FalseNode.InsertBranches(visited);
                Child.InsertBranches(visited);
            }
        }
    }
}