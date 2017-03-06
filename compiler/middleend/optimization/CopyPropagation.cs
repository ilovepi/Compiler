using System.Collections.Generic;
using System.Runtime.CompilerServices;
using compiler.middleend.ir;

namespace compiler.middleend.optimization
{
    public class CopyPropagation
    {
        private static HashSet<Node> _visited;

        public static void Propagate(Node root)
        {
            _visited = new HashSet<Node>();
            PropagateValues(root);
        }

        private static void PropagateValues(Node root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return;
            }

            _visited.Add(root);

            foreach (Instruction instruction in root.Bb.Instructions)
            {
                if (instruction.Arg1?.Kind == Operand.OpType.Variable)
                {
                    instruction.Arg1 = instruction.Arg1.OpenOperand();
                    //instruction.Arg1 = instruction.Arg1.Variable.Value.OpenOperand();
                }

				if ((instruction.Arg2?.Kind == Operand.OpType.Variable) && (instruction.Op != IrOps.Ssa))
                {
                    instruction.Arg2 = instruction.Arg2.Variable.Value.OpenOperand();
                }
            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
                PropagateValues(child);
            }
        }

        public static void ConstantFolding(Node root)
        {
            _visited = new HashSet<Node>();
            FoldConstantValues(root, _visited);
        }


		private static void FoldConstantValues(Node root, HashSet<Node> visited)
		{
			if ((root == null) || visited.Contains(root))
			{
				return;
			}

			visited.Add(root);
			var removalList = new List<Instruction>();

			for (int i = 0; i < root.Bb.Instructions.Count; i++)
			{
				var bbInstruction = root.Bb.Instructions[i];
				if ((bbInstruction.Op != IrOps.Phi) &&(bbInstruction.Op != IrOps.Load))
				{
					if ((bbInstruction.Arg1.Kind == Operand.OpType.Constant) && (bbInstruction.Arg2?.Kind == Operand.OpType.Constant))
					{
						FoldValue(bbInstruction, removalList);
					}
				}
				else if (bbInstruction.Op == IrOps.Cmp)
				{
					FoldComparison();
				}
			}

			// can't mutate a list while we're iterating through it so delay removal till here
			foreach (Instruction instruction in removalList)
			{
				//root.Bb.AnchorBlock.FindOpChain(instruction.Op).RemoveAll(instruction.ExactMatch);
				root.Bb.Instructions.RemoveAll(instruction.ExactMatch);
			}


			List<Node> children = root.GetAllChildren();
			foreach (Node child in children)
			{
				FoldConstantValues(child, visited);
			}
		}



		private static void FoldValue(Instruction inst, List<Instruction> removalList)
		{
			int result;
			switch (inst.Op)
			{
				case IrOps.Add:
					result = inst.Arg1.Val + inst.Arg2.Val;
					break;
				case IrOps.Sub:
					result = inst.Arg1.Val - inst.Arg2.Val;
					break;
				case IrOps.Mul:
					result = inst.Arg1.Val * inst.Arg2.Val;
					break;
				case IrOps.Div:
					result = inst.Arg1.Val / inst.Arg2.Val;
					break;
				default:
					return;
			}

			inst.FoldConst(result);
			removalList.Add(inst);

		}

		public static void FoldComparison()
		{
			
		}





    }
}