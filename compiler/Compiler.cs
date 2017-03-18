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
using System.IO;
using System.Linq;
using compiler.frontend;
using compiler.middleend.ir;
using compiler.middleend.optimization;

#endregion

namespace compiler
{
    public class Compiler
    {
        public List<ParseTree> FuncList;

        public List<FunctionBuilder> DlxFunctions { get; set; }

        public CompilerOptions Opts { get; }

        public Compiler(CompilerOptions pOptions)
        {
            Opts = pOptions;
        }

        public void Parse()
        {
            using (var p = new Parser(Opts.InputFilename, Opts.CopyProp))
            {
                FuncList = new List<ParseTree>();
                p.Parse();

                foreach (Cfg func in p.FunctionsCfgs)
                {
                    func.Sym = p.Scanner.SymbolTble;
                    // create dominator Tree
                    DomTree dom = DominatorNode.ConvertCfg(func);

                    // add CFG and DomTree to the functin list
                    FuncList.Add(new ParseTree(func, dom));
                }
            }
        }

        public void GenerateGraphOutput()
        {
            if (!Opts.GraphOutput)
            {
                return;
            }

            using (var file = new StreamWriter(Opts.CfgFilename))
            {
                file.WriteLine("digraph G{\n");
                file.WriteLine(GenControlGraphString());
                file.WriteLine("}");
            }

            using (var file = new StreamWriter(Opts.DomFilename))
            {
                file.WriteLine("digraph Dom{\n");
                file.WriteLine(GenDomGraphString());
                file.WriteLine("}");
            }


            // quickgraph makes the files for us, so no using statement here
            GenInterferenceGraphString();


            using (var file = new StreamWriter(Opts.DomFilename + ".code"))
            {
                file.WriteLine("digraph Dom{\n");
                file.WriteLine(GenInstructionListGraphString());
                file.WriteLine("}");
            }

            using (var file = new StreamWriter(Opts.DomFilename + ".dlx"))
            {
                file.WriteLine("digraph DlxCode{\n");
                file.WriteLine(GenDlxGraphString());
                file.WriteLine("}");
            }
        }

        private string GenDlxGraphString()
        {
            string ret = string.Empty;
            var i = 0;
            return DlxFunctions.Aggregate(ret,
                (current, functionBuilder) => current + (functionBuilder.PrintFunction(i++) + "\n"));
        }

        private string GenInstructionListGraphString()
        {
            List<ParseTree> straightFuncs = GenStraightLineFunctions();
            var i = 0;
            return straightFuncs.Aggregate(string.Empty,
                (current, func) => current + (func.DominatorTree.PrintTreeGraph(i++, func.ControlFlowGraph.Sym) + "\n"));
        }

        private List<ParseTree> GenStraightLineFunctions()
        {
            var straightFuncList = new List<ParseTree>();
            PopulateDlxFunc();
            foreach (var dlxFunc in DlxFunctions)
            {
                DomTree dom = new DomTree
                {
                    Root = new DominatorNode(new BasicBlock("StatSequence",1)) {Bb = {Instructions = dlxFunc.FuncBody}}
                };
                straightFuncList.Add(new ParseTree(dlxFunc.Tree.ControlFlowGraph, dom));
                dom.Name = dlxFunc.Tree.ControlFlowGraph.Name;
                dom.Root.Colorname = dlxFunc.Tree.ControlFlowGraph.Root.Colorname;
            }
            return straightFuncList;
        }

        private void PopulateDlxFunc()
        {
            DlxFunctions = new List<FunctionBuilder>();
            foreach (ParseTree parseTree in FuncList)
            {
                FunctionBuilder newFunction = new FunctionBuilder(parseTree);
                DlxFunctions.Add(newFunction);
            }
            foreach (var func in DlxFunctions)
            {
                func.TransformDlx(DlxFunctions);
            }
            AssignAddresses();
        }

        private string GenDomGraphString()
        {
            var i = 0;
            return FuncList.Aggregate(string.Empty,
                (current, func) => current + (func.DominatorTree.PrintTreeGraph(i++, func.ControlFlowGraph.Sym) + "\n"));
        }

        private string GenInterferenceGraphString()
        {
            return FuncList.Aggregate(string.Empty,
                (current, parseTree) => current + (parseTree.DominatorTree.PrintInterference() + "\n"));
        }

        private string GenControlGraphString()
        {
            var i = 0;
            var s = string.Empty;
            foreach (var func in FuncList)
            {
                func.ControlFlowGraph.GenerateDotOutput(i++);
                s += func.ControlFlowGraph.DotOutput + "\n";
            }
            return s;
        }

