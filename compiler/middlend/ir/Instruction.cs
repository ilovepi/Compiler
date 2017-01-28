using System;
namespace compiler.middleend.ir
{
	public class Instruction : IEquatable<Instruction>
	{
		public int Num { get; set; }
		public int Op { get; set; }
		public int Arg1 { get; set; }
		public int Arg2 { get; set; }

		public Instruction(int pNum, int pOp, int pArg1, int pArg2)
		{
			Num = pNum;
			Op = pOp;
			Arg1 = pArg1;
			Arg2 = pArg2;
		}

		public bool Equals(Instruction other)
		{
			//TODO: determine if the instruction number is important in comparison
			return (Op == other.Op) && (Arg1 == other.Arg1) && (Arg2 == other.Arg2);
		}
	}
}
