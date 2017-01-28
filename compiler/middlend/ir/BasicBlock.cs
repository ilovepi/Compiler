using System;
using System.Collections.Generic;

namespace compiler.middleend.ir
{
	//TODO: redisign this class and its supporting classes
	public class BasicBlock
	{

		public List<Instruction> Instructions{ get; set; }

		public Anchor AnchorBlock { get; set; }



		public BasicBlock()
		{
			Instructions = new List<Instruction>();

			AnchorBlock = new Anchor();
		}


		public void AddInstruction(Instruction ins)
		{
			Instructions.Add(ins);
			AnchorBlock.Insert(ins);
		}


		public Instruction Search(Instruction ins)
		{
			var instList = AnchorBlock.FindOpChain(ins.Op);

			foreach(var item in instList)
			{
				if (item.Equals(ins))
					return item;
			}

			return null;
		}





	}
}
