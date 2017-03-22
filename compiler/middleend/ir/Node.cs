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

using System;
using System.Collections.Generic;
using System.Linq;
using compiler.frontend;

#endregion

namespace compiler.middleend.ir
{
    public class Node
    {
        public enum NodeTypes
        {
            BB,
            CompareB,
            JoinB,
            WhileB
        }

        private static int BlockId;


        private readonly int _blockNumber;

        public string Colorname = "khaki";

        public BasicBlock Bb { get; set; }

        public NodeTypes NodeType { get; set; }

        /// <summary>
        ///     Parent Node
        /// </summary>
        public Node Parent { get; set; }


        /// <summary>
        ///     Successor Node
        /// </summary>
        public Node Child { get; set; }

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="pBb">A Basic block</param>
        public Node(BasicBlock pBb)
        {
            BlockId++;
            _blockNumber = BlockId;
            Bb = pBb;
            Parent = null;
            Child = null;
            NodeType = NodeTypes.BB;
            Bb.NodeType = NodeTypes.BB;
        }

        public Node(BasicBlock pBb, NodeTypes n)
        {
            BlockId++;
            _blockNumber = BlockId;
            Bb = pBb;
            Parent = null;
            Child = null;
            NodeType = n;
            Bb.NodeType = n;
        }


        /// <summary>
        ///     Checks if this node is a root with no parents
        /// </summary>
        /// <returns>True if this node has no parents</returns>
        public bool IsRoot()
        {
            return Parent == null;
        }


        /// <summary>
        ///     Inserts a node/subgraph into the CFG
        /// </summary>
        /// <param name="other">The root of the other CFG</param>
        public virtual void Insert(Node other)
        {
            if (Child == null)
            {
                Child = other;
                other.UpdateParent(this);
            }
            else
            {
                Child.Insert(other);
            }
        }

        public virtual void InsertJoinTrue(JoinNode other)
        {
            Child = other;
            other.Parent = this;
        }

        public virtual void InsertJoinFalse(JoinNode other)
        {
            other.FalseParent = this;
            Child = other;
        }

        //TODO: does this really work? maybe find a better design
        public virtual void UpdateParent(Node other)
        {
            Parent = other;
        }

        public static Node Leaf(Node root)
        {
            if (root == null)
            {
                return null;
            }
            if (root.Child == null)
            {
                return root;
            }
            return root.Child.Leaf();
        }

        public virtual Node Leaf()
        {
            if (Child == null)
            {
                return this;
            }
            return Child.Leaf();
        }


        public virtual List<Node> GetAllChildren()
        {
            var ret = new List<Node> {Child};
            return ret;
        }

        public virtual void Consolidate()
        {
            var visited = new HashSet<Node>();
            Consolidate(visited);
        }


        public virtual void Consolidate(HashSet<Node> visited)
        {
            if (visited.Contains(this))
            {
                return;
            }

            visited.Add(this);

            if (Child == null)
            {
                return;
            }

            CircularRef(Child);

            if (Child.GetType() == typeof(Node))
            {
                Bb.AddInstructionList(Child.Bb.Instructions);

                Node temp = Child;
                Child = temp.Child;

                // restart Consolidate from here to coalesc all blocks possible
                Consolidate();
            }
            else
            {
                Child.Consolidate(visited);
            }
        }


        protected void CircularRef(Node childNode)
        {
            if (ReferenceEquals(this, childNode))
            {
                throw new Exception("Circular reference in basic block!!");
            }
        }

        public virtual void CheckEnqueue(Cfg cfg)
        {
            cfg.BfsCheckEnqueue(this, Child);
        }


        public Instruction GetNextInstruction()
        {
            if (Bb.Instructions.Count != 0)
            {
                return Bb.Instructions.First();
            }
            return Child?.GetNextInstruction();
        }

        public Instruction GetNextNonPhi()
        {
            if (Bb.Instructions.Count != 0)
            {
                Instruction res =
                    Bb.Instructions.FirstOrDefault(curr => (curr.Op != IrOps.Phi) && (curr.Op != IrOps.Adda));
                return res ?? Child?.GetNextNonPhi();
            }
            return Child?.GetNextNonPhi();
        }

        public virtual Instruction GetLastInstruction()
        {
            if (Child == null)
            {
                if (Bb.Instructions.Count == 0)
                {
                    return null;
                }
                return Bb.Instructions.Last();
            }
            Instruction ret = Child.GetLastInstruction();
            if (ret == null)
            {
                if (Bb.Instructions.Count == 0)
                {
                    return null;
                }
                return Bb.Instructions.Last();
            }
            return ret;
        }

        public string DotId()
        {
            return Bb.Name + _blockNumber;
        }

        public string DotLabel(SymbolTable pSymbolTable)
        {
            string label = Bb.Name;
            var slot = 0;

            foreach (Instruction inst in Bb.Instructions)
            {
                label += " \\l| <i" + slot++ + ">" + inst.Display(pSymbolTable);
            }

            return label;
        }

        public virtual Instruction AnchorSearch(Instruction ins)
        {
            Instruction res = Bb.Search(ins);
            if (IsRoot())
            {
                return res;
            }

            return res ?? Parent.AnchorSearch(ins);
        }

        public virtual Instruction AnchorSearch(Instruction ins, bool alternate)
        {
            Instruction res = Bb.Search(ins);
            if (IsRoot())
            {
                return res;
            }

            return res ?? Parent.AnchorSearch(ins, alternate);
        }


        public virtual DominatorNode ConvertNode()
        {
            var d = new DominatorNode(Bb);
            d.TestInsert(Child);
            d.Colorname = Colorname;
            return d;
        }


        public virtual void InsertBranches(HashSet<Node> visited)
        {
            if (!visited.Contains(this))
            {
                visited.Add(this);
                foreach (Node child in GetAllChildren())
                {
                    child?.InsertBranches(visited);
                }
            }
        }



        public virtual void InsertMove(Operand src, Operand dest)
        {
			if (src.Register == dest.Register)
			{
				return;
			}

            bool swap = false;
            if (Bb.Instructions.Count > 0)
            {
                if (Bb.Instructions.Last().Op == IrOps.Bra)
                   { swap = true;}
            }

            var moveinst = new Instruction(IrOps.Move, src, dest);
            if (swap)
            {
                Bb.Instructions.Insert(Bb.Instructions.Count-1, moveinst);
            }
            else
            {
                Bb.Instructions.Add(moveinst);
            }

        }

        public virtual void InsertMoveInst(Instruction phiInst)
        {

        }
    }
}