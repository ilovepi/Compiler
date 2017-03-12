﻿using System.Collections.Generic;
using System.Linq;
using compiler.middleend.ir;

namespace compiler
{
    public class LiveRanges
    {
        public static HashSet<Instruction> PopulateRanges(DominatorNode d, HashSet<Instruction> liveRange,
            InterferenceGraph intGraph)
        {
            var live = new HashSet<Instruction>(liveRange);

            foreach (Instruction inst in Enumerable.Reverse(d.Bb.Instructions))
            {
                switch (inst.Op)
                {
                    case IrOps.End:
                    case IrOps.Bra:
                    case IrOps.Bne:
                    case IrOps.Beq:
                    case IrOps.Ble:
                    case IrOps.Blt:
                    case IrOps.Bge:
                    case IrOps.Bgt:
                        continue;
                }

                if (inst.Arg1?.Kind == Operand.OpType.Instruction)
                {
                    live.Add(inst.Arg1.Inst);
                }

                if (inst.Arg2?.Kind == Operand.OpType.Instruction)
                {
                    live.Add(inst.Arg2.Inst);
                }

                // add this instruction's operands to the live range
                inst.LiveRange.UnionWith(live);

                if (inst.LiveRange.Contains(inst))
                {
                    // remove this instruction from the live range
                    inst.LiveRange.Remove(inst);
                }
            }

            intGraph.AddInterferenceEdges(d.Bb);

            return live;
        }


        public static HashSet<Instruction> GenerateRanges(DominatorNode d, HashSet<Instruction> liveRange,
            InterferenceGraph intGraph)
        {
            if (d.Bb.NodeType == Node.NodeTypes.WhileB)
            {
                return GenerateLoopRanges(d, liveRange, intGraph);
            }
            else
            {
                return GenerateNonLoopRanges(d, liveRange, intGraph);
            }
        }


        // Handles BB, Compare, Join
        public static HashSet<Instruction> GenerateNonLoopRanges(DominatorNode d, HashSet<Instruction> liveRange,
            InterferenceGraph intGraph)
        {
            HashSet<Instruction> firstRange = null;
            var newRange = new HashSet<Instruction>();
            var singleBlock = true;
            foreach (DominatorNode child in d.Children)
            {
                if (firstRange == null)
                {
                    firstRange = GenerateRanges(child, liveRange, intGraph);
                }
                else
                {
                    singleBlock = false;
                    newRange.UnionWith(GenerateRanges(child, firstRange, intGraph));
                }
            }

            if (singleBlock && (firstRange != null))
            {
                newRange = firstRange;
            }

            return PopulateRanges(d, newRange, intGraph);
        }


        // Handles While
        public static HashSet<Instruction> GenerateLoopRanges(DominatorNode d, HashSet<Instruction> liveRange,
            InterferenceGraph intGraph)
        {
            // "Regular" live range generation (as above)
            var newRange = GenerateRanges(d.Children[1], liveRange, intGraph);
            newRange.UnionWith(GenerateRanges(d.Children[0], newRange, intGraph));
            newRange = PopulateRanges(d, newRange, intGraph);

            // Union with live range of LB with new LH live range
            newRange.UnionWith(GenerateRanges(d.Children[0], newRange, intGraph));
            newRange = PopulateRanges(d, newRange, intGraph);

            // Back to LH, and then back to LB again
            newRange = GenerateRanges(d.Children[1], newRange, intGraph);
            newRange.UnionWith(GenerateRanges(d.Children[0], newRange, intGraph));
            newRange = PopulateRanges(d, newRange, intGraph);

            newRange.UnionWith(GenerateRanges(d.Children[0], newRange, intGraph));
            return PopulateRanges(d, newRange, intGraph);
        }


        public static void GenerateRanges(DomTree tree)
        {
            var liveRange = new HashSet<Instruction>();
            tree.IntGraph = new InterferenceGraph();

            GenerateRanges(tree.Root, liveRange, tree.IntGraph);
        }
    }
}