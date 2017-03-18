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
        }

        public DlxInstruction(Instruction inst)
        {
            A = 0;
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
                        A = inst.Arg1.Val;
                        B = inst.Arg1.Inst.Arg1.Val;
                        C = inst.Arg1.Inst.Arg2.Val;
                        inst.Arg1.Inst.MachineInst = this;
                        if (inst.Arg1.Inst.Arg2.Kind == Operand.OpType.Instruction)
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
                    }
                    else
                    {
                        Op = OpCodes.LDW;
                        A = (int) inst.Reg;
                        B = inst.Arg1.Val;
                        C = 0;
                        PutF1();
                    }
                    break;
                case IrOps.Store:
                    if ((inst.Arg1.Kind == Operand.OpType.Instruction) && (inst.Arg2.Inst?.Op == IrOps.Adda))
                    {
                        A = inst.Arg1.Val;
                        B = inst.Arg2.Inst.Arg2.Val;
                        C = inst.Arg1.Inst.Arg1.Val;
                        //C = inst.Arg1.Val;

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
                    else
                    {
                        // Else this is a normal store to a stack variable
                        Op = OpCodes.STW;
                        A = (int) inst.Reg;
                        B = inst.Arg1.Val;
                        C = 0;
                        PutF1();
                    }
                    break;

                case IrOps.End:
                    Op = OpCodes.RET;
                    //A = B = C = 0;
                    PutF2();
                    break;
                case IrOps.Ret:
                    Op = OpCodes.RET;
                    //A = B = C = 0;
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
                    A = inst.Arg1.Val;
                    PutF2();
                    break;
                case IrOps.Write:
                    Op = OpCodes.WRD;
                    B = inst.Arg1.Val;
                    PutF2();
                    break;
                case IrOps.WriteNl:
                    Op = OpCodes.WRL;
                    PutF1();
                    break;
                case IrOps.Move:
                    //emulate a move instruction to copy with an OR operation
                    Op = OpCodes.AND;
                    A = inst.Arg2.Val;
                    B = inst.Arg1.Val;
                    C = B;
                    PutF2();
                    break;
                case IrOps.Call:
                    Op = OpCodes.JSR;
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
                Op = opCode + 16;
                var temp = arg1.Val;
                B = arg2.Val;
                C = temp;
            }
            else
            {
                Op = arg2.Kind == Operand.OpType.Constant ? opCode + 16 : opCode;
                B = arg1.Val;
                C = arg2.Val;
            }
        }

        public void MakeBranchInst(OpCodes opCode, Instruction inst)
        {
            Op = opCode;
            A = (int) inst.Arg1.Inst.Reg;
            C = inst.Arg2.Inst.Offset;
            PutF1();
        }


        public override string ToString()
        {
            return Op + " A: " + A + " B: " + B + " C: " + C;
        }
    }
}