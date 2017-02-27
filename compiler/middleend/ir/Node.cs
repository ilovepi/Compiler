using System;
using System.Collections.Generic;
using System.Linq;
using compiler.frontend;

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

        public static int BlockId;


        public int BlockNumber;

        public string Colorname = "khaki";

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="pBb">A Basic block</param>
        public Node(BasicBlock pBb)
        {
            BlockId++;
            BlockNumber = BlockId;
            Bb = pBb;
            Parent = null;
            Child = null;
            NodeType = NodeTypes.BB;
        }

        public Node(BasicBlock pBb, NodeTypes n)
        {
            BlockId++;
            BlockNumber = BlockId;
            Bb = pBb;
            Parent = null;
            Child = null;
            NodeType = n;
        }

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

                Consolidate();
            }
            else
            {
                Child.Consolidate();
            }
        }

        public void CircularRef(Node childNode)
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
            return Bb.Name + BlockNumber;
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
            if (IsRoot())
            {
                return Bb.Search(ins);
            }
            Instruction res = Bb.Search(ins);
            return res ?? Parent.AnchorSearch(ins);
        }


        public virtual DominatorNode ConvertNode()
        {
            var d = new DominatorNode(Bb);
            d.TestInsert(Child);
            d.Colorname = Colorname;
            return d;
        }
    }
}