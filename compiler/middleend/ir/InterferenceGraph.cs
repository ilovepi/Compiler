using System;
using System.Collections.Generic;

using QuickGraph;
using System.Linq;
using System.Linq.Expressions;

//using EdgeList = System.Collections.Generic.SortedDictionary<compiler.middleend.ir.Instruction, compiler.middleend.ir.InterferenceGraph.GraphNode>;


//using EdgeList = System.Collections.Generic.HashSet<compiler.middleend.ir.InterferenceGraph.GraphNode>;
using NUnit.Framework.Internal.Commands;
using compiler.midleend.ir;
using System.Runtime.InteropServices;
using QuickGraph.Graphviz;


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
					AddVerticesAndEdge(new Edge<Instruction>(instruction, item));
				}
			}
		}

		public void color()
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
