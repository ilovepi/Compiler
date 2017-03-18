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
using QuickGraph;

#endregion

//using EdgeList = System.Collections.Generic.SortedDictionary<compiler.middleend.ir.Instruction, compiler.middleend.ir.InterferenceGraph.GraphNode>;
//using EdgeList = System.Collections.Generic.HashSet<compiler.middleend.ir.InterferenceGraph.GraphNode>;

namespace compiler.middleend.ir
{
    public class InterferenceGraph : UndirectedGraph<Instruction, UndirectedEdge<Instruction>>
    {
        // # of available registers
        private const uint RegisterCount = 27;

        public BasicBlock Bb { get; set; }

        private Stack<Instruction> _coloringStack = new Stack<Instruction>();

        public bool useSuperNodes;

        // Colored and spilled instructions
        public Dictionary<Instruction, uint> GraphColors = new Dictionary<Instruction, uint>();
        public uint SpillCount = 32; // Virtual register to track spilled instructions, starts at reg 32

        public InterferenceGraph()
        {
            useSuperNodes = true;
        }

        public InterferenceGraph(bool pUseSuper)
        {
            useSuperNodes = pUseSuper;
        }

        public InterferenceGraph(BasicBlock block)
        {
            useSuperNodes = true;

            AddVertexRange(block.Instructions);
            Bb = block;

            AddInterferenceEdges(block);
        }
        
        public InterferenceGraph(InterferenceGraph other)
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

                        if (instruction.Op == IrOps.Ssa)
                        {
                            if (item == instruction.Arg1.Inst)
                                continue;
                        }


                        AddVertex(item);

                        if (!ContainsEdge(instruction, item))
                        {
                            var newEdge = new UndirectedEdge<Instruction>(instruction, item);
                            AddEdge(newEdge);
                        }
                    }
                }
            }
        }


        public InterferenceGraph PhiGlobber(Instruction root, HashSet<Instruction> visited)
        {
            var globbed = new InterferenceGraph();

            var q = new Queue<Instruction>();

            if (visited.Contains(root))
            {
                return globbed;
            }

            q.Enqueue(root);
            globbed.AddVertex(root);

            while (q.Count != 0)
            {
                var curNode = q.Dequeue();
                visited.Add(curNode);
                bool isPhi = (curNode.Op == IrOps.Phi);
                var children = new List<Instruction>();
                bool phiRemoved = false;

                foreach (var e in AdjacentEdges(curNode))
                {
                    children.Add(e.GetOtherVertex(curNode));
                }

                // Generate list of children and iteratively glob phis 'til fixpoint
                //*
                do
                {
                    var newChildren = new List<Instruction>();
                    foreach (var orphanChild in children)
                    {
                        phiRemoved = false;
                        if (!visited.Contains(orphanChild))
                        {
                            if (isPhi && (orphanChild.Op == IrOps.Phi))
                            {
                                visited.Add(orphanChild);
                                phiRemoved = true;
                                foreach (var e in AdjacentEdges(orphanChild))
                                {
                                    newChildren.Add(e.GetOtherVertex(orphanChild));
                                }
                            }

                            else
                            {
                                newChildren.Add(orphanChild);
                            }
                        }
                    }
                    children = newChildren;
                } while (phiRemoved);
                //*/

                // Actual BFS
                foreach (var adoptedChild in children)
                {
                    if (!visited.Contains(adoptedChild))
                    {
                        if (!globbed.ContainsVertex(adoptedChild))
                        {
                            globbed.AddVertex(adoptedChild);
                        }

                        if (!globbed.ContainsEdge(curNode, adoptedChild))
                        {
                            var newEdge = new UndirectedEdge<Instruction>(curNode, adoptedChild);
                            globbed.AddEdge(newEdge);
                        }
                        q.Enqueue(adoptedChild);
                    }
                }
            }

            return globbed;
        }

        /*
            if (!globbed.ContainsVertex(curNode))
            {
                globbed.AddVertex(curNode);

                foreach (var e in AdjacentEdges(curNode))
                {
                    var child = e.GetOtherVertex(curNode);
                    var newEdge = new Edge<Instruction>(curNode, child);

                    if (!globbed.ContainsEdge(newEdge))
                    {
                        globbed.AddEdge(newEdge);
                    }

                    PhiGlobberRecursive(child, isPhi);
                }
            }
        }
        */
        
        /*
        public void GlobPhis()
        {
            bool modified;

            do
            {
                Instruction PhiGlobbed = null;
                Instruction PhiRemoved = null;
                var lEdgeAddition = new List<Instruction>();
                var lEdgeRemoval = new List<Edge<Instruction>>();

                modified = false;

                foreach (var edge in Edges)
                {
                    var firstPhi = edge.Source;
                    var secondPhi = edge.Target;

                    if (firstPhi.Op == IrOps.Phi && secondPhi.Op == IrOps.Phi)
                    {
                        PhiGlobbed = firstPhi;
                        PhiRemoved = secondPhi;

                        foreach (var phiEdge in AdjacentEdges(secondPhi))
                        {
                            lEdgeRemoval.Add(phiEdge);
                            var otherVertex = phiEdge.GetOtherVertex(secondPhi);
                            if (otherVertex != firstPhi)
                            {
                                lEdgeAddition.Add(otherVertex);
                            }
                        }
                        modified = true;
                        break;
                    }
                }

                if (modified)
                {
                    foreach (var vertex in lEdgeAddition)
                    {
                        var newEdge = new Edge<Instruction>(PhiGlobbed, vertex);
                        if (!ContainsEdge(newEdge))
                        {
                            AddEdge(newEdge);
                        }
                    }

                    foreach (var edge in lEdgeRemoval)
                    {
                        RemoveEdge(edge);
                    }

                    RemoveVertex(PhiRemoved);
                }
            } while (modified);
        }
        */
        


        // Probably all broken
        /*
        public void MakeSupernodes(Instruction otherInstruction, Instruction phiInstruction )
        {

            MakeSupernodes(otherInstruction);

            var adjacent = AdjacentEdges(otherInstruction);
            foreach (var edge in adjacent)
            {
                var other = edge.GetOtherVertex(otherInstruction);
                var newEdge = new Edge<Instruction>(phiInstruction, other);

                AddEdge(newEdge);
                RemoveVertex(other);
            }
        }

        private void MakeSupernodes(Instruction phiInst)
        {
            if (phiInst.Op == IrOps.Phi)
            {
                MakeSupernodes(phiInst.Arg1.Inst, phiInst);
                MakeSupernodes(phiInst.Arg2.Inst, phiInst);
            }
        }
        */


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

            var _copy = new InterferenceGraph(this);

            // Call recursive coloring fxn with the mutable copy
            ColorRecursive(_copy);

            int x = 5;

            // Until there are no more instructions to be colored...
            while (_coloringStack.Count != 0)
            {
                // ... pop an instruction from the stack...
                Instruction curInstr = _coloringStack.Pop();

                // ... get a list of its neighbors' already assigned registers...
                List<uint> neighborRegs = new List<uint>();
                foreach (Instruction neighbor in curInstr.LiveRange)
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