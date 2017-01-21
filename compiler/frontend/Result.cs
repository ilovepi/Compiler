using System;
namespace compiler
{
	public enum Kind
	{
		Constant,
		Variable,
		Register,
		Conditional
	}


	public struct Result
	{
        private int kind; // const, var, register, conditional
        private int value; // numeric value
        private int id; //id number
        private int regno; // register number
        private int cc; // conditional code: GT, GTE, LT, LTE, EQ, NEQ, 
        private int trueValue; // branch offset for true
        private int falseValue; // branch offset for false

        public int Kind { get => kind; set => kind = value; }
        public int Value { get => value; set => this.value = value; }
        public int Id { get => id; set => id = value; }
        public int Regno { get => regno; set => regno = value; }
        public int Cc { get => cc; set => cc = value; }
        public int TrueValue { get => trueValue; set => trueValue = value; }
        public int FalseValue { get => falseValue; set => falseValue = value; }
    }
}
