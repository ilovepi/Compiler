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
            switch (inst.Op)
            {
                case IrOps.Add:
                    if(inst.Arg1 == constant)
                    break;
                case IrOps.Sub:
                    break;
                case IrOps.Mul:
                    break;
                case IrOps.Div:
                    break;
                case IrOps.Cmp:
                    break;
                case IrOps.Adda:
                    break;
                case IrOps.Load:
                    break;
                case IrOps.Store:
                    break;
                case IrOps.Move:
                    break;
                case IrOps.Phi:
                    break;
                case IrOps.End:
                    break;
                case IrOps.Bra:
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
