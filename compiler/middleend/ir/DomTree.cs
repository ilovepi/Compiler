﻿#region Basic header

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

using System.IO;
using compiler.frontend;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using QuickGraph.Serialization.DirectedGraphML;

#endregion

namespace compiler.middleend.ir
{
    public class DomTree
    {
        public string GraphOutput;
        public string Name;

        public int NumReg;

        public InterferenceGraph IntGraph { get; set; }

        public DominatorNode Root { get; set; }

        public DomTree()
        {
            Root = null;
            NumReg = 0;
        }

        public string PrintTreeGraph(int n, SymbolTable sym)
        {
            GraphOutput = string.Empty;
            GraphOutput += Root?.PrintGraphNode(sym);

            GraphOutput = "subgraph cluster_" + n + " {\nlabel = \"" + Name + "\";\n node[style=filled,shape=record]\n" +
                          GraphOutput + "}";

            return GraphOutput;
        }

        public string PrintInterference(bool printGraph)
        {

           var temp = new BidirectionalGraph<Instruction, UndirectedEdge<Instruction>>();
            temp.AddVertexRange(IntGraph.Vertices);
            temp.AddEdgeRange(IntGraph.Edges);
            var graphViz = new GraphvizAlgorithm<Instruction, UndirectedEdge<Instruction>>(temp, @".", GraphvizImageType.Gif);

            graphViz.FormatVertex += FormatVertex;
            graphViz.FormatEdge += FormatEdge;
            return printGraph ? graphViz.Generate(new FileDotEngine(), Name + "-InterferenceGraph.dot") : string.Empty;
            
        }


        private static void FormatVertex(object sender, FormatVertexEventArgs<Instruction> e)
        {
            e.VertexFormatter.Label = e.Vertex.ToString();
            e.VertexFormatter.Shape = GraphvizVertexShape.Circle;
            e.VertexFormatter.BottomLabel = e.Vertex.ToString();

            //e.VertexFormatter.StrokeColor = GraphvizColor.Black;
            //e.VertexFormatter.Font = new GraphvizFont("Calibri", 11);
        }

        private static void FormatEdge(object sender, FormatEdgeEventArgs<Instruction, UndirectedEdge<Instruction>> e)
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
                File.WriteAllText(output, dot);

                /*ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = @"C:\Program Files (x86)\Graphviz2.38\bin\dot.exe";
                startInfo.Arguments = @"dot -Tsvg graph.dot -o graph.svg";

                Process.Start(startInfo);
                */
                return output;
            }
        }
    }
}