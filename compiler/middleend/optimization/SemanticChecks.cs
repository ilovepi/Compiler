using System.Collections.Generic;
using System.Linq;
using compiler.frontend;
using compiler.middleend.ir;

namespace compiler.middleend.optimization
{
    internal static class SemanticChecks
    {

        private static HashSet<DominatorNode> _visited;

        public static void RunChecks(DominatorNode root)
        {
            _visited = new HashSet<DominatorNode>();
            Validate(root);
        }

        private static void Validate(DominatorNode root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return;
            }

            _visited.Add(root);
            //foreach (Instruction instruction in root.Bb.Instructions)
            {
                //DefUse(instruction, root);
                

            }

            CheckPhi(root);



            foreach (var child in root.Children)
            {
                Validate(child);
            }

        }


        public static void CheckPhi( DominatorNode root)
        {
            if ((root.Bb.NodeType == Node.NodeTypes.BB) || (root.Bb.NodeType == Node.NodeTypes.CompareB))
            {
                //return;
            }

            foreach (Instruction target in root.Bb.Instructions)
            {
                if (target.Op == IrOps.Phi)
                {
                    if ((target.Arg1.Inst == null) || (target.Arg2.Inst == null))
                    {
                        if (target.UsesLocations.Any(root.SearchDominated))
                        {
                            throw new ParserException("Variable Uninitialized before use:");
                        }
                    }
                }
            }
            
        }


        private static void BadPhiArg(Operand arg)
        {
            if ((arg.Kind == Operand.OpType.Instruction) && (arg.Inst == null))
            {
                throw new ParserException("Variable Uninitialized before use:");
            }
        }

        private static void CheckUseDef(Operand arg)
        {
            if ((arg?.Kind == Operand.OpType.Instruction) && (arg.Inst?.Op == IrOps.Phi))
            {
                BadPhiArg(arg.Inst.Arg1);
                BadPhiArg(arg.Inst.Arg2);
            }
        }

        private static void DefUse(Instruction inst, DominatorNode root)
        {
            switch (inst.Op)
            {
                //no checks
                case IrOps.Kill:
                case IrOps.End:
                case IrOps.Read:
                case IrOps.Phi:
                case IrOps.Bra:
                case IrOps.Bne:
                case IrOps.Beq:
                case IrOps.Ble:
                case IrOps.Blt:
                case IrOps.Bge:
                case IrOps.Bgt:
                case IrOps.Write:
                case IrOps.WriteNl:
                case IrOps.Call:
                    break;

                //second
                case IrOps.Ret:
                    CheckUseDef(inst.Arg2);
                    break;

                //both
                case IrOps.Store:
                case IrOps.Neg:
                case IrOps.Add:
                case IrOps.Sub:
                case IrOps.Mul:
                case IrOps.Div:
                case IrOps.Cmp:
                case IrOps.Adda:
                    CheckUseDef(inst.Arg1);
                    CheckUseDef(inst.Arg2);
                    break;

                //first
                case IrOps.Move:

                case IrOps.Load:
                    CheckUseDef(inst.Arg1);
                    break;

                case IrOps.Ssa:
                    CheckUseDef(inst.Arg1);
                    BadPhiArg(inst.Arg1);
                    break;
            }
        }
    }
}