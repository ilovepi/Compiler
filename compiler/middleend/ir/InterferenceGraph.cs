using System;
using System.Collections.Generic;
using System.Linq;

//using EdgeList = System.Collections.Generic.SortedDictionary<compiler.middleend.ir.Instruction, compiler.middleend.ir.InterferenceGraph.GraphNode>;


using EdgeList = System.Collections.Generic.HashSet<compiler.middleend.ir.InterferenceGraph.GraphNode>;


namespace compiler.middleend.ir
{
	
	public class InterferenceGraph
	{
		public class GraphNode
		{
			public EdgeList edges;
			public Instruction inst;

			public GraphNode(Instruction pInst)
			{
				inst = pInst;
				edges = new EdgeList();
			}

			public GraphNode(Instruction pInst, EdgeList e)
			{
				inst = pInst;
				edges = new EdgeList(e);
			}

			public GraphNode(GraphNode n)
			{
				inst = n.inst;
				edges = new EdgeList(n.edges);
			}

			public override int GetHashCode()
			{
				return inst.GetHashCode();
			}

		}

		public HashSet<GraphNode> Graph { get; }

		public InterferenceGraph()
		{
			Graph = new HashSet<GraphNode>();
		}

		public void AddNode(GraphNode n)
		{
			Graph.UnionWith(new HashSet<GraphNode>() { n });
		}

		public void AddEdge(GraphNode a, GraphNode b)
		{
			if (Graph.Contains(a) && Graph.Contains(b))
			{
				 a.edges.UnionWith(new EdgeList() {  b  });
				//var temp = a.edges.Union(new EdgeList() { { b.inst, b } });
				//temp.GetEnumerator();
				//a.edges = temp;
				b.edges.UnionWith(new EdgeList() {  a  });
				//b.edges.Union(new EdgeList() { { a.inst, a } });
			}
			else
			{
				throw new Exception("Interference Graph Edges can only exist between two nodes in the graph");
			}
		}

		public void RemoveNode(GraphNode n)
		{
			if (Graph.Contains(n))
			{
				// remove all edges from adjacent nodes
				foreach (var edge in n.edges)
				{
					edge.edges.Remove(n);
					//edge.Value.edges.Remove(n.inst);
				
				}

				// remove node from graph
				Graph.Remove(n);
			}
			else
			{
				throw new Exception("Interference Graph Does not contain Node:" + n.inst);
			}
		}


	}
}
