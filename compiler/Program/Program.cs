using System.IO;
using compiler.frontend;

namespace Program
{
    internal class Program
    {
        //TODO: adjust main to use the parser when it is complete
        private static void Main(string[] args)
        {
            using (var p = new Parser(@"../../testdata/test009.txt"))
            {
                p.Parse();
                p.ProgramCfg.GenerateDOTOutput();

                using (var file = new StreamWriter("graph.dot"))
                {
                    file.WriteLine(p.ProgramCfg.DotOutput);
                }
            }
        }
    }
}
