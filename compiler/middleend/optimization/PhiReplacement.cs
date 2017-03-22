using System.Collections.Generic;
using compiler.middleend.ir;

namespace compiler.middleend.optimization
{
    public class PhiReplacement
    {

        private static HashSet<Node> _visited;

        public static void InsertMoveInst(Node root)
        {
            _visited = new HashSet<Node>();
            InsertMove(root);
        }

        private static void InsertMove(Node root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return;
            }

            _visited.Add(root);

            foreach (Instruction instruction in root.Bb.Instructions)
            {
                if (instruction.Op == IrOps.Phi)
                {

                    root.InsertMoveInst(instruction);

                }
            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
                InsertMove(child);
            }
        }

    }
}