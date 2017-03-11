using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using compiler.frontend;
using compiler.midleend.ir;

namespace compiler.middleend.ir
{
    public class Instruction : IEquatable<Instruction>
    {
        /// <summary>
        ///     Global instruction counter for all IR instructions
        /// </summary>
        public static int InstructionCounter;

        public Instruction(Instruction other)
        {
            if (other != null)
            {
                Num = other.Num;
                Op = other.Op;
                Arg1 = other.Arg1;
                Arg2 = other.Arg2;
                LiveRange = other.LiveRange;

                Prev = other.Prev;
                Next = other.Next;
                Search = other.Search;
                Uses = other.Uses;
            }
        }


        public Instruction(IrOps pOp, Operand pArg1, Operand pArg2)
        {
            InstructionCounter++;
            Num = InstructionCounter;

            Op = pOp;
            Arg1 = pArg1;
            Arg2 = pArg2;

            AddRefs();

            LiveRange = new HashSet<Instruction>();

            Prev = null;
            Next = null;
            Search = null;
            Uses = new List<Operand>();
        }

        public int Offset;

        public VariableType VArId { get; set; }

        public List<Operand> Uses { get; set; }

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
        private Instruction Prev { get; }

        /// <summary>
        ///     Linked list pointer to the next instruction
        /// </summary>
        private Instruction Next { get; }

        /// <summary>
        ///     Pointer to the next Instruction of the same type (op)
        /// </summary>
        public Instruction Search { set; get; }


        /// <summary>
        ///     The set of live ranges used in liveness analysis
        /// </summary>
        public HashSet<Instruction> LiveRange { get; set; }

        public Register Reg { get; set; }


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


        private void AddRefs()
        {
            AddInstructionRef(Arg1);
            AddInstructionRef(Arg2);
        }

        public void AddInstructionRef(Operand op)
        {
            if (op == null)
            {
                return;
            }

            if (op.Kind == Operand.OpType.Instruction)
            {
                op.Inst?.Uses.Add(op);
            }
            else if (op.Kind == Operand.OpType.Variable)
            {
                op.Variable.Location?.Uses.Add(op);
            }
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
            string a1 = DisplayArg(smb, Arg1);
            // unconditionalbranches don't have a second arg, so they shouldn't print
            string a2 = (Op != IrOps.Bra) && ((Op != IrOps.End) && (Op != IrOps.Load))
                ? DisplayArg(smb, Arg2)
                : string.Empty;
            return $"{Num}: {Op} {a1} {a2}";
        }

        private static string DisplayArg(SymbolTable smb, Operand arg)
        {
            return arg?.Display(smb) ?? "Uninitialized Arg";
        }

        public override string ToString()
        {
            return "" + Num + ": " + Op + " " + Arg1 + " " + Arg2;
        }


        public void ReplaceInst(Instruction newInst)
        {
            foreach (Operand operand in Uses)
            {
                // TODO: this may need to be verified
                operand.Inst = newInst;
                newInst.Uses.Add(new Operand(this));
            }

            // clear all references just incase we need to fix this in Dead Code Elimination
            Uses.Clear();
        }

        public void FoldConst(int val)
        {
            foreach (Operand operand in Uses)
            {
                // TODO: this may need to be verified
                operand.Inst = null;
                operand.Val = val;
                operand.Kind = Operand.OpType.Constant;
                operand.Variable = null;
            }

            // clear all references just incase we need to fix this in Dead Code Elimination
            Uses.Clear();
        }

        public bool ExactMatch(Instruction other)
        {
            return (other?.Num == Num) && Equals(other);
        }
    }
}