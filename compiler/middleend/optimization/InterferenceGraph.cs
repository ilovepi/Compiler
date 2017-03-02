using System;
using NUnit.Framework;
using System.Xml.Xsl.Runtime;
using compiler.middleend.ir;
using System.Collections.Generic;
namespace compiler
{
	public class InterferenceGraph
	{
		struct Node
		{
			HashSet<Node> edges;
			Instruction inst;
		}

		public InterferenceGraph()
		{
		}
	}
}
