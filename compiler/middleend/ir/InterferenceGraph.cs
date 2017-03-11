using System;
using System.Collections.Generic;
using QuickGraph;
using System.Linq;

//using EdgeList = System.Collections.Generic.SortedDictionary<compiler.middleend.ir.Instruction, compiler.middleend.ir.InterferenceGraph.GraphNode>;
//using EdgeList = System.Collections.Generic.HashSet<compiler.middleend.ir.InterferenceGraph.GraphNode>;


namespace compiler.middleend.ir
{
	
	public class InterferenceGraph: UndirectedGraph<Instruction, Edge<Instruction>>
	{
		public InterferenceGraph() { }

        // # of available registers
	    private const int REGISTER_COUNT = 28;

        // Generic copy of this graph (for mutation), built alongside Interference Graph
        private UndirectedGraph<Instruction, Edge<Instruction>> copy = new UndirectedGraph<Instruction, Edge<Instruction>>();

        // Colored and spilled instructions
        public Dictionary<Instruction, int> GraphColors = new Dictionary<Instruction, int>();
        public List<Instruction> spilledInstr = new List<Instruction>();

        public InterferenceGraph(BasicBlock block)
		{
			AddVertexRange(block.Instructions);
            copy.AddVertexRange(block.Instructions);

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
				        copy.AddVerticesAndEdge(new Edge<Instruction>(instruction, item));
				    }
				}
			}
		}

        private Stack<Instruction> coloringStack = new Stack<Instruction>();

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
		        if (AdjacentDegree(vertex) < REGISTER_COUNT)
		        {
                    // Put that node on the coloring stack and remove it from graph
                    coloringStack.Push(vertex);
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
                spilledInstr.Add(highest);
                curGraph.RemoveVertex(highest);
            }

            // Either way, we've removed a vertex and logged it. Time for the subgraph.
            ColorRecursive(curGraph);
		}

	    private void Color()
	    {
            Stack<Instruction> coloringStack = new Stack<Instruction>();
            List<Instruction> spilledInstr = new List<Instruction>();

            // Call recursive coloring fxn with the mutable copy
            ColorRecursive(copy);

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
	            for (int reg = 1; reg <= REGISTER_COUNT; reg++)
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
