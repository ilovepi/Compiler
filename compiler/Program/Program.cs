using System.IO;
using compiler.frontend;

namespace Program
{
    internal class Program
    {
        //TODO: adjust main to use the parser when it is complete
        private static void Main(string[] args)
        {

            using (var p = new Parser(@"../../testdata/test003.txt"))
            {
                p.Parse();
                p.FlowCfg.GenerateDOTOutput();

                using (var file = new StreamWriter("graph.txt"))
                {
                    file.WriteLine(p.FlowCfg.DOTOutput);
                }
            }
        }
    }
}