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
using compiler.backend;

#endregion

namespace compiler.middleend.ir
{
    public class FunctionBuilder
    {
        /// <summary>
        ///     The Control Flow Graph, Dominator Tree, & Interference Graph of the function
        /// </summary>
        public ParseTree Tree { get; set; }


        /// <summary>
        ///     The flat list of inorder IR instructions
        /// </summary>
        public List<Instruction> FuncBody { get; set; }

        /// <summary>
        ///     The Machine code translation of the function body
        /// </summary>
        public List<DlxInstruction> MachineBody { get; set; }


        /// <summary>
        ///     The function name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The function's base address
        /// </summary>
        public int Address { get; set; }

        /// <summary>
        ///     The size of the function's call frame
        /// </summary>
        public int FrameSize { get; set; }

        public int CodeSize => (MachineBody?.Count * 4) ?? 0;

        public List<VariableType> Globals => Tree.ControlFlowGraph.Globals;
        public HashSet<VariableType> UsedGlobals => Tree.ControlFlowGraph.UsedGlobals;
        public List<VariableType> Params => Tree.ControlFlowGraph.Parameters;
        public List<VariableType> Locals => Tree.ControlFlowGraph.Locals;

        /// <summary>
        ///     constructor
        /// </summary>
        /// <param name="parseTree">The parse tree of the function</param>
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


            // Scale number of vars by size of int;
            //FrameSize *= 4;
        }


