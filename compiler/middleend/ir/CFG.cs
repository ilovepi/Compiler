﻿using System;
using System.Collections.Generic;
using compiler.middleend.ir;

namespace compiler
{
    public class CFG
    {
        // TODO: create visitor function that recursively clears 'visited' flags

        //TODO: create BFS method to walk the CFG

        // External data for BFS
        private int BlockCount = 0;
        public string DOTOutput = String.Empty;
        private Queue<Node> q = new Queue<Node>();
        private HashSet<Node> visited = new HashSet<Node>();


        // may not need these
        //public Node Curr { get; set; }
        //public Node Accsessor { get; set; }

        public CFG()
        {
            Root = null;
        }

        public Node Root { get; set; }


        public void Insert(Node subTree)
        {
            if (Root == null)
            {
                Root = subTree;
            }
            else
            {
                GetLeaf(Root).Insert(subTree);
            }
        }

        public void Insert(CFG subtree)
        {
            Insert(subtree.Root);
        }

        public Node GetLeaf()
        {
            // TODO: Modify to check visited
            return GetLeaf(Root);
        }

        public Node GetLeaf(Node child)
        {
            return Node.Leaf(child);
        }


        // Checks whether to enqueue a child and do so if appropriate
        // Validity check *must* be done before enqueue, since output
        // is generated for all parent-children pairs at parent. 
        public void BFSCheckEnqueue(Node parent, Node child)
        {
            // TODO: Fix to account for cycles/join blocks
            if (child != null)
            {
                if (!visited.Contains(child))
                {
                    q.Enqueue(child);
                    visited.Add(child);
                }

                DOTOutput += parent.DotId() + "[label=\"" + parent.DotLabel() + "\"]\n";
                DOTOutput += parent.DotId() + " -> " + child.DotId() + "\n";
            }
        }

        private void CheckEnqueue(Node CurNode)
        {
            BFSCheckEnqueue(CurNode, CurNode.Child);
        }

        private void CheckEnqueue(CompareNode CurNode)
        {
            BFSCheckEnqueue(CurNode, CurNode.Child);
            BFSCheckEnqueue(CurNode, CurNode.FalseNode);
        }

        private void CheckEnqueue(JoinNode CurNode)
        {
            BFSCheckEnqueue(CurNode, CurNode.Child);
            ;
        }

        private void CheckEnqueue(WhileNode CurNode)
        {
            BFSCheckEnqueue(CurNode,  CurNode.Child);
            BFSCheckEnqueue(CurNode,  CurNode.FalseNode);
            //DOTOutput += CurNode.DotId() + " -> " + CurNode.Child.DotId() + "\n";
        }

        public void GenerateDOTOutput()
        {
            // Resets external BFS data on each run
            q = new Queue<Node>();
            visited = new HashSet<Node>();
            DOTOutput = string.Empty;

            q.Enqueue(Root);
            while (q.Count > 0)
            {
                Node current = q.Dequeue();
                current.CheckEnqueue(this);
            }

            DOTOutput = "digraph {\n" + DOTOutput + "}";
        }
    }
}
