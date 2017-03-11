using System.Collections.Generic;
using System.Linq;

namespace compiler.middleend.ir
{
    public class Anchor
    {
        public Anchor()
        {
            Oplist = new List<List<Instruction>>();
        }

        public List<List<Instruction>> Oplist { get; set; }

        /// <summary>
        ///     Insert the specified inst. into the Anchor lists
        /// </summary>
        /// <param name="inst">Inst.</param>
        public void Insert(Instruction inst)
        {
            IrOps key = inst.Op;

            List<Instruction> chain = FindOpChain(key);

            // if the op never existed, add it
            if (chain == null)
            {
                chain = new List<Instruction>();
                Oplist.Add(chain);
            }

            // insert the new instruction at the bottom of the list
            chain.Add(inst);
        }

        public List<Instruction> FindOpChain(IrOps key)
        {
            foreach (List<Instruction> sublist in Oplist)
            {
                if ((sublist.Count > 0) && (sublist.First().Op == key))
                {
                    return sublist;
                }
            }

            return null;
        }

        public void InsertKill(Operand target)
        {
            var chain = FindOpChain(IrOps.Load);
            // if the op never existed, add it
            if (chain == null)
            {
                chain = new List<Instruction>();
                Oplist.Add(chain);
            }

            chain.Add(new Instruction(IrOps.Kill, target, null));
        }
    }
}