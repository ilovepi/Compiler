using System.Collections.Generic;
using compiler.frontend;

namespace compiler.middleend.ir
{
    public class Cfg
    {
        private Queue<Node> _q = new Queue<Node>();
        private HashSet<Node> _visited = new HashSet<Node>();
        // TODO: create visitor function that recursively clears 'visited' flags

        //TODO: create BFS method to walk the CFG

        // External data for BFS
        //private int BlockCount = 0;
        public string DotOutput = string.Empty;


        // may not need these
        //public Node Curr { get; set; }
        //public Node Accsessor { get; set; }

        public Cfg()
        {
            Root = null;
        }

        public Cfg(SymbolTable pSymbolTable)
        {
            Sym = pSymbolTable;
            Root = null;
        }

        public SymbolTable Sym { get; set; }

        public string Name { get; set; }

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

        public void Insert(Cfg subtree)
        {
            Insert(subtree.Root);
        }

/*
        public Node GetLeaf()
        {
            // TODO: Modify to check visited
            return GetLeaf(Root);
        }
*/

        public Node GetLeaf(Node child)
        {
            return Node.Leaf(child);
        }


        // Checks whether to enqueue a child and do so if appropriate
        // Validity check *must* be done before enqueue, since output
        // is generated for all parent-children pairs at parent. 
        public void BfsCheckEnqueue(Node parent, Node child)
        {
            // TODO: Fix to account for cycles/join blocks
            if (child != null)
            {
                if (!_visited.Contains(child))
                {
                    _q.Enqueue(child);
                    _visited.Add(child);
                }

                DotOutput += parent.DotId() + " -> " + child.DotId() + "\n";
            }
        }

/*
        private void CheckEnqueue(Node curNode)
        {
            BfsCheckEnqueue(curNode, curNode.Child);
        }
*/

/*
        private void CheckEnqueue(CompareNode curNode)
        {
            BfsCheckEnqueue(curNode, curNode.Child);
            BfsCheckEnqueue(curNode, curNode.FalseNode);
        }
*/

/*
        private void CheckEnqueue(JoinNode curNode)
        {
            BfsCheckEnqueue(curNode, curNode.Child);
        }
*/

/*
        private void CheckEnqueue(WhileNode curNode)
        {
            BfsCheckEnqueue(curNode,  curNode.Child);
            BfsCheckEnqueue(curNode,  curNode.FalseNode);
            //DOTOutput += CurNode.DotId() + " -> " + CurNode.Child.DotId() + "\n";
        }
*/

        public void GenerateDotOutput(int n)
        {
            // Resets external BFS data on each run
            _q = new Queue<Node>();
            _visited = new HashSet<Node>();
            DotOutput = string.Empty;

            _q.Enqueue(Root);
            while (_q.Count > 0)
            {
                Node current = _q.Dequeue();
                DotOutput += current.DotId() + "[label=\"{" + current.DotLabel(Sym) + "\\l}\",fillcolor=" +
                             current.Colorname + "]\n";
                current.CheckEnqueue(this);
            }

            DotOutput = "subgraph cluster_" + n + " {\nlabel = \"" + Name + "\";\n node[style=filled,shape=record]\n" +
                        DotOutput + "}";
        }
    }
}