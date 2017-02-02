using System;
namespace compiler.middleend.ir
{
	public class Instruction : IEquatable<Instruction>
	{
		/// <summary>
		/// The Instruction number
		/// </summary>
		/// <value>The number.</value>
		public int Num { get; set; }

		/// <summary>
		/// The op/op code
		/// </summary>
		/// <value>The op.</value>
		public int Op { get; set; }

		/// <summary>
		/// The First Argument in the Instruction
		/// </summary>
		/// <value>The arg1.</value>
		//public int Arg1 { get; set; }

		/// <summary>
		/// The Second Argument in the instruction
		/// </summary>
		/// <value>The arg2.</value>
		//public int Arg2 { get; set; }

		/// <summary>
		/// Linked list pointer to the previous instruction
		/// </summary>
		Instruction Prev { get; set; }

		/// <summary>
		/// Linked list pointer to the next instruction
		/// </summary>
		Instruction Next { get; set; }

		/// <summary>
		/// Pointer to the next Instruction of the same type (op) 
		/// </summary>
		public Instruction Search { set; get; }

		public Instruction(int pNum, int pOp, int pArg1, int pArg2)
		{
			Num = pNum;
			Op = pOp;
			//Arg1 = pArg1;
			//Arg2 = pArg2;

			Prev = null;
			Next = null;
			Search = null;
		}

		public bool Equals(Instruction other)
		{
			//TODO: determine if the instruction number is important in comparison
		    return (Op == other.Op);  // && (Arg1 == other.Arg1) && (Arg2 == other.Arg2);
		}





	}
}
