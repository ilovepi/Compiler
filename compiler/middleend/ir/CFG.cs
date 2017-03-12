using System.Collections.Generic;
using compiler.frontend;

namespace compiler.middleend.ir
{
    public class Cfg
    {
        private Queue<Node> _q = new Queue<Node>();
        private HashSet<Node> _visited = new HashSet<Node>();

        // External data for BFS
        //private int BlockCount = 0;
        public string DotOutput = string.Empty;
        public List<VariableType> Globals;
        public List<VariableType> Locals;

        public List<VariableType> Parameters;


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

        public void InsertBranches()
        {
            var visited = new HashSet<Node>();
            Root.InsertBranches(visited);
        }


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