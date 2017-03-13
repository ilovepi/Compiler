using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.middleend.ir;

namespace compiler.middleend.optimization
{
    class Prune
    {
        private static HashSet<Node> _visited;

        public static bool StartPrune(Node root)
        {
            _visited = new HashSet<Node>();
            return PruneBranches(root);
        }

        private static bool PruneBranches(Node root)
        {
            if ((root == null) || _visited.Contains(root))
            {
                return false;
            }

            _visited.Add(root);

            bool mutatedGraph = false;

            if (root.GetType() == typeof(CompareNode))
            {
               
                var instList = root.Bb.Instructions;
                Instruction cmp = instList.Find((current) => current.Op == IrOps.Cmp);

                if ((cmp?.Arg1.OpenOperand().Kind == Operand.OpType.Constant) && (cmp?.Arg2.OpenOperand().Kind == Operand.OpType.Constant))
                {
                    var branch = instList.Last();
                    bool takeBranch;
                    switch (branch.Op)
                    {
                        case IrOps.Bne:
                            takeBranch = cmp.Arg1.Val != cmp.Arg2.Val;
                            break;
                        case IrOps.Beq:
                            takeBranch = cmp.Arg1.Val == cmp.Arg2.Val;
                            break;
                        case IrOps.Ble:
                            takeBranch = cmp.Arg1.Val <= cmp.Arg2.Val;
                            break;
                        case IrOps.Blt:
                            takeBranch = cmp.Arg1.Val < cmp.Arg2.Val;
                            break;
                        case IrOps.Bge:
                            takeBranch = cmp.Arg1.OpenOperand().Val >= cmp.Arg2.OpenOperand().Val;
                            break;
                        case IrOps.Bgt:
                            takeBranch = cmp.Arg1.Val > cmp.Arg2.Val;
                            break;
                        default:
                            throw new Exception("Comparison cannot be evaluated!");
                    }

                    CompareNode compareNode = (CompareNode)root;
                    Node begin = compareNode.Parent;
                    JoinNode join = compareNode.Join;
                    Node joinParent = takeBranch ? join.FalseParent : join.Parent;
                    Node end = join.Child;
                    Node keep = takeBranch ? compareNode.FalseNode : compareNode.Child;

                    // propagate phi results
                    foreach (Instruction bbInstruction in join.Bb.Instructions)
                    {
                        if (takeBranch)
                        {
                            bbInstruction.ReplaceInst(bbInstruction.Arg2.Inst);
                            
                        }
                        else
                        {
                            bbInstruction.ReplaceInst(bbInstruction.Arg1.Inst);
                        }
                    }


                    // check to see if we have a real else branch or a regular if
                    // only required to check if the evaluation is always false
                    if (takeBranch && (compareNode.Join == compareNode.FalseNode))
                    {
                        
                        begin.Child = end;
                        end.Parent = begin;
                    }
                    else
                    {
                        // move nodes around
                        begin.Child = keep;
                        keep.Parent = begin;

                        joinParent.Child = end;
                        end.Parent = joinParent;
                    }

                    //compareNode.Parent = null;

                    
                    //begin.Consolidate();

                    PruneBranches(begin.Child);
                    return true;
                }

            }

            List<Node> children = root.GetAllChildren();

            foreach (Node child in children)
            {
               mutatedGraph  = (mutatedGraph || PruneBranches(child));
            }

            return mutatedGraph;
        }

    }
}
