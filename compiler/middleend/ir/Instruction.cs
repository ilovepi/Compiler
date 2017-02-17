using System;
using System.Collections.Generic;
using compiler.frontend;

namespace compiler.middleend.ir
{
    public class Instruction : IEquatable<Instruction>
    {
        /// <summary>
        ///     Global instruction counter for all IR instructions
        /// </summary>
        public static int InstructionCounter;

        public Instruction(IrOps pOp, Operand pArg1, Operand pArg2)
        {
            InstructionCounter++;
            Num = InstructionCounter;

            Op = pOp;
            Arg1 = pArg1;
            Arg2 = pArg2;

            LiveRange = new HashSet<Instruction>();

            Prev = null;
            Next = null;
            Search = null;
        }

        /// <summary>
        ///     The Instruction number
        /// </summary>
        /// <value>The number.</value>
        public int Num { get; set; }

        /// <summary>
        ///     The op/op code
        /// </summary>
        /// <value>The op.</value>
        public IrOps Op { get; set; }

        /// <summary>
        ///     The First Argument in the Instruction
        /// </summary>
        /// <value>The arg1.</value>
        public Operand Arg1 { get; set; }

        /// <summary>
        ///     The Second Argument in the instruction
        /// </summary>
        /// <value>The arg2.</value>
        public Operand Arg2 { get; set; }

        /// <summary>
        ///     Linked list pointer to the previous instruction
        /// </summary>
        private Instruction Prev { get; set; }

        /// <summary>
        ///     Linked list pointer to the next instruction
        /// </summary>
        private Instruction Next { get; set; }

        /// <summary>
        ///     Pointer to the next Instruction of the same type (op)
        /// </summary>
        public Instruction Search { set; get; }


        /// <summary>
        ///     The set of live ranges used in liveness analysis
        /// </summary>
        public HashSet<Instruction> LiveRange { get; set; }


        public bool Equals(Instruction other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return (Op == other.Op) && Equals(Arg1, other.Arg1) && Equals(Arg2, other.Arg2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((Instruction) obj);
        }


        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Op;
                hashCode = (hashCode * 397) ^ (Arg1?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Arg2?.GetHashCode() ?? 0);
                return hashCode;
            }
        }


        public static bool operator ==(Instruction left, Instruction right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Instruction left, Instruction right)
        {
            return !Equals(left, right);
        }




        public string Display(SymbolTable smb)
        {
            return $"{Num} {Op} {Arg1?.Display(smb)} {Arg2?.Display(smb)}";
        }

        
        public override string ToString()
        {
            // TODO: determine if we can replace this exception with a runtime check or fix for the general case
            try
            {
                return "" + Num + " " + Op + " " + Arg1 + " " + Arg2;
            }
            catch (Exception e)
            {
                return "" + Num + " " + Op + " " + Arg1;
            }
        }

    }
}