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

#endregion

namespace compiler.middleend.ir
{
    public class Anchor
    {
        public List<List<Instruction>> Oplist { get; set; }

        public Anchor()
        {
            Oplist = new List<List<Instruction>>();
        }

        /// <summary>
        ///     Insert the specified inst. into the Anchor lists
        /// </summary>
        /// <param name="inst">Inst.</param>
        public void Insert(Instruction inst)
        {
            IrOps key = inst.Op;

            List<Instruction> chain = FindOpChain(key);

            // if the op never existed, add it
            if (chain == null)
            {
                chain = new List<Instruction>();
                Oplist.Add(chain);
            }

            // insert the new instruction at the bottom of the list
            chain.Add(inst);
        }

        public List<Instruction> FindOpChain(IrOps key)
        {
            foreach (List<Instruction> sublist in Oplist)
            {
                if ((sublist.Count > 0) && (sublist.First().Op == key))
                {
                    return sublist;
                }
            }

            return null;
        }

        public void InsertKill(Operand target)
        {
            List<Instruction> chain = FindOpChain(IrOps.Load);
            // if the op never existed, add it
            if (chain == null)
            {
                chain = new List<Instruction>();
                Oplist.Add(chain);
            }

            chain.Add(new Instruction(IrOps.Kill, target, null));
        }
    }
}