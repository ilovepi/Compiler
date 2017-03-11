using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using compiler.frontend;
using compiler.middleend.ir;
using compiler.middleend.optimization;

namespace compiler
{
    public class Compiler
    {
        public List<ParseTree> FuncList;


        public Compiler(CompilerOptions pOptions)
        {
            Opts = pOptions;
        }

        public CompilerOptions Opts { get; }

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

            using (var file = new StreamWriter(Opts.DomFilename + ".interference"))
            {
                //file.WriteLine("digraph Dom{\n");
                file.WriteLine(GenInterferenceGraphString());
                //file.WriteLine("}");
            }
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
                (current, func) => current + (func.DominatorTree.PrintInterference() + "\n"));
        }

        private string GenControlGraphString()
        {
            var i = 0;
            String s = string.Empty;
            foreach (ParseTree func in FuncList)
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
                // Copy propagation
                if (Opts.CopyProp)
                {
                    CopyPropagation.Propagate(func.ControlFlowGraph.Root);
                    CopyPropagation.ConstantFolding(func.ControlFlowGraph.Root);
                }

                //Common Sub Expression Elimination
                if (Opts.Cse)
                {
                    CsElimination.Eliminate(func.ControlFlowGraph.Root);
                }


                // Reevaluation
                if (Opts.DeadCode)
                {
                    throw new NotImplementedException();
                }

                // Pruning
                if (Opts.PruneCfg)
                {
                    throw new NotImplementedException();
                }

                func.ControlFlowGraph.InsertBranches();

                LiveRanges.GenerateRanges(func.DominatorTree);
            }
        }

        public void RegisterAllocation()
        {
            if (!Opts.RegAlloc)
            {
                return;
            }

            throw new NotImplementedException();
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

            throw new NotImplementedException();
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
                DeadCode = false,
                PruneCfg = false,
                RegAlloc = false,
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
            c.GenerateOutput();
        }

        public static void TestRun(string pFilename)
        {
            //TODO make this a commandline program with args parsing
            CompilerOptions opts = DefaultOpts(pFilename);
            var c = new Compiler(opts);
            c.Parse();
            c.Optimize();
            c.GenerateTestOutput();
        }
    }
}