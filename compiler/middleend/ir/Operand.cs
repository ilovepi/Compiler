using System;
using compiler.frontend;

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
        }


        public Operand(Instruction pInst)
        {
            Kind = OpType.Instruction;
            Inst = pInst;
            Val = 0;
        }


        public Operand(OpType opType, int pValue)
        {
            if (opType == OpType.Instruction)
            {
                throw new Exception("Wrong op type in constructor");
            }

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


        public OpType Kind { get; set; }


        public int Val { get; set; }


        public int IdKey { get; set; }


        public Instruction Inst { get; set; }

        public bool Equals(Operand other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return (Kind == other.Kind) && (Val == other.Val) && (IdKey == other.IdKey) && Equals(Inst, other.Inst);
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
            return Equals((Operand) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Kind;
                hashCode = (hashCode * 397) ^ Val;
                hashCode = (hashCode * 397) ^ IdKey;
                int? i = (hashCode * 397) ^ Inst?.GetHashCode();
                if (i != null)
                {
                    hashCode = (int) i;
                }
                return hashCode;
            }
        }

        public override string ToString()
        {
            switch (Kind)
            {
                case OpType.Constant:
                    return "#"+Val.ToString();
                case OpType.Identifier:
                    return "VAR" + IdKey;
                case OpType.Instruction:
                    return "(" + Inst.Num + ")";
                case OpType.Register:
                    return "R" + Val;
            }
            return "ERROR!!!";
        }

    }
}