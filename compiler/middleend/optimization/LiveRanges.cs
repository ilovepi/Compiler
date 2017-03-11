using System.Collections.Generic;
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

                // add this isntructions operands to the live range
                inst.LiveRange.UnionWith(live);

                if (inst.LiveRange.Contains(inst))
                {
                    //remove this instruction from the live range
                    inst.LiveRange.Remove(inst);
                }
            }

            intGraph.AddInterferenceEdges(d.Bb);

            return live;
        }


        public static HashSet<Instruction> GenerateRanges(DominatorNode d, HashSet<Instruction> liveRange,
            InterferenceGraph intGraph)
        {
            HashSet<Instruction> firstRange = null;
            var newRange = new HashSet<Instruction>();
            var singlebeBlock = true;
            foreach (DominatorNode child in d.Children)
            {
                if (firstRange == null)
                {
                    firstRange = GenerateRanges(child, liveRange, intGraph);
                }
                else
                {
                    singlebeBlock = false;
                    newRange.UnionWith(GenerateRanges(child, firstRange, intGraph));
                }
            }

            if (singlebeBlock && (firstRange != null))
            {
                newRange = firstRange;
            }

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