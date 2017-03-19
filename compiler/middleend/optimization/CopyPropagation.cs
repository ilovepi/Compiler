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
using System.Net.Configuration;
using compiler.middleend.ir;

#endregion

namespace compiler.middleend.optimization
{
    public static class CopyPropagation
    {
        private static HashSet<Node> _visited;

        public static void Propagate(Node root)
        {
            _visited = new HashSet<Node>();
            PropagateValues(root,true);
        }



        private static void PropagateValues(Node root, bool complete)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return;
            }
            var bb = root.Bb;
            var instList = bb.Instructions;

            foreach (Instruction instruction in instList)
            {
                if (instruction.Op == IrOps.Ssa)
                {
                    if (instruction.Arg1.Kind == Operand.OpType.Constant)
                    {
                        var assignedValue = instruction.Arg1.Val;
                        foreach (Operand operand in instruction.Uses)
                        {
                            operand.Val = assignedValue;
                            operand.Kind = Operand.OpType.Constant;
                            // leave the instruction ref an variable value intact for now
                            //operand.Inst = null;
                            //operand.Variable = null;
                        }

                        instruction.Uses.Clear();
                        instruction.UsesLocations.Clear();
                    }
                }
            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
                PropagateValues(child, true);
            }
        }



        private static void PropagateValues(Node root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return;
            }

            _visited.Add(root);

            foreach (Instruction instruction in root.Bb.Instructions)
            {
                if ((instruction.Op == IrOps.Adda) || (instruction.Op == IrOps.Load))
                {
                    continue;
                }


                if (instruction.Arg1?.Kind == Operand.OpType.Variable)
                {
                    instruction.Arg1 = instruction.Arg1.OpenOperand();
                }

                if ((instruction.Arg2?.Kind == Operand.OpType.Variable) && (instruction.Op != IrOps.Ssa))
                {
                    instruction.Arg2 = instruction.Arg2.Variable.Value.OpenOperand();
                }
            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
                PropagateValues(child);
            }
        }

        public static void ConstantFolding(Node root)
        {
            _visited = new HashSet<Node>();
            FoldConstantValues(root, _visited);
        }


        private static void FoldConstantValues(Node root, HashSet<Node> visited)
        {
            if ((root == null) || visited.Contains(root))
            {
                return;
            }

            visited.Add(root);
            var removalList = new List<Instruction>();

            foreach (var bbInstruction in root.Bb.Instructions)
            {
                if ((bbInstruction.Op != IrOps.Phi) && (bbInstruction.Op != IrOps.Load) && (bbInstruction.Op != IrOps.Adda))
                {
                    if ((bbInstruction.Arg1.Kind == Operand.OpType.Constant) &&
                        (bbInstruction.Arg2?.Kind == Operand.OpType.Constant))
                    {
                        FoldValue(bbInstruction, removalList);
                    }
                }
            }

            // can't mutate a list while we're iterating through it so delay removal till here
            foreach (Instruction instruction in removalList)
            {
                root.Bb.Instructions.RemoveAll(instruction.ExactMatch);
            }


            List<Node> children = root.GetAllChildren();
            foreach (Node child in children)
            {
                FoldConstantValues(child, visited);
            }
        }

      

        private static void FoldValue(Instruction inst, List<Instruction> removalList)
        {
            int result;
            switch (inst.Op)
            {
                case IrOps.Add:
                    result = inst.Arg1.Val + inst.Arg2.Val;
                    break;
                case IrOps.Sub:
                    result = inst.Arg1.Val - inst.Arg2.Val;
                    break;
                case IrOps.Mul:
                    result = inst.Arg1.Val * inst.Arg2.Val;
                    break;
                case IrOps.Div:
                    result = inst.Arg1.Val / inst.Arg2.Val;
                    break;
                default:
                    return;
            }

            inst.FoldConst(result);
            removalList.Add(inst);
        }
    }
}