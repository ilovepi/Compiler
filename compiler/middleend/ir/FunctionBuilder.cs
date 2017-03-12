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
using compiler.backend;

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


        public List<Instruction> GetInstructions(DominatorNode root)
        {
            List<Instruction> lastBlock = null;
            List<Instruction> intermediateBlocks = new List<Instruction>();
            if (root == null)
            {
                return intermediateBlocks;
            }

            List<Instruction> myInstructions = new List<Instruction>(root.Bb.Instructions);

            var singlebeBlock = true;
            foreach (DominatorNode child in root.Children)
            {
                if (lastBlock == null)
                {
                    lastBlock = GetInstructions(child);
                }
                else
                {
                    singlebeBlock = false;
                    intermediateBlocks.AddRange(GetInstructions(child));
                }
            }

            if (lastBlock != null)
            {
                if (singlebeBlock)
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
                    var dlx = new DlxInstruction(instruction);
                    MachineBody.Add(dlx);
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

        public void Prologue()
        {
            // allocate memory for a return value
            // push current ret address onto stack
            // save sp and FP to stack
            // load each param into register and push onto stack
            // allocate memory for all local variables
            // save any global variable that might have be modified in function
        }


        public void Epilogue()
        {
            // save return value back on stack, or in a register if that works
            // save any globals variables that might have been modified
            // pop all the locals off the stack
            // pop all the parameters off the stack
            // restore the Stack pointer
            // restore the Frame pointer
        }
    }
}