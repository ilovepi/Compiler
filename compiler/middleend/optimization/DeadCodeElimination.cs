using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.middleend.ir;

namespace compiler.middleend.optimization
{
    class DeadCodeElimination
    {

        private static HashSet<Node> _visited;

        public static void RemoveDeadCode(Node root)
        {
            _visited = new HashSet<Node>();
            RemoveDead(root);
        }

        private static void RemoveDead(Node root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return;
            }

            _visited.Add(root);

            List<Instruction> removalList = new List<Instruction>();
            foreach (Instruction instruction in root.Bb.Instructions)
            {
                switch (instruction.Op)
                {
                    case IrOps.Store:
                    case IrOps.Move:
                    case IrOps.Phi:
                    case IrOps.End:
                    case IrOps.Bra:
                    case IrOps.Bne:
                    case IrOps.Beq:
                    case IrOps.Ble:
                    case IrOps.Blt:
                    case IrOps.Bge:
                    case IrOps.Bgt:
                    case IrOps.Ret:
                    case IrOps.Read:
                    case IrOps.Write:
                    case IrOps.WriteNl:
                        break;
                    case IrOps.Neg:
                    case IrOps.Add:
                    case IrOps.Sub:
                    case IrOps.Mul:
                    case IrOps.Div:
                    case IrOps.Cmp:
                    case IrOps.Adda:
                    case IrOps.Load:
                    default:
                        if (instruction.Uses.Count == 0)
                        {
                            removalList.Add(instruction);
                        }
                        break;

                }
            }

            foreach (Instruction instruction in removalList)
            {
                root.Bb.Instructions.Remove(instruction);
            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
                RemoveDead(child);
            }
        }

    }
}
