using System.Collections.Generic;
using System.Linq;
using QuickGraph.Graphviz;

namespace compiler.middleend.ir
{
    //TODO: redisign this class and its supporting classes
    public class BasicBlock
    {
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

        public string Name { get; }
        public List<Instruction> Instructions { get; set; }

        public Anchor AnchorBlock { get; set; }

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