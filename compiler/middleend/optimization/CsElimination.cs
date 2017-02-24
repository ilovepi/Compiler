using System.Collections.Generic;
using compiler.middleend.ir;
using NUnit.Framework;

namespace compiler.middleend.optimization
{
    public class CsElimination
    {
        public static void Eliminate(Node root)
        {
            if (root == null)
                return;
            List<Instruction> removalList = new List<Instruction>();

            foreach (Instruction bbInstruction in root.Bb.Instructions)
            {
                if (bbInstruction.Op != IrOps.Phi)
                {
                    var predecessor = root.AnchorSearch(bbInstruction);

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

            // can't mutate a list while we're iterating through it so delay removal till here
            foreach (Instruction instruction in removalList)
            {
                root.Bb.Instructions.RemoveAll(instruction.ExactMatch);
                root.Bb.AnchorBlock.FindOpChain(instruction.Op).RemoveAll(instruction.ExactMatch);
            }


            var children = root.GetAllChildren();
            foreach (Node child in children)
            {
                Eliminate(child);
            }
        }
    }



}