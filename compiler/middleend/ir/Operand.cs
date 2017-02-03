using System;
using compiler.middleend.ir;

namespace compiler.middleend.ir
{
    public class Operand : IEquatable<Operand>
    {
        public enum OpType
        {
            Constant,
            Identifier,
            Instruction,
            Register
        };


        public OpType Kind { get; set; }


        public int Val { get; set; }


        public int IdKey { get; set; }


        public Instruction Inst { get; set; }


        public Operand(Instruction pInst)
        {
            Kind = OpType.Instruction;
            Inst = pInst;
            Val = 0;
        }


        public Operand(OpType opType, int pValue)
        {
            if (opType == OpType.Instruction)
                throw new Exception("Wrong op type in constructor");

            Kind = opType;
            Inst = null;
            if (Kind == OpType.Constant)
            {
                Val = pValue;

            }
            else if (Kind == OpType.Identifier)
            {
                IdKey = pValue;
            }
            else
            {
                Val = pValue;
            }
        }

        public bool Equals(Operand other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Kind == other.Kind && Val == other.Val && IdKey == other.IdKey && Equals(Inst, other.Inst);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Operand) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Kind;
                hashCode = (hashCode * 397) ^ Val;
                hashCode = (hashCode * 397) ^ IdKey;
                hashCode = (hashCode * 397) ^ (Inst != null ? Inst.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}