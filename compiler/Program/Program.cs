using System.IO;
using compiler;
using compiler.frontend;

namespace Program
{
    internal class Program
    {
        //TODO: adjust main to use the parser when it is complete
        private static void Main(string[] args)
        {
            using (var p = new Parser(@"../../testdata/test022.txt"))
            {
                p.Parse();
                

                using (var file = new StreamWriter("graph.dot"))
                {
                    //*
                    file.WriteLine("digraph G{\n");                    
                    int i = 0;
                    foreach (CFG func in p.FunctionsCfgs)
                    {
                        func.Sym = p.Scanner.SymbolTble;
                        func.GenerateDOTOutput(i++);
                        file.WriteLine(func.DotOutput);
                    }
                    file.WriteLine("\n}");
                    //*
                    /*
                    file.WriteLine("digraph G{\n");
                    int i = 0;
                   
                       p.ProgramCfg.Sym = p.Scanner.SymbolTble;
                        p.ProgramCfg.GenerateDOTOutput(i++);
                        file.WriteLine(p.ProgramCfg.DotOutput);
                   
                    file.WriteLine("\n}");
                    */
                }
            }
        }
    }
}
