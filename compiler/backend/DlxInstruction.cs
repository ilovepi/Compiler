using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.middleend.ir;

namespace compiler.backend
{
    class DlxInstruction
    {
        public DlxInstruction(Instruction inst)
        {
            A = 0;
            B = 0;
            C = 0;

            switch (inst.Op)
            {
                case IrOps.Add:
                    if ((inst.Arg1.Kind == Operand.OpType.Constant) || (inst.Arg2.Kind == Operand.OpType.Constant))
                    {
                        Op = OpCodes.Addi;
                    }
                    else
                    {
                        Op = OpCodes.Add;
                    }
                    break;
                case IrOps.Sub:
                    if ((inst.Arg1.Kind == Operand.OpType.Constant) || (inst.Arg2.Kind == Operand.OpType.Constant))
                    {
                        Op = OpCodes.Subi;
                    }
                    else
                    {
                        Op = OpCodes.Sub;
                    }
                    break;
                case IrOps.Mul:
                    if ((inst.Arg1.Kind == Operand.OpType.Constant) || (inst.Arg2.Kind == Operand.OpType.Constant))
                    {
                        Op = OpCodes.Muli;
                    }
                    else
                    {
                        Op = OpCodes.Mul;
                    }
                    break;
                case IrOps.Div:
                    if ((inst.Arg1.Kind == Operand.OpType.Constant) || (inst.Arg2.Kind == Operand.OpType.Constant))
                    {
                        Op = OpCodes.Divi;
                    }
                    else
                    {
                        Op = OpCodes.Div;
                    }
                    break;
                case IrOps.Cmp:
                    if ((inst.Arg1.Kind == Operand.OpType.Constant) || (inst.Arg2.Kind == Operand.OpType.Constant))
                    {
                        Op = OpCodes.Cmpi;
                    }
                    else
                    {
                        Op = OpCodes.Cmp;
                    }
                    break;
                case IrOps.Adda:
                    break;
                case IrOps.Load:
                    if ((inst.Arg1.Kind == Operand.OpType.Instruction) && (inst.Arg1.Inst.Op == IrOps.Adda))
                    {
                        A = (uint)inst.Arg1.Val;
                        B = (uint)inst.Arg1.Inst.Arg1.Val;
                        C = (uint)inst.Arg1.Inst.Arg2.Val;

                        if (inst.Arg1.Inst.Arg2.Kind == Operand.OpType.Instruction)
                        {
                            Op = OpCodes.Ldw;
                            PutF1();
                        }
                        else
                        {
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
                            Op = OpCodes.Stw;
                            PutF1();
                        }
                        else
                        {
                            Op = OpCodes.Stx;
                            PutF2();
                        }
                    }
                    else
                    {
                        Op = OpCodes.Stw;
                        A = (uint)inst.Reg;
                        B = (uint)inst.Arg1.Val;
                        C = 0;
                        PutF1();
                    }
                    break;
                case IrOps.Move:
                    break;
                case IrOps.Phi:
                    break;
                case IrOps.End:
                    Op = OpCodes.Ret;
                    //A = B = C = 0;
                    PutF2();
                    break;
                case IrOps.Bra:
                    Op = OpCodes.Bsr;
                    
                    break;
                case IrOps.Bne:
                    break;
                case IrOps.Beq:
                    break;
                case IrOps.Ble:
                    break;
                case IrOps.Blt:
                    break;
                case IrOps.Bge:
                    break;
                case IrOps.Bgt:
                    break;
                case IrOps.Read:
                    break;
                case IrOps.Write:
                    break;
                case IrOps.WriteNl:
                    break;
                case IrOps.Kill:
                    break;
                case IrOps.Ssa:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public OpCodes Op { get; set; }

        public uint A { get; set; }

        public uint B { get; set; }

        public uint C { get; set; }

        public uint MachineCode { get; set; }

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





    }
}
