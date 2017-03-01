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
            Variable,
            Function,
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

        public Operand(SsaVariable ssa)
        {
            Kind = OpType.Variable;
            Variable = ssa;
            IdKey = Variable.UuId;
        }


        public OpType Kind { get; set; }


        public int Val { get; set; }


        public int IdKey { get; set; }

        public SsaVariable Variable { get; set; }


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
            return (Kind == other.Kind) && (Val == other.Val) && (IdKey == other.IdKey) &&
                   Equals(Inst?.Num, other.Inst?.Num);
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

        public static bool operator ==(Operand left, Operand right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Operand left, Operand right)
        {
            return !Equals(left, right);
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
                case OpType.Function:
                    return "func-" + IdKey;
                case OpType.Variable:
                    return "(" + Variable + ")";
                case OpType.Constant:
                    return "#" + Val;
                case OpType.Instruction:
                    string s = Inst?.Num.ToString() ?? ".unknown";
                    return "(" + s + ")";
                case OpType.Register:
                    return "R" + Val;
                default:
                    return "ERROR!!!";
            }
        }


        public string Display(SymbolTable smb)
        {
            switch (Kind)
            {
                case OpType.Function:
                    return "func-" + IdKey;
                case OpType.Variable:
                    return "(" + Variable + ")";
                case OpType.Constant:
                    return "#" + Val;
                case OpType.Identifier:
                    return smb.Symbols[IdKey];
                case OpType.Instruction:
                    string val = (Inst != null) ? Inst.Num.ToString() : "Uninitialized";
                    return "(" + val + ")";
                case OpType.Register:
                    return "R" + Val;
            }
            return "ERROR!!!";
        }


        public Operand OpenOperand()
        {
            if ((Inst != null) && (Kind == OpType.Instruction))
            {
				if (Inst.Op == IrOps.Ssa)
                {
                    if (Inst.Arg2 == Inst.Arg2.Inst.Arg2)
                    {
                        return Inst.Arg1;
                    }
                    return Inst.Arg2?.OpenOperand() ?? this;
                }

                if (Inst.Op == IrOps.Phi)
                {
                    return this;
                }
            }


            return Kind == OpType.Variable ? Variable.Value.OpenOperand() : this;
        }
    }
}