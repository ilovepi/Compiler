using System;
using System.Collections.Generic;
using compiler.middleend.ir;
namespace compiler
{
	public class CopyPropagation
	{
		public CopyPropagation()
		{
		}

	    private static HashSet<Node> visited = null;

		public static void Propagate(Node root)
		{
            visited = new HashSet<Node>();
		    PropagateValues(root);
		}

	    private static void PropagateValues(Node root )
	    {
	        if ( (root == null) || visited.Contains(root) )
	        {
	            return;
	        }

	        visited.Add(root);
            
	        foreach (var instruction in root.Bb.Instructions)
	        {
	            if (false)
	            {


	                if (instruction.Arg1.Kind == Operand.OpType.Variable)
	                {
	                    instruction.Arg1 = new Operand(instruction.Arg1.Variable.Location);
	                }

	                if ((instruction.Arg2?.Kind == Operand.OpType.Variable) && (instruction.Op != IrOps.Store))
	                {
	                    instruction.Arg2 = new Operand(instruction.Arg2.Variable.Location);
	                }
	            }
	            else
	            {
                    if (instruction.Arg1.Kind == Operand.OpType.Variable)
                    {
                        instruction.Arg1 = instruction.Arg1.Variable.Value.OpenOperand();
                    }

                    if ((instruction.Arg2?.Kind == Operand.OpType.Variable) && (instruction.Op != IrOps.Store))
                    {
                        instruction.Arg2 = instruction.Arg2.Variable.Value.OpenOperand();
                    }
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
