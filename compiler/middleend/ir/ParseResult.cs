using System.Collections.Generic;
using VarTbl = System.Collections.Generic.SortedDictionary<int, compiler.middleend.ir.SsaVariable>;


namespace compiler.middleend.ir
{
    public class ParseResult
    {

        public ParseResult(Operand pOperand, List<Instruction> pInstructions, VarTbl pSymTble)
        {
            Operand = pOperand;
            Instructions = new List<Instruction>(pInstructions);
            VarTable = new VarTbl(pSymTble);
        }

        public Operand Operand { get; set; }

        public List<Instruction> Instructions { get; set; }

        public VarTbl VarTable { get; set; }
    }
}