using System;
using System.Collections.Generic;

namespace compiler.middleend.ir
{
	public class Anchor
	{
		public List<Instruction> Neg { get; set; }
		public List<Instruction> Add { get; set; }
		public List<Instruction> Sub { get; set; }
		public List<Instruction> Mul { get; set; }
		public List<Instruction> Div { get; set; }
		public List<Instruction> Cmp { get; set; }
		public List<Instruction> Adda { get; set; }
		public List<Instruction> Load { get; set; }
		public List<Instruction> Store { get; set; }
		public List<Instruction> Move { get; set; }
		public List<Instruction> Phi { get; set; }
		public List<Instruction> End { get; set; }
		public List<Instruction> Bra { get; set; }
		public List<Instruction> Bne { get; set; }
		public List<Instruction> Ble { get; set; }
		public List<Instruction> Blt { get; set; }
		public List<Instruction> Bge { get; set; }
		public List<Instruction> Bgt { get; set; }
		public List<Instruction> Read { get; set; }
		public List<Instruction> Write { get; set; }
		public List<Instruction> WriteNL { get; set; }



		public Anchor()
		{
			Neg = new List<Instruction>();
			Add = new List<Instruction>();
			Sub = new List<Instruction>();
			Mul = new List<Instruction>();
			Div = new List<Instruction>();
			Cmp = new List<Instruction>();
			Adda = new List<Instruction>();
			Load = new List<Instruction>();
			Store = new List<Instruction>();
			Move = new List<Instruction>();
			Phi = new List<Instruction>();
			End = new List<Instruction>();
			Bra = new List<Instruction>();
			Bne = new List<Instruction>();
			Ble = new List<Instruction>();
			Blt = new List<Instruction>();
			Bge = new List<Instruction>();
			Bgt = new List<Instruction>();
			Read = new List<Instruction>();
			Write = new List<Instruction>();
			WriteNL = new List<Instruction>();
		}



	}
}