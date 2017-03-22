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
using compiler.frontend;

#endregion

namespace compiler.middleend.ir
{
    public class Cfg
    {
        private Queue<Node> _q = new Queue<Node>();
        private HashSet<Node> _visited = new HashSet<Node>();

        public HashSet<string> Callgraph;

        // External data for BFS
        //private int BlockCount = 0;
        public string DotOutput = string.Empty;
        public List<VariableType> Globals;
        public List<VariableType> Locals;

        public List<VariableType> Parameters;

        public HashSet<VariableType> UsedGlobals;

        public SymbolTable Sym { get; set; }

        public string Name { get; set; }

        public Node Root { get; set; }

        public SortedDictionary<VariableType, Instruction> UsedGlobalMap { get; set; }


        public bool isProcedure;

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