        /// <summary>
        ///     Converts the CFG into a flat list of instructions
        /// </summary>
        /// <param name="root">The root of the dominator tree</param>
        /// <returns>A list of IR instructions</returns>
        public List<Instruction> GetInstructions(DominatorNode root)
        {
            List<Instruction> lastBlock = null;
            var intermediateBlocks = new List<Instruction>();
            if (root == null)
            {
                return intermediateBlocks;
            }

            var myInstructions = new List<Instruction>(root.Bb.Instructions);

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


        /// <summary>
        ///     Transforms IR instructions into DLX instructions
        /// </summary>
        /// <param name="functionBuilders">The list of all functions translated as function builders</param>
        public void TransformDlx(List<FunctionBuilder> functionBuilders)
        {
            MachineBody = new List<DlxInstruction>();
            foreach (Instruction instruction in FuncBody)
            {
                try
                {
                    if (instruction.Op == IrOps.Call)
                    {
                        FunctionBuilder func = functionBuilders.Find(curr => curr.Name == instruction.Arg1.Name);
                        List<DlxInstruction> prologue = Prologue(instruction, func.Tree);
                        MachineBody.AddRange(prologue);
                        var dlx = new DlxInstruction(instruction);
                        MachineBody.Add(dlx);
                        dlx.CalledFunction = func;
                        dlx.IrInst.MachineInst = prologue.First();

                        Instruction retInst =
                            func.Tree.ControlFlowGraph.Root.Leaf()
                                .Bb.Instructions.Find(
                                    current => (current.Op == IrOps.Ret) || (current.Op == IrOps.End));

                        MachineBody.AddRange(Epilogue(retInst.Arg2?.Val ?? 0, instruction, func.Tree));
                    }
                    else
                    {

                        if (instruction.Op == IrOps.Store)
                        {
                            if (instruction.Arg1.Kind == Operand.OpType.Constant)
                            {
                                var scratch = 27;
                                // load the constant into memory using the scratch register
                                MachineBody.Add(new DlxInstruction(OpCodes.ADDI, scratch, 0 ,instruction.Arg1.Val));
                                instruction.Arg1.Kind = Operand.OpType.Register;
                                instruction.Arg1.Register = scratch;

                            }
                        }

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


        /// <summary>
        ///     Creates a label for graphviz's dot language
        /// </summary>
        /// <returns>label string for an instruction entry in a graph node</returns>
        public string DotLabel()
        {
            string label = Name;
            var slot = 0;
            foreach (DlxInstruction inst in MachineBody)
            {
                label += " \\l| <i" + slot++ + ">" + inst;
            }

            return label;
        }


        /// <summary>
        ///     creates a node for graphviz
        /// </summary>
        /// <returns>string rep of a node</returns>
        public string PrintGraphNode()
        {
            string local = string.Empty;
            local += Name + "[label=\"{" + DotLabel() + "\\l}\",fillcolor=" + "khaki" + "]\n";

            return local;
        }


        /// <summary>
        ///     creates a graph viz subgraph
        /// </summary>
        /// <param name="n">cluster ID number</param>
        /// <returns></returns>
        public string PrintFunction(int n)
        {
            string graphOutput = "subgraph cluster_" + n + " {\nlabel = \"" + Name +
                                 "\";\n node[style=filled,shape=record]\n" +
                                 PrintGraphNode() + "}";
            return graphOutput;
        }

        /// <summary>
        ///     generates a function prologue for caller saved values
        /// </summary>
        /// <param name="calliInstruction">The instruction calling the function</param>
        /// <param name="tree">The parse tree of the functon being called</param>
        /// <returns>A list of DLX machine instructions the comprise the prologue</returns>
        public List<DlxInstruction> Prologue(Instruction calliInstruction, ParseTree tree)
        {
            var prologue = new List<DlxInstruction>();

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
            for (var i = 0; i < tree.DominatorTree.NumReg; i++)
            {
                prologue.Add(new DlxInstruction(OpCodes.PSH, i, DlxInstruction.Sp, 4));
            }


            // load each param into register and push onto stack
            foreach (Operand param in Enumerable.Reverse(calliInstruction.Parameters))
            {

                int regNo;
                if (param.Inst == null)
                {
                    regNo = 27;
                    prologue.Add(new DlxInstruction(OpCodes.ADDI, regNo, 0, param.Val));

                }
                else
                {
                    regNo =  param.Inst.Reg;
                }

                prologue.Add(new DlxInstruction(OpCodes.PSH, regNo, DlxInstruction.Sp, 4));
            }


            var size = 0;
            // allocate memory for all local variables
            foreach (VariableType variableType in Tree.ControlFlowGraph.Locals)
            {
                size += variableType.Size;
            }

            if (size > 0)
            {
                prologue.Add(new DlxInstruction(OpCodes.ADDI, DlxInstruction.Sp, DlxInstruction.Sp, size));
            }

            prologue.Add(new DlxInstruction(OpCodes.ADDI, DlxInstruction.Fp, DlxInstruction.Sp, 0));
            // save any global variable that might have be modified in function
            foreach (VariableType variableType in tree.ControlFlowGraph.UsedGlobals)
            {
                // get instruction from liverange
                IEnumerable<Instruction> good =
                    calliInstruction.LiveRange.Where(current => current.Arg1.IdKey == variableType.Id);
                Instruction[] instructions = good as Instruction[] ?? good.ToArray();
                if (instructions.Length != 0)
                {
                    prologue.Add(new DlxInstruction(OpCodes.STW, instructions.First().Reg, DlxInstruction.Globals,
                        variableType.Offset));
                }
            }

            return prologue;
        }


        /// <summary>
        ///     Generates a function epilogue for caller saved functions
        /// </summary>
        /// <param name="retValReg">The register number of the return value</param>
        /// <param name="calliInstruction">The instuction doing the call</param>
        /// <param name="tree">the parse tree of the called function</param>
        /// <returns>A list of DLX machine instructions the comprise the epilogue</returns>
        public List<DlxInstruction> Epilogue(int retValReg, Instruction calliInstruction, ParseTree tree)
        {
            var localSize = 0;

            // calculate memory for all local variables
            foreach (VariableType variableType in tree.ControlFlowGraph.Locals)
            {
                localSize += variableType.Size;
            }

            var eplilogue = new List<DlxInstruction>();

            // save return value back on stack, or in a register if that works
            var retInst = new DlxInstruction(OpCodes.STX, retValReg, DlxInstruction.Sp,
                -4 *
                (3 + tree.DominatorTree.NumReg + calliInstruction.Parameters.Count +
                 Tree.ControlFlowGraph.Parameters.Count));
            retInst.PutF2();
            eplilogue.Add(retInst);


            // save any global variable that might have be modified in function
            foreach (VariableType variableType in tree.ControlFlowGraph.UsedGlobals)
            {
                // get instruction from liverange
                IEnumerable<Instruction> good =
                    calliInstruction.LiveRange.Where(
                        current => (current.VArId?.Id ?? current.Arg1.IdKey) == variableType.Id);
                Instruction[] instructions = good as Instruction[] ?? good.ToArray();
                if (instructions.Length != 0)
                {
                    eplilogue.Add(new DlxInstruction(OpCodes.STW, instructions.First().Reg, DlxInstruction.Globals,
                        variableType.Address));
                }
            }

            // pop all the locals off the stack
            // pop all the parameters off the stack
            if ((tree.ControlFlowGraph.Parameters.Count + localSize) > 0)
            {
                eplilogue.Add(new DlxInstruction(OpCodes.SUBI, DlxInstruction.Sp, DlxInstruction.Sp,
                    (4 * tree.ControlFlowGraph.Parameters.Count) + localSize));
            }


            // restore old registers
            for (var i = 0; i < Tree.DominatorTree.NumReg; i++)
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


            // deallocate memory for a return value
            var newVal = new DlxInstruction(OpCodes.POP, calliInstruction.Reg, DlxInstruction.Sp, -4);
            eplilogue.Add(newVal);

            return eplilogue;
        }


        public void AssignAddresses(int baseAddr)
        {
            Address = baseAddr;

            // all params are ints 

            var i = 0;
            foreach (VariableType parameter in Params)
            {

                parameter.Offset = i;
                i -= parameter.Size * 4;
            }

            // locals can be arrays or ints

            i = 0;
            foreach (VariableType local in Locals)
            {
                local.Offset = i;
                i += local.Size * 4;
            }


            for (var j = 0; j < MachineBody.Count; j++)
            {
                MachineBody[j].Address = j;
            }
        }

        public void FixInstructions()
        {
            foreach (DlxInstruction inst in MachineBody)
            {
                FixInstructions(inst);
            }
        }


        public void FixInstructions(DlxInstruction inst)
        {
            if (inst.IrInst == null)
            {
                return;
            }

            switch (inst.Op)
            {
                case OpCodes.BEQ:
                case OpCodes.BNE:
                case OpCodes.BLT:
                case OpCodes.BGE:
                case OpCodes.BLE:
                case OpCodes.BGT:
                    int targetOffset;
                    if ((inst.IrInst.Op == IrOps.Bra) || (inst.IrInst.Op == IrOps.Ssa))
                    {
                        targetOffset = inst.IrInst.Arg1.Inst.MachineInst.Address;
                    }
                    else
                    {
                        Instruction arg = inst.IrInst.Arg2.Inst;
                        if (arg.Op == IrOps.Ssa)
                        {
                            targetOffset = inst.IrInst.Arg1.Inst.MachineInst.Address;
                        }
                        else
                        {
                            targetOffset = arg.MachineInst.Address;
                        }
                    }

                    inst.C = targetOffset - inst.Address;
                    inst.PutF1();
                    break;
                case OpCodes.JSR:
                    inst.C = inst.CalledFunction.Address;
                    inst.PutF3();
                    break;
                case OpCodes.ADD:
                case OpCodes.SUB:
                case OpCodes.MUL:
                case OpCodes.DIV:
                case OpCodes.CMP:
                    inst.ImmediateOperands(inst.Op, inst.IrInst.Arg1, inst.IrInst.Arg2);
                    break;
                case OpCodes.ADDI:
                case OpCodes.SUBI:
                case OpCodes.MULI:
                case OpCodes.DIVI:
                case OpCodes.CMPI:
                    inst.ImmediateOperands(inst.Op - 16, inst.IrInst.Arg1, inst.IrInst.Arg2);
                    break;
                case OpCodes.LDW:
                case OpCodes.LDX:
                    break;
                case OpCodes.POP:
                    break;
                case OpCodes.STW:
                    break;
                case OpCodes.STX:
                    break;
                case OpCodes.PSH:
                    break;
                case OpCodes.BSR:
                    break;
                case OpCodes.RET:
                    break;
                case OpCodes.RDD:
                    break;
                case OpCodes.WRD:
                    //inst.B = inst.IrInst.Arg1.Variable.Location.Reg;
                    break;
                case OpCodes.WRH:
                    break;
                case OpCodes.WRL:
                    break;
                default:
                    return;
            }
        }
    }
}