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
    internal static class CleanUpSsa
    {
        private static HashSet<Node> _visited;

        public static void Clean(Node root)
        {
            _visited = new HashSet<Node>();
            CleanConstSsa(root);
        }

        private static void CleanConstSsa(Node root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return;
            }

            _visited.Add(root);

            foreach (Instruction instruction in root.Bb.Instructions)
            {
                if (instruction.Op == IrOps.Ssa)
                {
                    if (instruction.VArId.IsGlobal)
                    {
                        instruction.Op = IrOps.Add;
                        instruction.Arg1 = new Operand(Operand.OpType.Constant, instruction.VArId.Offset);
                        instruction.Arg2 = new Operand(Operand.OpType.Register, 30);
                        instruction.Arg2.Register = 30;
                    }
                    else if (instruction.Arg1.Kind == Operand.OpType.Constant)
                    {
                        instruction.Op = IrOps.Add;
                        instruction.Arg2 = new Operand(Operand.OpType.Constant, 0);
                    }
                }

                
            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
                CleanConstSsa(child);
            }
        }
    }
}