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

using System;
using System.Collections.Generic;
using System.Linq;
using compiler.middleend.ir;

#endregion

namespace compiler.middleend.optimization
{
    class Prune
    {
        private static HashSet<Node> _visited;

        public static bool StartPrune(Node root)
        {
            _visited = new HashSet<Node>();
            return PruneBranches(root);
        }

        private static bool PruneBranches(Node root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return false;
            }

            _visited.Add(root);

            bool mutatedGraph = false;

            if (root.GetType() == typeof(CompareNode))
            {
                var instList = root.Bb.Instructions;
                Instruction cmp = instList.Find((current) => current.Op == IrOps.Cmp);

                if ((cmp?.Arg1.OpenOperand().Kind == Operand.OpType.Constant) &&
                    (cmp?.Arg2.OpenOperand().Kind == Operand.OpType.Constant))
                {
                    var branch = instList.Last();
                    bool takeBranch;

                    var arg1 = cmp.Arg1.OpenOperand().Val;
                    var arg2 = cmp.Arg2.OpenOperand().Val;

                    switch (branch.Op)
                    {
                        case IrOps.Bne:
                            takeBranch = arg1 != arg2;
                            break;
                        case IrOps.Beq:
                            takeBranch = arg1 == arg2;
                            break;
                        case IrOps.Ble:
                            takeBranch = arg1 <= arg2;
                            break;
                        case IrOps.Blt:
                            takeBranch = arg1 < arg2;
                            break;
                        case IrOps.Bge:
                            takeBranch = arg1 >= arg2;
                            break;
                        case IrOps.Bgt:
                            takeBranch = arg1 > arg2;
                            break;
                        default:
                            throw new Exception("Comparison cannot be evaluated!");
                    }

                    CompareNode compareNode = (CompareNode) root;
                    Node begin = compareNode.Parent;
                    JoinNode join = compareNode.Join;
                    Node joinParent = takeBranch ? join.FalseParent : join.Parent;
                    Node end = join.Child;
                    Node keep = takeBranch ? compareNode.FalseNode : compareNode.Child;

                    // propagate phi results
                    foreach (Instruction bbInstruction in join.Bb.Instructions)
                    {
                        if (takeBranch)
                        {
                            bbInstruction.ReplaceInst(bbInstruction.Arg2.Inst);
                        }
                        else
                        {
                            bbInstruction.ReplaceInst(bbInstruction.Arg1.Inst);
                        }
                    }


                    // check to see if we have a real else branch or a regular if
                    // only required to check if the evaluation is always false
                    if (takeBranch && (compareNode.Join == compareNode.FalseNode))
                    {
                        begin.Child = end;
                        end.Parent = begin;
                    }
                    else
                    {
                        // move nodes around
                        begin.Child = keep;
                        keep.Parent = begin;

                        joinParent.Child = end;
                        end.Parent = joinParent;
                    }

                    //compareNode.Parent = null;


                    //begin.Consolidate();

                    PruneBranches(begin.Child);
                    return true;
                }
            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
                mutatedGraph = (mutatedGraph || PruneBranches(child));
            }

            return mutatedGraph;
        }
    }
}