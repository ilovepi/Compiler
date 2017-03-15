#region Basic header

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using compiler.backend;
using NUnit.Framework;

#endregion

namespace compiler.middleend.ir
{
    public class FunctionBuilder
    {
        public FunctionBuilder(ParseTree parseTree)
        {
            Tree = parseTree;
            Name = Tree.ControlFlowGraph.Name;
            FrameSize = 0;
            foreach (VariableType variableType in Tree.ControlFlowGraph.Parameters)
            {
                FrameSize += variableType.Size;
            }

            foreach (VariableType variableType in Tree.ControlFlowGraph.Locals)
            {
                FrameSize += variableType.Size;
            }

            FuncBody = GetInstructions(parseTree.DominatorTree.Root);

            TransformDlx();
            // Scale number of vars by size of int;
            //FrameSize *= 4;
        }

        public ParseTree Tree { get; set; }
        public List<Instruction> FuncBody { get; set; }

        public List<DlxInstruction> MachineBody { get; set; }


        public string Name { get; set; }

        public int Address { get; set; }

        public int FrameSize { get; set; }

        public int CodeSize => (MachineBody?.Count * 4) ?? 0;


        public List<Instruction> GetInstructions(DominatorNode root)
        {
            List<Instruction> lastBlock = null;
            List<Instruction> intermediateBlocks = new List<Instruction>();
            if (root == null)
            {
                return intermediateBlocks;
            }

            List<Instruction> myInstructions = new List<Instruction>(root.Bb.Instructions);

            var singleBlock = true;
            foreach (DominatorNode child in root.Children)
            {
                if (lastBlock == null)
                {
                    lastBlock = GetInstructions(child);
                }
                else
                {
                    singleBlock = false;
                    intermediateBlocks.AddRange(GetInstructions(child));
                }
            }

            if (lastBlock != null)
            {
                if (singleBlock)
                {
                    intermediateBlocks = lastBlock;
                }
                else
                {
                    intermediateBlocks.AddRange(lastBlock);
                }
            }
            myInstructions.AddRange(intermediateBlocks);

            return myInstructions;
        }


        private void TransformDlx()
        {
            MachineBody = new List<DlxInstruction>();
            foreach (Instruction instruction in FuncBody)
            {
                try
                {
                    if (instruction.Op == IrOps.Call)
                    {
                        MachineBody.AddRange(Prologue(instruction));
                        var dlx = new DlxInstruction(instruction);
                        MachineBody.Add(dlx);
                        var retInst =
                            Tree.ControlFlowGraph.Root.Leaf().Bb.Instructions.Find((current) => (current.Op == IrOps.Ret) || (current.Op == IrOps.End));
                        
                        
                        
                        MachineBody.AddRange(Epilogue(retInst.Arg2?.Val ?? 0,instruction));

                    }
                    else
                    {
                        var dlx = new DlxInstruction(instruction);
                        MachineBody.Add(dlx);
                    }
                }
                catch (ArgumentOutOfRangeException e)
                {
                    // consider emiting a debugging statement here
                    //throw e;
                }
            }
        }


        public string DotLabel()
        {
            string label = Name;
            var slot = 0;
            foreach (var inst in MachineBody)
            {
                label += " \\l| <i" + slot++ + ">" + inst;
            }

            return label;
        }


        public string PrintGraphNode()
        {
            string local = string.Empty;
            local += Name + "[label=\"{" + DotLabel() + "\\l}\",fillcolor=" + "khaki" + "]\n";

            return local;
        }


        public string PrintFunction(int n)
        {
            string graphOutput = "subgraph cluster_" + n + " {\nlabel = \"" + Name +
                                 "\";\n node[style=filled,shape=record]\n" +
                                 PrintGraphNode() + "}";

            return graphOutput;
        }

