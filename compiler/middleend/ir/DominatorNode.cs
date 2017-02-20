﻿using System.Collections.Generic;
using System.Linq;

using compiler.frontend;
using System;

namespace compiler.middleend.ir
{
	public class DominatorNode : IEquatable<DominatorNode>
    {
        /// <summary>
        ///     The Nodes which this basic block directly dominates
        /// </summary>
        public List<DominatorNode> Children;

		/// <summary>
		/// The Global set of visited nodes
		/// </summary>
		public static HashSet<Node> Visited;


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


		public static DomTree convertCfg(Cfg controlFlow)
		{
			Visited = new HashSet<Node>();
			DomTree d = new DomTree();
			d.Root = controlFlow.Root.convertNode();
			d.Name = controlFlow.Name;

			return d;
		}

	

		public void testInsert(Node n)
		{
			if (n == null)
				return;
			
			if (!Visited.Contains(n))
			{
				Visited.Add(n);
				InsertChild(n.convertNode());
			}
		}





		public string printGraphNode(SymbolTable Sym)
		{
			string local = string.Empty;

			local += DotId() + "[label=\"{" + DotLabel(Sym) + "\\l}\",fillcolor=" + Colorname + "]\n";
			foreach (var child in Children)
			{
				local += DotId() + "->" + child.DotId() + "\n";
			}


			foreach (var child in Children)
			{
				local += child.printGraphNode(Sym);
			}

			return local;

		}

		public string DotId()
		{
			string ret = Bb.Name;
			if(Bb.Instructions.Count != 0)
				ret += Bb.Instructions.First().Num;
			return ret;
		}

		public string DotLabel(SymbolTable pSymbolTable)
		{
			string label = Bb.Name;
			int slot = 0;

			foreach (Instruction inst in Bb.Instructions)
			{
				label += " \\l| <i" + (slot++) + ">" + inst.Display(pSymbolTable);
			}

			return label;
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

			for (int i = 0; i < Children.Count; i++)
			{
				if (Children[i] != other.Children[i])
				{
					return false;
				}
			}

			return true;
		}
	}
}