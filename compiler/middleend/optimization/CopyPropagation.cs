using System.Collections.Generic;
using compiler.middleend.ir;

namespace compiler.middleend.optimization
{
	public class CopyPropagation
	{
		public CopyPropagation()
		{
		}

	    private static HashSet<Node> _visited = null;

		public static void Propagate(Node root)
		{
            _visited = new HashSet<Node>();
		    PropagateValues(root);
		}

	    private static void PropagateValues(Node root )
	    {
	        if ( (root == null) || _visited.Contains(root) )
	        {
	            return;
	        }

	        _visited.Add(root);
            
	        foreach (var instruction in root.Bb.Instructions)
	        {
				if (instruction.Arg1?.Kind == Operand.OpType.Variable)
				{
					instruction.Arg1 = instruction.Arg1.Variable.Value.OpenOperand();
				}

				if ((instruction.Arg2?.Kind == Operand.OpType.Variable) && (instruction.Op != IrOps.Store))
				{
					instruction.Arg2 = instruction.Arg2.Variable.Value.OpenOperand();
				}
                
	        }

	        var children = root.GetAllChildren();

	        foreach (var child in children)
	        {
	            PropagateValues(child);
	        }
	    }
	}
}
