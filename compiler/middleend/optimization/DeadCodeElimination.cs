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
    class DeadCodeElimination
    {
        private static HashSet<Node> _visited;

        public static void RemoveDeadCode(Node root)
        {
            _visited = new HashSet<Node>();
            RemoveDead(root);
        }

        private static void RemoveDead(Node root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return;
            }

            _visited.Add(root);

            List<Instruction> removalList = new List<Instruction>();
            foreach (Instruction instruction in root.Bb.Instructions)
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
                        break;
                    case IrOps.Neg:
                    case IrOps.Add:
                    case IrOps.Sub:
                    case IrOps.Mul:
                    case IrOps.Div:
                    case IrOps.Cmp:
                    case IrOps.Adda:
                    case IrOps.Load:
                    default:
                        if (instruction.Uses.Count == 0)
                        {
                            removalList.Add(instruction);
                        }
                        break;
                }
            }

            foreach (Instruction instruction in removalList)
            {
                root.Bb.Instructions.Remove(instruction);
            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
                RemoveDead(child);
            }
        }
    }
}