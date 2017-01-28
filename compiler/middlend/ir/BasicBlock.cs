using System;
using System.Collections.Generic;

namespace compiler.middleend.ir
{
	public class BasicBlock
	{

		public List<Instruction> Instructions{ get; set; }

		public Anchor Anchors { get; set; }



		public BasicBlock()
		{
			Instructions = new List<Instruction>();

			Anchors = new Anchor();
		}


		public void AddInstruction(Instruction ins)
		{
			
		}


		public Instruction search(Instruction ins)
		{

			return Anchor.
		}





	}
}
