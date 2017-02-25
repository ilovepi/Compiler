
namespace compiler.middleend.ir
{
    public class SsaVariable
    {
       
		public SsaVariable(SsaVariable other)
		{
			Name = other.Name;
			Prev = other.Prev;
			Location = new Instruction(other.Location);
			Value = other.Value;
			UuId = other.UuId;
		}

        public SsaVariable(int puuid, Instruction plocation, Instruction pPrev, string pName)
        {
            Name = pName;
            Prev = pPrev;
            Location = plocation;
            UuId = puuid;
        }

        public int UuId { get; set; }

        public Instruction Location { get; set; }

        /// <summary>
        ///     Previous Instruction
        /// </summary>
        public Instruction Prev { get; set; }

        public string Name { get; set; }

		public Operand Value { get; set; }

        public override string ToString()
        {
            return Name + Location.Num + "=" + Value;
        }
    }
}