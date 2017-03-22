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
using compiler.middleend.ir;

#endregion

namespace compiler.middleend.optimization
{
    public static class CsElimination
    {
        public static void Eliminate(Node root)
        {
            var visited = new HashSet<Node>();
            Eliminate(root, visited);
        }


        private static void Eliminate(Node root, HashSet<Node> visited)
        {
            if ((root == null) || visited.Contains(root))
            {
                return;
            }

            visited.Add(root);
            var removalList = new List<Instruction>();

            var delayed = new List<Instruction>();

            foreach (Instruction bbInstruction in root.Bb.Instructions)
            {
                if ((bbInstruction.Op != IrOps.Phi))// && (bbInstruction.Op != IrOps.Adda))
                {
                    if (bbInstruction.Op == IrOps.Load)
                    {
                        //if (bbInstruction.Arg1.Kind == Operand.OpType.Instruction)
                        {
                            //if (bbInstruction.Arg1.Inst.Op == IrOps.Adda)
                            {
                                //continue;
                            }
                        }
                        //TODO fix cse for loop headers

                        if (root.GetType() == typeof(WhileNode))
                        {
                            // determine if we can replace it later
                            delayed.Add(bbInstruction);
                            continue;
                        }
                        //continue;
                    }
                    else if (bbInstruction.Op == IrOps.Store)
                    {
                        // insert kill instruction for all loads
                        root.Bb.AnchorBlock.InsertKill(bbInstruction.Arg2);
                    }


                    switch (bbInstruction.Op)
                    {
                        case IrOps.Store:
                        case IrOps.Move:
                        case IrOps.Phi:
                        case IrOps.End:
                        case IrOps.Bra:
                        case IrOps.Bne:
                        case IrOps.Beq:
                        case IrOps.Ble:
                        case IrOps.Blt:
                        case IrOps.Bge:
                        case IrOps.Bgt:
                        case IrOps.Ret:
                        case IrOps.Read:
                        case IrOps.Write:
                        case IrOps.WriteNl:
                        //case IrOps.Adda:
                        //case IrOps.Load:
                        case IrOps.Call:

                            break;
                        case IrOps.Neg:
                        case IrOps.Add:
                        case IrOps.Sub:
                        case IrOps.Mul:
                        case IrOps.Div:
                        case IrOps.Cmp:
                        //case IrOps.Adda:
                        //case IrOps.Load:
                        default:

                            EliminatePriv(root, bbInstruction, removalList, false);
                            break;
                    }
                }
            }

            // can't mutate a list while we're iterating through it so delay removal till here
            foreach (Instruction instruction in removalList)
            {
                //root.Bb.AnchorBlock.FindOpChain(instruction.Op).RemoveAll(instruction.ExactMatch);
                switch (instruction.Op)
                {
                    case IrOps.Store:
                    case IrOps.Move:
                    case IrOps.Phi:
                    case IrOps.End:
                    case IrOps.Bra:
                    case IrOps.Bne:
                    case IrOps.Beq:
                    case IrOps.Ble:
                    case IrOps.Blt:
                    case IrOps.Bge:
                    case IrOps.Bgt:
                    case IrOps.Ret:
                    case IrOps.Read:
                    case IrOps.Write:
                    case IrOps.WriteNl:
                    //case IrOps.Adda:
                   // case IrOps.Load:
                    case IrOps.Call:

                        break;
                    case IrOps.Neg:
                    case IrOps.Add:
                    case IrOps.Sub:
                    case IrOps.Mul:
                    case IrOps.Div:
                    case IrOps.Cmp:
                    //case IrOps.Adda:
                    //case IrOps.Load:
                    default:
                        root.Bb.Instructions.RemoveAll(instruction.ExactMatch);
                        break;
                }

                //rely on using instruction hashkey for removing a particular instruction
                //root.Bb.Graph.RemoveVertex(instruction);
            }


            List<Node> children = root.GetAllChildren();
            foreach (Node child in children)
            {
                Eliminate(child, visited);
            }

            // TODO: fix loop cse
            foreach (Instruction instruction in delayed)
            {
                switch (instruction.Op)
                {
                    case IrOps.Store:
                    case IrOps.Move:
                    case IrOps.Phi:
                    case IrOps.End:
                    case IrOps.Bra:
                    case IrOps.Bne:
                    case IrOps.Beq:
                    case IrOps.Ble:
                    case IrOps.Blt:
                    case IrOps.Bge:
                    case IrOps.Bgt:
                    case IrOps.Ret:
                    case IrOps.Read:
                    case IrOps.Write:
                    case IrOps.WriteNl:
                    //case IrOps.Adda:
                    //case IrOps.Load:
                    case IrOps.Call:

                        break;
                    default:
                        EliminatePriv(root, instruction, removalList, true);
                        break;
                }
            }
        }

        private static void EliminatePriv(Node root, Instruction bbInstruction, List<Instruction> removalList,
            bool alternate)
        {
            Instruction predecessor = alternate
                ? root.AnchorSearch(bbInstruction, true)
                : root.AnchorSearch(bbInstruction);


            if (predecessor != null)
            {
                // thi check is probably redundant now that we're rebuilding the search structure
                if (predecessor.Num != bbInstruction.Num)
                {
                    // delay removal from list -- because c# says so
                    bbInstruction.ReplaceInst(predecessor);
                    removalList.Add(bbInstruction);
                }
            }
            else
            {
                root.Bb.AnchorBlock.Insert(bbInstruction);
            }
        }
    }
}