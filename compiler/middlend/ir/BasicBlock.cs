using System;
using System.Collections.Generic;

namespace compiler.middleend.ir
{
	public class BasicBlock
	{

		public List<Instruction> Instructions{ get; set; }


		public BasicBlock()
		{
			Instructions = new List<Instruction>();
		}






	}
}
