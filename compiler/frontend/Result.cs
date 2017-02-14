namespace compiler.frontend
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
        ///     Const, Variable, Register, Conditional
        /// </summary>
        public Kind Kind { get; set; }

        /// <summary>
        ///     Numeric value
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        ///     UUID for an identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     Memory address location
        /// </summary>
        public int Addr { get; set; }

        /// <summary>
        ///     Register number
        /// </summary>
        public int Regno { get; set; }

        /// <summary>
        ///     Comparison Code ??? maybe I forgot what this was
        /// </summary>
        public int Cc { get; set; }

        /// <summary>
        ///     True branch offset
        /// </summary>
        public int TrueValue { get; set; }

        /// <summary>
        ///     False branch offset
        /// </summary>
        public int FalseValue { get; set; }
    }
}