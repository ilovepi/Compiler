using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.middleend.ir;

namespace compiler.middleend.optimization
{
    class CleanUpSsa
    {

        private static HashSet<Node> _visited;

        public static void Clean(Node root)
        {
            _visited = new HashSet<Node>();
            CleanConstSsa(root);
        }

        private static void CleanConstSsa(Node root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return;
            }

            _visited.Add(root);

            foreach (Instruction instruction in root.Bb.Instructions)
            {
                if (instruction.Op == IrOps.Ssa)
                {
                    if (instruction.Arg1.Kind == Operand.OpType.Constant)
                    {
                        instruction.Op = IrOps.Add;
                        instruction.Arg2 = new Operand(Operand.OpType.Constant, 0);
                    }
                }
            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
                CleanConstSsa(child);
            }
        }




    }
}
