using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.middleend.ir
{
    class ParseResult
    {
        public Operand Operand { get; set; }
        public List<Instruction> Instructions { get; set; }

        public Dictionary<SsaVariable, Instruction> VarTable { get; set; }

        public ParseResult()
        {
            Operand = null;
            Instructions = null;
            VarTable = new Dictionary<SsaVariable, Instruction>();
        }

        public ParseResult(Dictionary<SsaVariable, Instruction> symTble )
        {
            Operand = null;
            Instructions = null;
            VarTable = new Dictionary<SsaVariable, Instruction>(symTble);
        }

        public ParseResult(Operand pOperand, List<Instruction> pInstructions, Dictionary<SsaVariable, Instruction> pSymTble)
        {
            Operand = pOperand;
            Instructions = new List<Instruction>(pInstructions);
            VarTable = new Dictionary<SsaVariable, Instruction>(pSymTble);
        }


    }
}
