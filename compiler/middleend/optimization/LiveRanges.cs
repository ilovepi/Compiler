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

namespace compiler.middleend.optimization
{
    internal static class LiveRanges
    {
        private static HashSet<Instruction> PopulateRanges(DominatorNode d, HashSet<Instruction> liveRange,
            InterferenceGraph intGraph)
        {
            var live = new HashSet<Instruction>(liveRange);

            foreach (Instruction inst in Enumerable.Reverse(d.Bb.Instructions))
            {
                switch (inst.Op)
                {
                    case IrOps.Bra:
                    case IrOps.End:
                    case IrOps.WriteNl:
                        continue;
                }


                // add this instruction's operands to the live range



                if (live.Contains(inst))
                {
                    // remove this instruction from the live range
                    //live.Remove(inst);
                    live.Remove(inst);
                }

                inst.LiveRange.UnionWith(live);


                // add arguments to the live range
                AddArgToLiveRange(inst.Arg1, live);


                switch (inst.Op)
                {

                    case IrOps.Bne:
                    case IrOps.Beq:
                    case IrOps.Ble:
                    case IrOps.Blt:
                    case IrOps.Bge:
                    case IrOps.Bgt:
                    case IrOps.Write:
                        break;
                    default:
                        AddArgToLiveRange(inst.Arg2, live);
                        break;
                }



            }

            intGraph.AddInterferenceEdges(d.Bb);

            return live;
        }

        private static void AddArgToLiveRange(Operand arg, HashSet<Instruction> live)
        {
            if ((arg?.Kind == Operand.OpType.Instruction) && (arg.Inst?.Op != IrOps.Ssa))
            {
                live.Add(arg.Inst);
            }
            else if (arg?.Inst?.Op == IrOps.Ssa)
            {
                live.Add(arg.Inst.Arg1.Inst);
            }
        }


        private static HashSet<Instruction> GenerateRanges(DominatorNode d, HashSet<Instruction> liveRange,
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


        /// <summary>
        /// Generates live ranges for Compare, Join and standard Basic Blocks
        /// </summary>
        /// <param name="d">A dominator node in the dominator tree</param>
        /// <param name="liveRange">A set of instructions that are currently alive</param>
        /// <param name="intGraph">The current interference graph of the function</param>
        /// <returns></returns>
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
                    var temprange = GenerateRanges(child, firstRange, intGraph);
                    temprange.IntersectWith(firstRange);
                    newRange.UnionWith(temprange);
                }
            }

            // if we only have a single block, then firstrange might be null,
            // so we might not want to replace newrange in that case
            // but if we have more than one range, we've already unioned the
            // new ranges together.
            if (singleBlock && (firstRange != null))
            {
                newRange = firstRange;
            }

            return PopulateRanges(d, newRange, intGraph);
        }


        // Handles While
        private static HashSet<Instruction> GenerateLoopRanges(DominatorNode d, HashSet<Instruction> liveRange,
            InterferenceGraph intGraph)
        {
            // Get live range from the follow block
            var followRange = GenerateRanges(d.Children[0], liveRange, intGraph);

            // interfere the follow block with the loop header
            var headerRange = PopulateRanges(d, followRange, intGraph);

            // interfere the loop body with the new loop header live range
            headerRange.UnionWith(GenerateRanges(d.Children[1], headerRange, intGraph));

            // update the header range -- probably can erase this
            headerRange = PopulateRanges(d, headerRange, intGraph);

            // fix any new addtions in the loop body
            var newRange = GenerateRanges(d.Children[1], headerRange, intGraph);

            // return the new live ranges
            return PopulateRanges(d, newRange, intGraph);
        }

        /// <summary>
        /// Generates live ranges from a dominator tree
        /// </summary>
        /// <param name="tree"></param>
        public static void GenerateRanges(DomTree tree)
        {
            var liveRange = new HashSet<Instruction>();
            tree.IntGraph = new InterferenceGraph();
            GenerateRanges(tree.Root, liveRange, tree.IntGraph);
        }
    }
}