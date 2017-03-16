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
    public class DominatorNode : IEquatable<DominatorNode>
    {
        /// <summary>
        ///     The Global set of visited nodes
        /// </summary>
        public static HashSet<Node> Visited;

        /// <summary>
        ///     The Nodes which this basic block directly dominates
        /// </summary>
        public List<DominatorNode> Children;


        public string Colorname;

        /// <summary>
        ///     The basic block of the node
        /// </summary>
        public BasicBlock Bb { get; set; }

        /// <summary>
        ///     The parent of this node
        /// </summary>
        public DominatorNode Parent { get; set; }


        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="pBb">The basic block of the node</param>
        public DominatorNode(BasicBlock pBb)
        {
            Bb = pBb;
            Parent = null;
            Children = new List<DominatorNode>();
        }

        public bool Equals(DominatorNode other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other.Children.Count != Children.Count)
            {
                return false;
            }

            if (Bb != other.Bb)
            {
                return false;
            }

            for (var i = 0; i < Children.Count; i++)
            {
                if (Children[i] != other.Children[i])
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        ///     Checks if this node is a root node
        /// </summary>
        /// <returns>True if this node is a root, with no parents</returns>
        public bool IsRoot()
        {
            return Parent == null;
        }


        /// <summary>
        ///     Adds a new subtree to this node.
        /// </summary>
        /// <param name="other">The root of the subtree to insert</param>
        public virtual void InsertChild(DominatorNode other)
        {
            // detatch other's subtree from any tree it previously belonged to
            if (!other.IsRoot())
            {
                other.Parent.RemoveChild(other);
            }

            // add other to this subtree
            Children.Add(other);
            other.Parent = this;
        }


        /// <summary>
        ///     Removes a node from this subtree
        /// </summary>
        /// <param name="other"> the root of the subtree to remove</param>
        /// <returns>True if a node was removed, false if the node was not found</returns>
        public bool RemoveChild(DominatorNode other)
        {
            return Children.Remove(other);
        }


        /// <summary>
        ///     Creates a Dominator Tree from a Control Flow Graph
        /// </summary>
        /// <returns>A new Dominator tree for the CFG</returns>
        /// <param name="controlFlow">Control flow graph.</param>
        public static DomTree ConvertCfg(Cfg controlFlow)
        {
            Visited = new HashSet<Node>();
            var d = new DomTree
            {
                Root = controlFlow.Root.ConvertNode(),
                Name = controlFlow.Name
            };

            return d;
        }


        /// <summary>
        ///     Inserts a node if it isn't null
        /// </summary>
        /// <param name="n">A child of the current node</param>
        public void TestInsert(Node n)
        {
            if (n == null)
            {
                return;
            }

            if (!Visited.Contains(n))
            {
                Visited.Add(n);
                InsertChild(n.ConvertNode());
            }
        }


        public string PrintGraphNode(SymbolTable sym)
        {
            string local = string.Empty;

            local += DotId() + "[label=\"{" + DotLabel(sym) + "\\l}\",fillcolor=" + Colorname + "]\n";
            foreach (DominatorNode child in Children)
            {
                local += DotId() + "->" + child.DotId() + "\n";
            }


            foreach (DominatorNode child in Children)
            {
                local += child.PrintGraphNode(sym);
            }

            return local;
        }

        /*
        public void Walk(Action<Action<DominatorNode>, DominatorNode> traversal, Action<DominatorNode> visitor)
        {
            traversal(visitor, this);
        }

        public static void StaticPreOrder(Action<DominatorNode> visitor, DominatorNode n)
        {
            visitor(n);
            foreach (DominatorNode child in n.Children)
            {
                StaticPreOrder(visitor, child);
            }
        }


        public static void StaticPostOrder(Action<DominatorNode> visitor, DominatorNode n)
        {
            foreach (DominatorNode child in n.Children)
            {
                StaticPostOrder(visitor, child);
            }

            visitor(n);
        }
        

        /// <summary>
        ///     Preorder the specified visitor.
        /// </summary>
        /// <returns>The preorder.</returns>
        /// <param name="visitor">Visitor.</param>
        public void Preorder(Action<DominatorNode> visitor)
        {
            visitor(this);
            foreach (DominatorNode child in Children)
            {
                child.Preorder(visitor);
            }
        }


        /// <summary>
        ///     PostOrder the specified visitor.
        /// </summary>
        /// <returns>The Postorder.</returns>
        /// <param name="visitor">Visitor.</param>
        public void Postorder(Action<DominatorNode> visitor)
        {
            foreach (DominatorNode child in Children)
            {
                child.Postorder(visitor);
            }
            visitor(this);
        }
        */

        /// <summary>
        ///     Dots the identifier.
        /// </summary>
        /// <returns>The identifier.</returns>
        public string DotId()
        {
            string ret = Bb.Name;
            if (Bb.Instructions.Count != 0)
            {
                ret += Bb.Instructions.First().Num;
            }
            return ret;
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
    }
}