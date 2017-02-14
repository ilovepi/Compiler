using System;
using System.Collections.Generic;
using System.Linq;
using compiler.frontend;
using compiler.middleend.ir;

namespace compiler
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

        public static int BlockID;


        public int BlockNumber;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="pBB">A Basic block</param>
        public Node(BasicBlock pBB)
        {
            BlockID++;
            BlockNumber = BlockID;
            BB = pBB;
            Parent = null;
            Child = null;
            NodeType = NodeTypes.BB;
        }

        public Node(BasicBlock pBB, NodeTypes n)
        {
            BlockID++;
            BlockNumber = BlockID;
            BB = pBB;
            Parent = null;
            Child = null;
            NodeType = n;
        }

        public BasicBlock BB { get; set; }

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
            else if (root.Child == null)
            {
                return root;
            }
            else
            {
                return root.Child.Leaf();
            }
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

            if (ReferenceEquals(this, Child))
            {
                throw new Exception("Circular reference in basic block!!");
            }

            if (Child.GetType() == typeof(Node) )
            {
                BB.AddInstructionList(Child.BB.Instructions);

                Node temp = Child;
                Child = temp.Child;

                Consolidate();
            }
            else
            {
                Child.Consolidate();
            }
        }

        public virtual void CheckEnqueue(CFG cfg)
        {
            cfg.BFSCheckEnqueue(this, Child);
        }


        public Instruction GetNextInstruction()
        {
            if (BB.Instructions.Count != 0)
            {
                return BB.Instructions.First();
            }
            else
            {
                return Child?.GetNextInstruction();
            }

        }

        public virtual Instruction GetLastInstruction()
        {
            if (Child == null)
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
                var ret = Child.GetLastInstruction();
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

        public string DotId()
        {
            return BB.Name + BlockNumber;
        }

        public string DotLabel(SymbolTable pSymbolTable)
        {
            string label = BB.Name;

            foreach (Instruction inst in BB.Instructions)
            {
                label += "\\n " + inst.display(pSymbolTable);
            }

            return label;
        }

    }
}