        public List<DlxInstruction> Prologue(Instruction calliInstruction)
        {
            List<DlxInstruction> prologue = new List<DlxInstruction>();
           
            // allocate memory for a return value
            var retVal = new DlxInstruction(OpCodes.PSH, 0, DlxInstruction.Sp, 4);
            prologue.Add(retVal);

            // push current ret address onto stack
            var retAddr = new DlxInstruction(OpCodes.PSH, DlxInstruction.RetAddr, DlxInstruction.Sp, 4);
            prologue.Add(retAddr);


            // save sp and FP to stack
            var oldSp = new DlxInstruction(OpCodes.PSH, DlxInstruction.Fp, DlxInstruction.Sp, 4);
            var oldFp = new DlxInstruction(OpCodes.PSH, DlxInstruction.Sp, DlxInstruction.Sp, 4);

            prologue.Add(oldSp);
            prologue.Add(oldFp);

            // save registers required
            for (int i = 0; i < Tree.DominatorTree.NumReg; i++)
            {
                prologue.Add(new DlxInstruction(OpCodes.PSH, i, DlxInstruction.Sp, 4));
            }


            // load each param into register and push onto stack
            foreach (var param in calliInstruction.Parameters)
            {
                var regNo = param.Inst == null ? (int) param.Val : (int)param.Inst.Reg;
                prologue.Add(new DlxInstruction(OpCodes.PSH,  regNo, DlxInstruction.Sp, 4));
            }


            int size = 0;
            // allocate memory for all local variables
            foreach (VariableType variableType in Tree.ControlFlowGraph.Locals)
            {
                size += (int)variableType.Size;
            }

            if (size > 0)
            {
                prologue.Add(new DlxInstruction(OpCodes.ADDI, DlxInstruction.Sp, DlxInstruction.Sp, size));
            }

            // save any global variable that might have be modified in function
            foreach (VariableType variableType in Tree.ControlFlowGraph.UsedGlobals)
            {
                // get instruction from liverange
                var good = calliInstruction.LiveRange.First((current) => current.VArId.Id == variableType.Id);
                
                
                prologue.Add(new DlxInstruction(OpCodes.STW, (int)good.Reg , DlxInstruction.Globals, variableType.Address));
            }

            return prologue;
 ;       }


        public List<DlxInstruction> Epilogue(int retValReg, Instruction calliInstruction)
        {

            int localSize = 0;
            // calculate memory for all local variables
            foreach (VariableType variableType in Tree.ControlFlowGraph.Locals)
            {
                localSize += (int)variableType.Size;
            }

            List<DlxInstruction> eplilogue = new List<DlxInstruction>();

            // save return value back on stack, or in a register if that works
            var retInst = new DlxInstruction(OpCodes.STX, (int)retValReg , DlxInstruction.Sp, (-4*(3 + Tree.DominatorTree.NumReg + calliInstruction.Parameters.Count + Tree.ControlFlowGraph.Parameters.Count)));
            eplilogue.Add(retInst);

            
            // save any global variable that might have be modified in function
            foreach (VariableType variableType in Tree.ControlFlowGraph.UsedGlobals)
            {
                // get instruction from liverange
                var good = calliInstruction.LiveRange.First((current) => current.VArId.Id == variableType.Id);
                eplilogue.Add(new DlxInstruction(OpCodes.STW, (int)good.Reg, DlxInstruction.Globals, variableType.Address));
            }

            // pop all the locals off the stack
            if (localSize > 0)
            {
                eplilogue.Add(new DlxInstruction(OpCodes.SUBI, DlxInstruction.Sp, DlxInstruction.Sp, localSize));
            }

            // pop all the parameters off the stack
            if (Tree.ControlFlowGraph.Parameters.Count > 0)
            {
                eplilogue.Add(new DlxInstruction(OpCodes.SUBI, DlxInstruction.Sp, DlxInstruction.Sp, 4*Tree.ControlFlowGraph.Parameters.Count));
            }


            // restore old registers
            for (int i = 0; i < Tree.DominatorTree.NumReg; i++)
            {
                eplilogue.Add(new DlxInstruction(OpCodes.POP, i, DlxInstruction.Sp, -4));
            }

            
            // restore the Frame pointer
            var oldFp = new DlxInstruction(OpCodes.POP, DlxInstruction.Sp, DlxInstruction.Sp, -4);
            eplilogue.Add(oldFp);

            // restore the Stack pointer
            var oldSp = new DlxInstruction(OpCodes.POP, DlxInstruction.Fp, DlxInstruction.Sp, -4);
            eplilogue.Add(oldSp);

            // pop current ret address off of the stack
            var retAddr = new DlxInstruction(OpCodes.POP, DlxInstruction.RetAddr, DlxInstruction.Sp, -4);
            eplilogue.Add(retAddr);


            // allocate memory for a return value
            var newVal = new DlxInstruction(OpCodes.POP, (int)calliInstruction.Reg, DlxInstruction.Sp, -4);
            eplilogue.Add(newVal);

            return eplilogue;
        }
    }
}