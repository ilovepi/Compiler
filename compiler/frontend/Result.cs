using System;
namespace compiler
{
	public enum kind
	{
		constant,
		variable,
		register,
		conditional
	}


	public struct Result
	{
		public int kind; // const, var, register, conditional
		public int value; // numeric value
		public int id; //id number
		public int regno; // register number
		public int cc; // conditional code: GT, GTE, LT, LTE, EQ, NEQ, 
		public int true_value; // branch offset for true
		public int false_value; // branch offset for false

	}
}
