using compiler.frontend;
namespace compiler.middleend.ir
{
    public class DomTree
    {
        public DomTree()
        {
            Root = null;
        }

		public string GraphOutput;
		public string Name;
        public DominatorNode Root { get; set; }



        public void RecursiveDfs(DominatorNode curNode)
        {
            // TODO: include display or print function here
            foreach (DominatorNode child in curNode.Children)
            {
                if (child != null)
                {
                    RecursiveDfs(child);
                }
            }
        }

        public void DfsTraversal()
        {
            if (Root != null)
            {
                RecursiveDfs(Root);
            }
        }


		public string printTreeGraph(int n, SymbolTable Sym)
		{
			GraphOutput = string.Empty;
			GraphOutput += Root?.printGraphNode(Sym);

			GraphOutput = "subgraph cluster_" + n + " {\nlabel = \"" + Name + "\";\n node[style=filled,shape=record]\n" + GraphOutput + "}";

			return GraphOutput;
		}

    }
}