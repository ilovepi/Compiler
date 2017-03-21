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
using QuickGraph;

#endregion

//using EdgeList = System.Collections.Generic.SortedDictionary<compiler.middleend.ir.Instruction, compiler.middleend.ir.InterferenceGraph.GraphNode>;
//using EdgeList = System.Collections.Generic.HashSet<compiler.middleend.ir.InterferenceGraph.GraphNode>;

namespace compiler.middleend.ir
{
    public class InterferenceGraph : UndirectedGraph<Instruction, UndirectedEdge<Instruction>>
    {
        // # of available registers
        private const uint RegisterCount = 26;

        public BasicBlock Bb { get; set; }

        private Stack<Instruction> _coloringStack = new Stack<Instruction>();

        public bool UseSupeNodes;

        // Colored and spilled instructions
        public Dictionary<Instruction, uint> GraphColors = new Dictionary<Instruction, uint>();
        public uint SpillCount = 32; // Virtual register to track spilled instructions, starts at reg 32

        public InterferenceGraph():base(false)
        {
           
            
            UseSupeNodes = true;
        }

        public InterferenceGraph(bool pUseSuper) : base(false)
        {
            UseSupeNodes = pUseSuper;
        }

        public InterferenceGraph(BasicBlock block) : base(false)
        {
            UseSupeNodes = true;

            AddVertexRange(block.Instructions);
            Bb = block;

            AddInterferenceEdges(block);
        }
        
        public InterferenceGraph(InterferenceGraph other) : base(false)
        {
            foreach (var vertexAdd in other.Vertices)
            {
                if (!Vertices.Contains(vertexAdd))
                {
                    AddVertex(vertexAdd);
                }
            }

            foreach (var edgeAdd in other.Edges)
            {
                if (!Edges.Contains(edgeAdd))
                {
                    AddEdge(edgeAdd);
                }
            }
        }

        public void AddInterferenceEdges(BasicBlock block)
        {
            foreach (var instruction in block.Instructions)
            {

                switch (instruction.Op)
                {
                    case IrOps.Adda:
                    case IrOps.Bra:
                    case IrOps.Bne:
                    case IrOps.Beq:
                    case IrOps.Ble:
                    case IrOps.Blt:
                    case IrOps.Bge:
                    case IrOps.Bgt:
                    case IrOps.Write:

                    //case IrOps.Ssa:
                        continue;
                }

                AddVertex(instruction);
                foreach (var item in instruction.LiveRange)
                {
                    if (item != null)
                    {
                        if ((instruction.Op == IrOps.Ssa) && (item == instruction.Arg1.Inst))
                        {
                            continue;
                        }

                        AddVertex(item);

                        if (!ContainsEdge(instruction, item) && !ContainsEdge(item, instruction))
                        {
                            var newEdge = new UndirectedEdge<Instruction>(instruction, item);
                            AddEdge(newEdge);
                        }
                    }
                }
            }
        }

        public Dictionary<Instruction, List<Instruction>> PhiGlobber()
        {
            // Key: Instruction in glob
            // Value: All instructions globbed with that one
            var globDict = new Dictionary<Instruction, List<Instruction>>();

            foreach (var v in Vertices)
            {
                var globList = new List<Instruction>();
                globList.Add(v);
                globDict.Add(v, globList);
            }

            foreach (var v in Vertices)
            {
                if (v.Op == IrOps.Phi)
                {
                    var globList = new List<Instruction>();
                    var arg1 = v.Arg1.Inst;
                    var arg2 = v.Arg2.Inst;
                    var newGlob = new List<Instruction>();

                    globList.Add(v);
                    if (!GetNeighbors(v).Contains(arg1))
                    {
                        globList.Add(arg1);
                    }
                    if (!GetNeighbors(v).Contains(arg2))
                    {
                        globList.Add(arg2);
                    }

                    foreach (var instr in globList)
                    {
                        newGlob = newGlob.Union(globDict[instr]).ToList();
                    }
                    foreach (var instr in globList)
                    {
                        globDict[instr] = newGlob;
                    }
                }
            }

            return globDict;
        }

        private List<Instruction> GetNeighbors(Instruction curNode)
        {
            var neighbors = new List<Instruction>();

            foreach (var e in AdjacentEdges(curNode))
            {
                var neighbor = e.GetOtherVertex(curNode);
                if (neighbor != curNode)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        private void ColorRecursive(InterferenceGraph curGraph)
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
                }
            }

            // If we don't find an appropriate vertex, we'll have to spill.
            if (spill)
            {
                // By default, spills the instruction with the least dependencies
                // TODO: Maybe come up with a better spilling heuristic
                var spillVertex = curGraph.Vertices.OrderByDescending(item => AdjacentDegree(item)).Last();
                GraphColors.Add(spillVertex, SpillCount++);
                curGraph.RemoveVertex(spillVertex);
            }

            // Either way, we've removed a vertex and logged it. Time for the subgraph.
            ColorRecursive(curGraph);
        }

        public void Color()
        {
            _coloringStack = new Stack<Instruction>();

            var copy = new InterferenceGraph(this);

            // Call recursive coloring fxn with the mutable copy
            ColorRecursive(copy);

            int x = 5;

            // Until there are no more instructions to be colored...
            while (_coloringStack.Count != 0)
            {
                // ... pop an instruction from the stack...
                Instruction curInstr = _coloringStack.Pop();

                // ... get a list of its neighbors' already assigned registers...
                List<uint> neighborRegs = new List<uint>();
                foreach (Instruction neighbor in GetNeighbors(curInstr))
                {
                    if (GraphColors.ContainsKey(neighbor))
                    {
                        neighborRegs.Add(GraphColors[neighbor]);
                    }
                }

                // ... and give it a different one.
                for (uint reg = RegisterCount; reg >= 1; reg--)
                {
                    if (!neighborRegs.Contains(reg))
                    {
                        GraphColors.Add(curInstr, reg);
                        break;
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