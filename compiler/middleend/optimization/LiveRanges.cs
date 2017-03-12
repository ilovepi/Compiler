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

using System.Collections.Generic;
using System.Linq;
using compiler.middleend.ir;

#endregion

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