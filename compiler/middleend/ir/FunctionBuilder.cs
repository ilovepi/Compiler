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
// 
// 
// 
// Created on:  03 10, 2017

#endregion

using System.Collections.Generic;
using System.Linq;

namespace compiler.middleend.ir
{
    public class FunctionBuilder
    {
        public ParseTree Tree { get; set; }
        public List<Instruction> FuncBody { get; set; }

        public string Name { get; set; }

        public int Address { get; set; }

        public int FrameSize { get; set; }

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
    }
}