using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.middleend.ir;

namespace compiler.backend
{
    public class DlxInstruction
    {
        public DlxInstruction(Instruction inst)
        {
            A = 0;
            B = 0;
            C = 0;

            switch (inst.Op)
            {
                case IrOps.Add:
                    ImmediateOperands(OpCodes.Add, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Sub:
                    ImmediateOperands(OpCodes.Sub, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Mul:
                    ImmediateOperands(OpCodes.Mul, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Div:
                    ImmediateOperands(OpCodes.Div, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Cmp:
                    ImmediateOperands(OpCodes.Cmp, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Load:
                    if ((inst.Arg1.Kind == Operand.OpType.Instruction) && (inst.Arg1.Inst.Op == IrOps.Adda))
                    {
                        A = (uint)inst.Arg1.Val;
                        B = (uint)inst.Arg1.Inst.Arg1.Val;
                        C = (uint)inst.Arg1.Inst.Arg2.Val;

                        if (inst.Arg1.Inst.Arg2.Kind == Operand.OpType.Instruction)
                        {
                            // load stuff from array with register 
                            Op = OpCodes.Ldw;
                            PutF1();
                        }
                        else
                        {
                            // load stuff from array with Address 
                            Op = OpCodes.Ldx;
                            PutF2();
                        }
                    }
                    else
                    {
                        Op = OpCodes.Ldw;
                        A = (uint)inst.Reg;
                        B = (uint)inst.Arg1.Val;
                        C = 0;
                        PutF1();
                    }
                    break;
                case IrOps.Store:
                    if ((inst.Arg1.Kind == Operand.OpType.Instruction) && (inst.Arg1.Inst.Op == IrOps.Adda))
                    {
                        A = (uint)inst.Arg1.Val;
                        B = (uint)inst.Arg1.Inst.Arg1.Val;
                        C = (uint)inst.Arg1.Inst.Arg2.Val;

                        if (inst.Arg1.Inst.Arg2.Kind == Operand.OpType.Instruction)
                        {
                            // Store stuff in an array using instructions
                            Op = OpCodes.Stw;
                            PutF1();
                        }
                        else
                        {
                            // else store stuff in an array using an adresss
                            Op = OpCodes.Stx;
                            PutF2();
                        }
                    }
                    else
                    {
                        // Else this is a normal store to a stack variable
                        Op = OpCodes.Stw;
                        A = (uint)inst.Reg;
                        B = (uint)inst.Arg1.Val;
                        C = 0;
                        PutF1();
                    }
                    break;
 
                case IrOps.End:
                    Op = OpCodes.Ret;
                    //A = B = C = 0;
                    PutF2();
                    break;
                case IrOps.Bra:
                    // TODO: this needs work to handle calls
                    Op = OpCodes.Bsr;
                    C = inst.Offset;
                    PutF1();
                    break;
                case IrOps.Bne:
                    MakeBranchInst(OpCodes.Bne, inst);
                    break;
                case IrOps.Beq:
                    MakeBranchInst(OpCodes.Beq, inst);
                    break;
                case IrOps.Ble:
                    MakeBranchInst(OpCodes.Ble, inst);
                    break;
                case IrOps.Blt:
                    MakeBranchInst(OpCodes.Ble, inst);
                    break;
                case IrOps.Bge:
                    MakeBranchInst(OpCodes.Bge, inst);
                    break;
                case IrOps.Bgt:
                    MakeBranchInst(OpCodes.Bgt, inst);
                    break;
                case IrOps.Read:
                    Op = OpCodes.Rdd;
                    A = (uint)inst.Arg1.Val;
                    PutF2();
                    break;
                case IrOps.Write:
                    Op = OpCodes.Wrd;
                    B = (uint)inst.Arg1.Val;
                    PutF2();
                    break;
                case IrOps.WriteNl:
                    Op = OpCodes.Wrl;
                    PutF1();
                    break;
                case IrOps.Move:
                    //emulate a move instruction to copy with an OR operation
                    Op = OpCodes.Or;
                    A = (uint)inst.Arg2.Val;
                    B = (uint)inst.Arg1.Val;
                    C = B;
                    PutF2();
                    break;
                case IrOps.Adda:
                case IrOps.Phi:
                case IrOps.Kill:
                case IrOps.Ssa:
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public OpCodes Op { get; set; }

        public uint A { get; set; }

        public uint B { get; set; }

        public uint C { get; set; }

        public uint MachineCode { get; set; }

        public string Colorname { get; set; }

        public void PutF1()
        {
            MachineCode = 0;
            MachineCode = ((uint)Op << 26) | (A << 21) | (B << 16) | (C & 0xffff);
        }

        public void PutF2()
        {
            MachineCode = 0;
            MachineCode = ((uint)Op << 26) | (A << 21) | (B << 16) | (C & 0x001f);
        }

        public void PutF3()
        {
            MachineCode = 0;
            MachineCode = ((uint)Op << 26) | (C & 0x03ffffff);
        }


        public void ImmediateOperands(OpCodes opCode, Operand arg1, Operand arg2)
        {
            if (arg1.Kind == Operand.OpType.Constant)
            {
                Op = opCode + 16;
                var temp = (uint)arg1.Val;
                B = (uint)arg2.Val;
                C = temp;
            }
            else
            {
                Op = opCode;
                B= (uint)arg1.Val;
                C = (uint)arg2.Val;
            }

        }

        public void MakeBranchInst(OpCodes opCode, Instruction inst)
        {
            Op = opCode;
            A = (uint)inst.Arg1.Inst.Reg;
            C = inst.Arg2.Inst.Offset;
            PutF1();
        }


        public override string ToString()
        {
            return Op + " A: " + A + " B: " + B + " C: " + C;
        }
    }
}
