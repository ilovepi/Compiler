using System.IO;
using compiler.frontend;

namespace Program
{
    internal class Program
    {
        //TODO: adjust main to use the parser when it is complete
        private static void Main(string[] args)
        {
            using (var p = new Parser(@"../../testdata/test024.txt"))
            {
                p.Parse();
                p.ProgramCfg.GenerateDOTOutput();

                using (var file = new StreamWriter("graph.txt"))
                {
                    file.WriteLine(p.ProgramCfg.DOTOutput);
                }
            }
        }
    }
}
