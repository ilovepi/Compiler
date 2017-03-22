#region Basic header

// MIT License
// 
// Copyright (c) 2016 Paul Kirth
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#region

using System;
using compiler.middleend.ir;

#endregion

namespace compiler.backend
{
    public class DlxInstruction
    {
        // Reserved Register Numbers
        public const int Fp = 28;
        public const int Sp = 29;
        public const int Globals = 30;
        public const int RetAddr = 31;

        public FunctionBuilder CalledFunction;

        public OpCodes Op { get; set; }

        public int A { get; set; }

        public int B { get; set; }

        public int C { get; set; }

        public int Address { get; set; }

        public Instruction IrInst { get; set; }

        public uint MachineCode { get; set; }


        public string Colorname { get; set; }


        public DlxInstruction(OpCodes op, int a, int b, int c)
        {
            Op = op;
            A = a;
            B = b;
            C = c;
            PutF1();
        }

        public DlxInstruction(Instruction inst)
        {
            A = inst.Reg;
            B = 0;
            C = 0;

            switch (inst.Op)
            {
                case IrOps.Add:
                    ImmediateOperands(OpCodes.ADD, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Sub:
                    ImmediateOperands(OpCodes.SUB, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Mul:
                    ImmediateOperands(OpCodes.MUL, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Div:
                    ImmediateOperands(OpCodes.DIV, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Cmp:
                    ImmediateOperands(OpCodes.CMP, inst.Arg1, inst.Arg2);
                    break;
                case IrOps.Load:
                    if ((inst.Arg1.Kind == Operand.OpType.Instruction) && (inst.Arg1.Inst.Op == IrOps.Adda))
                    {
                        Instruction addaInst = inst.Arg1.Inst;

                        //A = inst.Arg1.Register;
                        B = addaInst.Arg1.Register;
                        C = addaInst.Arg2.Val*4;
                        addaInst.MachineInst = this;

                        Op = OpCodes.LDW;
                        PutF1();

                        /*
                        if (addaInst.Arg2.Kind == Operand.OpType.Instruction)
                        {
                            // load stuff from array with register 
                            Op = OpCodes.LDW;
                            PutF1();
                        }
                        else
                        {
                            // load stuff from array with Address 
                            Op = OpCodes.LDX;
                            PutF2();
                        }
*/
                    }
                    else
                    {
                        Op = OpCodes.LDW;
                        //A = (int) inst.Reg;
                        VariableType curVariable = inst.VArId;
                        B = curVariable.IsGlobal ? Globals : Fp;
                        //B = inst.Arg1.Register;
                        C = curVariable.Offset;
                        PutF1();
                    }
                    break;
                case IrOps.Store:
                    if ((inst.Arg1.Kind == Operand.OpType.Instruction) && (inst.Arg2.Inst?.Op == IrOps.Adda))
                    {
                        A = inst.Arg1.Register;
                        B = inst.Arg2.Inst.Arg2.Val;
                        C = inst.Arg1.Inst.Arg1.Variable?.Identity.Offset ?? inst.Arg1.Inst.VArId.Offset;
                        //C = inst.Arg1.Val;

                        Op = OpCodes.STW;

                         PutF1();

                        if(false)
                        {
                            if (inst.Arg2.Inst.Arg2.Kind == Operand.OpType.Instruction)
                            {
                                // Store stuff in an array using instructions
                                Op = OpCodes.STW;
                                PutF1();
                            }
                            else
                            {
                                // else store stuff in an array using an adresss
                                Op = OpCodes.STX;
                                PutF2();
                            }
                        }
                    }
                    else if ((inst.Arg1.Kind == Operand.OpType.Register) && (inst.Arg2.Inst?.Op == IrOps.Adda))
                    {
                        A = inst.Arg1.Register;
                        B = inst.Arg2.Inst.Arg1.Register;
                        C = inst.Arg2.Inst.Arg2.Val*4;

                        //C = inst.Arg1.Val;


                        Op = OpCodes.STW;

                        PutF1();

                        if (false)
                        {
                            if (inst.Arg2.Inst.Arg2.Kind == Operand.OpType.Instruction)
                            {
                                // Store stuff in an array using instructions
                                Op = OpCodes.STW;
                                PutF1();
                            }
                            else
                            {
                                // else store stuff in an array using an adresss
                                Op = OpCodes.STX;
                                PutF2();
                            }
                        }
                    }
                    else
                    {
                        // Else this is a normal store to a stack variable
                        Op = OpCodes.STW;
                        A = inst.Reg;
                        B = inst.Arg1.Register;
                        C = inst.Arg2.Register;
                        PutF1();
                    }
                    break;

                case IrOps.End:
                    Op = OpCodes.RET;
                    A = B = C = 0;
                    PutF2();
                    break;
                case IrOps.Ret:
                    Op = OpCodes.RET;
                    A = B = C = 0;
                    C = inst.Arg1.Val;
                    PutF2();
                    break;

                case IrOps.Bra:
                    Op = OpCodes.BEQ;
                    A = 0;
                    C = inst.Offset;
                    PutF1();
                    break;
                case IrOps.Bne:
                    MakeBranchInst(OpCodes.BNE, inst);
                    break;
                case IrOps.Beq:
                    MakeBranchInst(OpCodes.BEQ, inst);
                    break;
                case IrOps.Ble:
                    MakeBranchInst(OpCodes.BLE, inst);
                    break;
                case IrOps.Blt:
                    MakeBranchInst(OpCodes.BLE, inst);
                    break;
                case IrOps.Bge:
                    MakeBranchInst(OpCodes.BGE, inst);
                    break;
                case IrOps.Bgt:
                    MakeBranchInst(OpCodes.BGT, inst);
                    break;
                case IrOps.Read:
                    Op = OpCodes.RDD;
                    //A = inst.Arg1.Val;
                    PutF2();
                    break;
                case IrOps.Write:
                    Op = OpCodes.WRD;
                    A = 0;
                    B = inst.Arg1.Register;
                    PutF2();
                    break;
                case IrOps.WriteNl:
                    A = B = C = 0;
                    Op = OpCodes.WRL;
                    PutF1();
                    break;
                case IrOps.Move:
                    //emulate a move instruction to copy with an OR operation
                    Op = OpCodes.AND;
                    A = inst.Arg2.Register;
                    B = inst.Arg1.Register;
                    C = B;
                    PutF2();
                    break;
                case IrOps.Call:
                    A = 0;
                    Op = OpCodes.JSR;
                    // TODO: This value must be the actual addres of the function, fix it later
                    C = inst.Offset;
                    PutF3();
                    break;
                case IrOps.Adda:
                case IrOps.Phi:
                case IrOps.Kill:
                case IrOps.Ssa:
                case IrOps.Neg:
                default:
                    throw new ArgumentOutOfRangeException();
            }

            IrInst = inst;
            IrInst.MachineInst = this;
        }

        public void PutF1()
        {
            MachineCode = 0;
            MachineCode = (uint) (((int) Op << 26) | (A << 21) | (B << 16) | (C & 0xffff));
        }

        public void PutF2()
        {
            MachineCode = 0;
            MachineCode = (uint) (((int) Op << 26) | (A << 21) | (B << 16) | (C & 0x001f));
        }

        public void PutF3()
        {
            MachineCode = 0;
            MachineCode = (uint) (((int) Op << 26) | (C & 0x03ffffff));
        }


        public void ImmediateOperands(OpCodes opCode, Operand arg1, Operand arg2)
        {
            if (arg1.Kind == Operand.OpType.Constant)
            {
                // thse cannot have their argument ordering switched
                if ((opCode == OpCodes.SUB) || (opCode == OpCodes.DIV) || (opCode == OpCodes.CMP))
                {
                    Op = opCode;
                    B = arg1.Register;
                    C = arg2.Register;
                    PutF2();
                }
                else
                {
                    // arg 2 has the register value
                    Op = opCode + 16;
                    B = arg2.Register;
                    C = arg1.Val;
                    PutF1();
                }
            }
            else
            {
                B = arg1.Register;
                if (B == 0)
                {
                    B = arg1.Inst.Reg;
                }

                // then arg might have the constant, and B is the register for sure
                if (arg2.Kind == Operand.OpType.Constant)
                {
                    Op = opCode + 16;
                    C = arg2.Val;
                    PutF1();
                }
                else
                {
                    Op = opCode;
                    C = arg2.Register;
                    PutF2();
                }
            }
        }

        public void MakeBranchInst(OpCodes opCode, Instruction inst)
        {
            Op = opCode;
            A = inst.Arg1.Register;
            B = 0;
            C = inst.Arg2.Inst.Offset;
            PutF1();
        }


        public override string ToString()
        {
            return Op + " - A: " + A + " - B: " + B + " - C: " + C + " - Code: " + Convert.ToString(MachineCode, 2);
        }
    }
}