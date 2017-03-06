using System.Collections.Generic;
using compiler.middleend.ir;

namespace compiler.middleend.optimization
{
    public class CsElimination
    {
        public static void Eliminate(Node root)
        {
            var visited = new HashSet<Node>();
            Eliminate(root, visited);
        }


        public static void Eliminate(Node root, HashSet<Node> visited)
        {
            if ((root == null) || visited.Contains(root))
            {
                return;
            }

            visited.Add(root);
            var removalList = new List<Instruction>();

            var delayed = new List<Instruction>();

            foreach (Instruction bbInstruction in root.Bb.Instructions)
            {
                if ((bbInstruction.Op != IrOps.Phi) )
                {
                    if (bbInstruction.Op == IrOps.Load)
                    {
                        //TODO fix cse for loop headers

                        if (root.GetType() == typeof(WhileNode))
                        {
                            // determine if we can replace it later
                            delayed.Add(bbInstruction);
                            continue;
                        }
                        //continue;
                    }
                    else if(bbInstruction.Op == IrOps.Store)
                    {
                        // insert kill instruction for all loads
                        root.Bb.AnchorBlock.InsertKill(bbInstruction.Arg2);
                        
                    }

                    EliminateInternal(root, bbInstruction, removalList, false);
                }
            }

            // can't mutate a list while we're iterating through it so delay removal till here
            foreach (Instruction instruction in removalList)
            {
                //root.Bb.AnchorBlock.FindOpChain(instruction.Op).RemoveAll(instruction.ExactMatch);
                root.Bb.Instructions.RemoveAll(instruction.ExactMatch);

				//rely on using instruction hashkey for removing a particular instruction
				//root.Bb.Graph.RemoveVertex(instruction);
            }


            List<Node> children = root.GetAllChildren();
            foreach (Node child in children)
            {
                Eliminate(child, visited);
            }

            // TODO: fix loop cse
            foreach (Instruction instruction in delayed)
            {
                EliminateInternal(root,instruction,removalList, true);
            }
        }

        private static void EliminateInternal(Node root, Instruction bbInstruction, List<Instruction> removalList, bool alternate)
        {
            
            Instruction predecessor;
            if (alternate)
            {
                predecessor = root.AnchorSearch(bbInstruction, alternate);
            }
            else
            {
                predecessor = root.AnchorSearch(bbInstruction);
            }


            if (predecessor != null)
            {
                // thi check is probably redundant now that we're rebuilding the search structure
                if (predecessor.Num != bbInstruction.Num)
                {
                    // delay removal from list -- because c# says so
                    bbInstruction.ReplaceInst(predecessor);
                    removalList.Add(bbInstruction);
                }
            }
            else
            {
                root.Bb.AnchorBlock.Insert(bbInstruction);
            }
        }
    }
}