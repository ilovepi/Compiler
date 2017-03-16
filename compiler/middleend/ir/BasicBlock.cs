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
    public class BasicBlock
    {
        public string Name { get; }
        public List<Instruction> Instructions { get; set; }

        public Anchor AnchorBlock { get; set; }

        public Node.NodeTypes NodeType { get; set; }

        public BasicBlock()
        {
            Instructions = new List<Instruction>();
            AnchorBlock = new Anchor();
            //Graph = new InterferenceGraph();
        }

        public BasicBlock(string pName)
        {
            Instructions = new List<Instruction>();
            Name = pName;
            AnchorBlock = new Anchor();
            //Graph = new InterferenceGraph();
        }

        //public InterferenceGraph Graph { get; set; }


        public void AddInstruction(Instruction instruction)
        {
            Instructions.Add(instruction);
            //Graph.AddVertex(instruction);
            //AnchorBlock.Insert(ins);
        }


        public void AddInstructionList(List<Instruction> insList)
        {
            foreach (Instruction instruction in insList)
            {
                AddInstruction(instruction);
                //Graph.AddVertex(instruction);
            }
        }

        public void InsertInstruction(int index, Instruction ins)
        {
            Instructions.Insert(index, ins);
            //Graph.AddVertex(ins);
            //AnchorBlock.Insert(ins);
        }

        public void InsertInstructionList(int index, List<Instruction> insList)
        {
            foreach (Instruction instruction in Enumerable.Reverse(insList))
            {
                InsertInstruction(index, instruction);
            }
        }


        public Instruction Search(Instruction ins)
        {
            //TODO: also check for correct type of search instruction -- like a store can't be replaced with somthing else
            switch (ins.Op)
            {
                case IrOps.Store:
                case IrOps.Phi:
                    return null;
            }

            List<Instruction> instList = AnchorBlock.FindOpChain(ins.Op);

            if (instList != null)
            {
                foreach (Instruction item in Enumerable.Reverse(instList))
                {
                    // TODO: insert kill instructions after we see a load or a phi? cant remember
                    if ((item.Op == IrOps.Kill) && (ins.Op == IrOps.Load))
                    {
                        if (ins.VArId == item.VArId)
                        {
                            return ins;
                        }
                    }

                    if (item.Equals(ins))
                    {
                        return item;
                    }
                }
            }

            return null;
        }
    }
}