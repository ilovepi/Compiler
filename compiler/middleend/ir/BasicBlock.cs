﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;

namespace compiler.middleend.ir
{
    //TODO: redisign this class and its supporting classes
    public class BasicBlock
    {
        public BasicBlock()
        {
            Instructions = new List<Instruction>();
            AnchorBlock = new Anchor();
        }

        public BasicBlock(string pName)
        {
            Instructions = new List<Instruction>();
            Name = pName;
            AnchorBlock = new Anchor();
        }

        public string Name { get; }
        public List<Instruction> Instructions { get; set; }

        public Anchor AnchorBlock { get; set; }


        public void AddInstruction(Instruction ins)
        {
            Instructions.Add(ins);
            AnchorBlock.Insert(ins);
        }



        public void AddInstructionList(List<Instruction> insList)
        {
            foreach (Instruction instruction in insList)
            {
                AddInstruction(instruction);
            }
        }

        public void InsertInstruction(int index, Instruction ins)
        {
            Instructions.Insert(index, ins);
            AnchorBlock.Insert(ins);
        }

        public void InsertInstructionList(int index, List<Instruction> insList)
        {
            foreach (Instruction instruction in Enumerable.Reverse( insList) )
            {
                InsertInstruction(index, instruction);
            }
        }



        public Instruction Search(Instruction ins)
        {
            List<Instruction> instList = AnchorBlock.FindOpChain(ins.Op);

            if (instList != null)
            {
                foreach (Instruction item in instList)
                {
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