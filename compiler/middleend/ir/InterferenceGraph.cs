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

using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

#endregion

//using EdgeList = System.Collections.Generic.SortedDictionary<compiler.middleend.ir.Instruction, compiler.middleend.ir.InterferenceGraph.GraphNode>;
//using EdgeList = System.Collections.Generic.HashSet<compiler.middleend.ir.InterferenceGraph.GraphNode>;

namespace compiler.middleend.ir
{
    public class InterferenceGraph : UndirectedGraph<Instruction, Edge<Instruction>>
    {
        // # of available registers
        private const int RegisterCount = 28;

        private Stack<Instruction> _coloringStack = new Stack<Instruction>();

        // Generic copy of this graph (for mutation), built alongside Interference Graph
        private UndirectedGraph<Instruction, Edge<Instruction>> _copy =
            new UndirectedGraph<Instruction, Edge<Instruction>>();

        // Colored and spilled instructions
        public Dictionary<Instruction, int> GraphColors = new Dictionary<Instruction, int>();
        public List<Instruction> SpilledInstr = new List<Instruction>();

        public InterferenceGraph()
        {
        }

        public InterferenceGraph(BasicBlock block)
        {
            AddVertexRange(block.Instructions);
            _copy.AddVertexRange(block.Instructions);

            AddInterferenceEdges(block);

            Color();
        }

        public void AddInterferenceEdges(BasicBlock block)
        {
            foreach (var instruction in block.Instructions)
            {
                foreach (var item in instruction.LiveRange)
                {
                    if (item != null)
                    {
                        AddVerticesAndEdge(new Edge<Instruction>(instruction, item));
                        _copy.AddVerticesAndEdge(new Edge<Instruction>(instruction, item));
                    }
                }
            }
        }

        private void ColorRecursive(UndirectedGraph<Instruction, Edge<Instruction>> curGraph)
        {
            // We have to spill if we don't find a vertex with low enough edges.
            bool spill = true;

            // Base case (empty graph)
            if (curGraph.VertexCount == 0)
            {
                return;
            }

            // Step through vertices by descending edge count
            foreach (var vertex in curGraph.Vertices.OrderByDescending(item => AdjacentDegree(item)))
            {
                // Pick a node with fewer neighbors than the max
                if (AdjacentDegree(vertex) < RegisterCount)
                {
                    // Put that node on the coloring stack and remove it from graph
                    _coloringStack.Push(vertex);
                    curGraph.RemoveVertex(vertex);
                    spill = false;
                    continue;
                }
            }

            // If we don't find an appropriate vertex, we'll have to spill.
            if (spill)
            {
                // By default, spills the instruction with the most dependencies
                // TODO: Maybe come up with a better spilling heuristic
                var highest = curGraph.Vertices.OrderByDescending(item => AdjacentDegree(item)).First();
                SpilledInstr.Add(highest);
                curGraph.RemoveVertex(highest);
            }

            // Either way, we've removed a vertex and logged it. Time for the subgraph.
            ColorRecursive(curGraph);
        }

        public void Color()
        {
            Stack<Instruction> coloringStack = new Stack<Instruction>();
            List<Instruction> spilledInstr = new List<Instruction>();

            // Call recursive coloring fxn with the mutable copy
            ColorRecursive(_copy);

            // Until there are no more instructions to be colored...
            while (coloringStack.Count != 0)
            {
                // ... pop an instruction from the stack...
                Instruction curInstr = coloringStack.Pop();

                // ... get a list of its neighbors' already assigned registers...
                List<int> neighborRegs = new List<int>();
                foreach (Instruction neighbor in curInstr.LiveRange)
                {
                    if (GraphColors.ContainsKey(neighbor))
                    {
                        neighborRegs.Add(GraphColors[neighbor]);
                    }
                }

                // ... and give it a different one.
                for (int reg = 1; reg <= RegisterCount; reg++)
                {
                    if (!neighborRegs.Contains(reg))
                    {
                        GraphColors.Add(curInstr, reg);
                    }
                }

                // All coloring stack values should be assigned a color
                if (!GraphColors.ContainsKey(curInstr))
                {
                    throw new Exception("Did not color colorable reg.");
                }
            }
        }
    }
}