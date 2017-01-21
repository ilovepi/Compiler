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
        /// <summary>
        /// Const, Variable, Register, Conditional
        /// </summary>
	    public int Kind { get; }

        /// <summary>
        /// Numeric value
        /// </summary>
	    public int Value { get; }

        /// <summary>
        /// UUID for an identifier
        /// </summary>
	    public int Id { get; }

        /// <summary>
        /// Register number
        /// </summary>
	    public int Regno { get; }

        /// <summary>
        /// Comparison Code ??? maybe I forgot what this was
        /// </summary>
	    public int Cc { get; }

        /// <summary>
        /// True branch offset
        /// </summary>
	    public int TrueValue { get; }

        /// <summary>
        /// False branch offset
        /// </summary>
	    public int FalseValue { get; }
	}
}
