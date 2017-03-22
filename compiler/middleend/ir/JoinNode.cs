﻿#region Basic header

// MIT License
// 
// Copyright (c) 2016 Paul Kirth
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#region

using System.Collections.Generic;

#endregion

namespace compiler.middleend.ir
{
    public class JoinNode : Node
    {
        public Node FalseParent { get; set; }

        public JoinNode(BasicBlock pBb) : base(pBb, NodeTypes.JoinB)
        {
            Colorname = "coral";
            FalseParent = null;
        }


        public override void CheckEnqueue(Cfg cfg)
        {
            cfg.BfsCheckEnqueue(this, Child);
        }


        public override void Consolidate(HashSet<Node> visited, bool isLoop)
        {
            if (visited.Contains(this))
            {
                return;
            }
            visited.Add(this);

            CircularRef(Child);

            // consolidate children who exist
            Child?.Consolidate(visited, isLoop);
        }


        public override Instruction AnchorSearch(Instruction goal)
        {
            Instruction trueBranch = null;
            Instruction falseBranch = null;

            Instruction res = Bb.Search(goal);

            if (res != null)
            {
                return res;
            }


            if (Parent != null)
            {
                trueBranch = Parent.AnchorSearch(goal);
            }

            if (FalseParent != null)
            {
                falseBranch = FalseParent.AnchorSearch(goal);
            }

            if (falseBranch != trueBranch)
            {
                return null;
            }


            //TODO: this is wrong we need to figure out how to do this for a join block.
            return null;
        }

        public override Instruction AnchorSearch(Instruction goal, bool alternate)
        {
            Instruction trueBranch = null;
            Instruction falseBranch = null;

            Instruction res = Bb.Search(goal);

            if (res != null)
            {
                return res;
            }


            if (Parent != null)
            {
                trueBranch = Parent.AnchorSearch(goal, alternate);
            }

            if (FalseParent != null)
            {
                falseBranch = FalseParent.AnchorSearch(goal, alternate);
            }

            if (falseBranch == trueBranch)
            {
                return trueBranch;
            }


            //TODO: this is wrong we need to figure out how to do this for a join block.
            return null;
        }


        public override void InsertMoveInst(Instruction phiInst)
        {
            PhiOperand(phiInst, phiInst.Arg1, Parent);
            PhiOperand(phiInst, phiInst.Arg2, FalseParent);
        }

        private void PhiOperand(Instruction phiInst, Operand arg, Node parentNode)
        {
            if ((arg == null) || (arg.Inst == null))
            {
                return;
            }

            var phiOperand = new Operand(phiInst);
            var target =arg.Inst.Op == IrOps.Ssa ? arg.Inst.Arg1 : arg;

            if (phiInst.Reg != target.Register)
            {
                parentNode.InsertMove(target, phiOperand);
            }
           
        }

        public override void InsertMove(Operand src, Operand dest)
        {
            var moveinst = new Instruction(IrOps.Move, src, dest);
            Bb.Instructions.Add(moveinst);
        }


    }
}