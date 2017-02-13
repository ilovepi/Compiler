using System;
using System.Collections.Generic;
using System.Linq;
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

        public virtual void InsertTrue(JoinNode other)
        {
            Child = other;
            other.Parent = this;
        }

        public virtual void InsertFalse(JoinNode other)
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

        public static void Consolodate(Node root)
        {
            if ((root == null) || (root.Child == null))
            {
                return;
            }

            if (ReferenceEquals(root, root.Child))
            {
                throw new Exception("Circular reference in basic block!!");
            }

            if ((root.GetType() == typeof(Node)) && (root.Child.GetType() == typeof(Node)))
            {
                root.BB.Instructions.AddRange(root.Child.BB.Instructions);

                Node temp = root.Child;
                root.Child = temp.Child;

                Consolodate(root);
            }
            else
            {
                Consolodate(root.Child);
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

        public Instruction GetLastInstruction()
        {
            if (Child == null)
            {
                return BB.Instructions.Count == 0 ? null : BB.Instructions.Last();
            }
            else
            {
                return Child.GetLastInstruction();
            }
        }

    }
}