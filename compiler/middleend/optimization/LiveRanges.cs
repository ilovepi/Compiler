﻿using System;
using System.Collections;
using System.Linq;
using compiler.middleend.ir;
using NUnit.Framework;
using System.Collections.Generic;



namespace compiler
{
	public class LiveRanges
	{
		public static HashSet<Instruction> PopulateRanges(DominatorNode d, HashSet<Instruction> liveRange)
		{
			var live = new HashSet<Instruction>(liveRange);

			foreach (var inst in Enumerable.Reverse(d.Bb.Instructions))
			{
				if (inst.Arg1.Kind == Operand.OpType.Instruction)
				{
					live.Add(inst.Arg1.Inst);
				}

				if (inst.Arg2?.Kind == Operand.OpType.Instruction)
				{
					 live.Add(inst.Arg2.Inst);
				}

				// add this isntructions operands to the live range
				inst.LiveRange.UnionWith(live);

				if (inst.LiveRange.Contains(inst))
				{
					//remove this instruction from the live range
					inst.LiveRange.Remove(inst);
				}
			}

			d.Bb.Graph.AddInterferenceEdges(d.Bb);

			//d.Bb.Graph

			return live;
		}


		public static HashSet<Instruction> GenerateRanges(DominatorNode d, HashSet<Instruction> liveRange)
		{

			HashSet<Instruction> firstRange = null;
			HashSet<Instruction> newRange = new HashSet<Instruction>();
			bool singlebeBlock = true;
			foreach (var child in d.Children)
			{
				if (firstRange == null)
				{
					firstRange = PopulateRanges(child, liveRange);
				}
				else
				{
					singlebeBlock = false;
					newRange.Union(PopulateRanges(child,firstRange));
				}

			}

			if (singlebeBlock)
				newRange = firstRange;

			return PopulateRanges(d, newRange);
		}

		public static void GenerateRanges(DomTree tree)
		{
			HashSet<Instruction> liveRange = new HashSet<Instruction>();

			GenerateRanges(tree.Root, liveRange);
		}

	}
}
