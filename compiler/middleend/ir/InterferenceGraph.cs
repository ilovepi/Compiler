using QuickGraph;
using System.Linq;

//using EdgeList = System.Collections.Generic.SortedDictionary<compiler.middleend.ir.Instruction, compiler.middleend.ir.InterferenceGraph.GraphNode>;
//using EdgeList = System.Collections.Generic.HashSet<compiler.middleend.ir.InterferenceGraph.GraphNode>;


namespace compiler.middleend.ir
{
	
	public class InterferenceGraph: UndirectedGraph<Instruction, Edge<Instruction>>
	{
		public InterferenceGraph() { }

		public InterferenceGraph(BasicBlock block)
		{
			AddVertexRange(block.Instructions);
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
				    }
				}
			}
		}

		public void Color()
		{


            
			// if we have enough registers, allocate all results to a register
			if (VertexCount < 28)
			{ 
			
			}



			// otherwise color each register
			var highest = this.Vertices.OrderByDescending(item => AdjacentDegree(item)).First();

		}

    }
}
