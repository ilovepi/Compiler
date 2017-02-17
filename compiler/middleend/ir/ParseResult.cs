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

        public Dictionary<int, SsaVariable> VarTable { get; set; }

        public ParseResult()
        {
            Operand = null;
            Instructions = null;
            VarTable = new Dictionary<int, SsaVariable>();
        }

        public ParseResult(Dictionary<int, SsaVariable> symTble )
        {
            Operand = null;
            Instructions = null;
            VarTable = new Dictionary<int, SsaVariable>(symTble);
        }

        public ParseResult(Operand pOperand, List<Instruction> pInstructions, Dictionary<int, SsaVariable> pSymTble)
        {
            Operand = pOperand;
            Instructions = new List<Instruction>(pInstructions);
            VarTable = new Dictionary<int, SsaVariable>(pSymTble);
        }


    }
}