        public void Optimize()
        {
            // iterate through each CFG and do the optimizations.
            foreach (ParseTree func in FuncList)
            {
                //if (Opts.PruneCfg)
                {
                    bool restart;
                    do
                    {
                        restart = TransformIr(func, false);
                    } while (restart);
                }
                TransformIr(func, true);

                func.ControlFlowGraph.InsertBranches();
                LiveRanges.GenerateRanges(func.DominatorTree);
            }
        }

        private bool TransformIr(ParseTree func, bool cleanSsa)
        {
            bool restart = false;
            // Copy propagation
            if (Opts.CopyProp)
            {
                CopyPropagation.Propagate(func.ControlFlowGraph.Root);
                CopyPropagation.ConstantFolding(func.ControlFlowGraph.Root);
            }

            // replace ssa asignments with add instructions
            if (cleanSsa)
            {
                CleanUpSsa.Clean(func.ControlFlowGraph.Root);
            }

            //Common Sub Expression Elimination
            if (Opts.Cse)
            {
                CsElimination.Eliminate(func.ControlFlowGraph.Root);
            }

            // Reevaluation
            if (Opts.DeadCode)
            {
                DeadCodeElimination.RemoveDeadCode(func.ControlFlowGraph.Root);
            }

            // Pruning
            if (Opts.PruneCfg)
            {
                restart = Prune.StartPrune(func.ControlFlowGraph.Root);
                func.ControlFlowGraph.Root.Consolidate();
                func.DominatorTree = DominatorNode.ConvertCfg(func.ControlFlowGraph);
            }
            return restart;
        }

        public void RegisterAllocation()
        {
            if (!Opts.RegAlloc)
            {
                return;
            }

            foreach (ParseTree parseTree in FuncList)
            {
                var intGraph = parseTree.DominatorTree.IntGraph;
                var newIntGraph = new InterferenceGraph();
                HashSet<Instruction> visited = new HashSet<Instruction>();

                foreach (var vertex in intGraph.Vertices)
                {
                    //newIntGraph.AddVerticesAndEdgeRange( (new InterferenceGraph(intGraph.PhiGlobber(vertex, visited))).Edges );
                    //intGraph.Color();
                }

               // parseTree.DominatorTree.IntGraph = newIntGraph;
               intGraph.Color();
            }
        }


        public void InstructionScheduling()
        {
            if (!Opts.InstSched)
            {
                return;
            }

            throw new NotImplementedException();
        }


        public void CodeGeneration()
        {
            if (!Opts.CodeGen)
            {
                return;
            }

            RegisterAllocation();
            InstructionScheduling();

            //lower representation to machine code
            GenStraightLineFunctions();


            throw new NotImplementedException();
        }

        public void AssignAddresses()
        {
            int baseAddr = 0;
            foreach (FunctionBuilder functionBuilder in DlxFunctions)
            {
                functionBuilder.AssignAddresses(baseAddr);
                baseAddr += functionBuilder.CodeSize;
            }
        }


        public void GenerateOutput()
        {
            GenerateGraphOutput();
            CodeGeneration();
        }

        public void GenerateTestOutput()
        {
            if (!Opts.GraphOutput)
            {
                return;
            }
            GenControlGraphString();
            GenDomGraphString();
            GenInterferenceGraphString();
            GenStraightLineFunctions();
            GenInstructionListGraphString();
            GenDlxGraphString();
        }


        private static CompilerOptions DefaultOpts(string pFilename)
        {
            //TODO: make sure we set this in the CLI parser
            var opts = new CompilerOptions
            {
                // default options
                InputFilename = pFilename,
                OutputFilename = "output.txt",
                CfgFilename = "graph.dot",
                DomFilename = "Dominator.dot",
                GraphOutput = true,
                CopyProp = true,
                Cse = true,
                DeadCode = true,
                PruneCfg = false,
                RegAlloc = true,
                InstSched = false,
                CodeGen = false
            };
            return opts;
        }

        public static void DefaultRun(string pFilename)
        {
            CompilerOptions opts = DefaultOpts(pFilename);
            var c = new Compiler(opts);
            c.Parse();
            c.Optimize();
            c.RegisterAllocation();
            c.GenerateOutput();
        }

        public static void TestRun(string pFilename, bool copyprop, bool cse, bool deadcode, bool prune)
        {
            //TODO make this a commandline program with args parsing
            CompilerOptions opts = DefaultOpts(pFilename);
            opts.CopyProp = copyprop;
            opts.Cse = cse;
            opts.DeadCode = deadcode;
            opts.PruneCfg = prune;
            var c = new Compiler(opts);
            c.Parse();
            c.Optimize();
            c.RegisterAllocation();
            c.GenerateTestOutput();
            
        }
    }
}