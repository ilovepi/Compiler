using System;
using System.Collections.Generic;
using System.Linq;

namespace compiler.middleend.ir
{
	public class InterferenceGraph
	{
		public struct GraphNode
		{
			public HashSet<GraphNode> edges;
			public Instruction inst;
		}

		HashSet<GraphNode> graph;

		public InterferenceGraph()
		{
			graph = new HashSet<GraphNode>();
		}

		public void AddNode(GraphNode n)
		{
			graph.Union(new HashSet<GraphNode>() { n });
		}

		public void AddEdge(GraphNode a, GraphNode b)
		{
			if (graph.Contains(a) && graph.Contains(b))
			{
				a.edges.Add(b);
				b.edges.Add(a);
			}
			else
			{
				throw new Exception("Interference Graph Edges can only exist between two nodes in the graph");
			}
		}

		public void RemoveNode(GraphNode n)
		{
			if (graph.Contains(n))
			{
				// remove all edges from adjacent nodes
				foreach (var edge in n.edges)
				{
					edge.edges.Remove(n);
				}

				// remove node from graph
				graph.Remove(n);
			}
			else
			{
				throw new Exception("Interference Graph Does not contain Node:" + n.inst);
			}
		}


	}
}
