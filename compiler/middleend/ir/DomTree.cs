using System.Diagnostics;
using compiler.frontend;
using NUnit.Framework;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace compiler.middleend.ir
{
    public class DomTree
    {
        public string GraphOutput;
        public string Name;
        public InterferenceGraph intGraph { get; set; }

        public DomTree()
        {
            Root = null;
        }

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


        public string PrintTreeGraph(int n, SymbolTable Sym)
        {
            GraphOutput = string.Empty;
            GraphOutput += Root?.PrintGraphNode(Sym);

            GraphOutput = "subgraph cluster_" + n + " {\nlabel = \"" + Name + "\";\n node[style=filled,shape=record]\n" +
                          GraphOutput + "}";

            return GraphOutput;
        }

        public string PrintInterference()
        {

            var temp = intGraph.Edges.ToAdjacencyGraph<Instruction, Edge<Instruction>>();
            var graphViz = new GraphvizAlgorithm<Instruction, Edge<Instruction>>(temp, @".", GraphvizImageType.Gif);

            graphViz.FormatVertex += FormatVertex;
            graphViz.FormatEdge += FormatEdge;
            graphViz.Generate(new FileDotEngine(), "InterferenceGraph.dot");

            return intGraph.ToGraphviz();
        }




        private static void FormatVertex(object sender, FormatVertexEventArgs<Instruction> e)
        {
            e.VertexFormatter.Label = e.Vertex.ToString();
            e.VertexFormatter.Shape = GraphvizVertexShape.Circle;

            e.VertexFormatter.BottomLabel = e.Vertex.ToString();


            //e.VertexFormatter.StrokeColor = GraphvizColor.Black;
            //e.VertexFormatter.Font = new GraphvizFont("Calibri", 11);
        }

        private static void FormatEdge(object sender, FormatEdgeEventArgs<Instruction, Edge<Instruction>> e)
        {
           // e.EdgeFormatter.Font = new GraphvizFont("Calibri", 8);
            e.EdgeFormatter.FontGraphvizColor = GraphvizColor.Black;
            e.EdgeFormatter.StrokeGraphvizColor = GraphvizColor.Black;
        }


        public sealed class FileDotEngine : IDotEngine
        {
            public string Run(GraphvizImageType imageType, string dot, string outputFileName)
            {
                string output = outputFileName;
                System.IO.File.WriteAllText(output, dot);

                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
                startInfo.Arguments = @"dot -Tgif graph.dot -o graph.png";

                Process.Start(startInfo);
                return output;
            }
        }

    }

}