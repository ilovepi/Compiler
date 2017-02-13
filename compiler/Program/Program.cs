using System.IO;
using compiler.frontend;

namespace Program
{
    internal class Program
    {
        //TODO: adjust main to use the parser when it is complete
        private static void Main(string[] args)
        {
//            using (Lexer l = new Lexer(@"../../testdata/big.txt"))
//            {
//                Token t;
//                do
//                {
//                    t = l.GetNextToken();
//                    Console.WriteLine(TokenHelper.PrintToken(t));
//
//                } while (t != Token.EOF);
//
//                // necessary when testing on windows with visual studio
//                //Console.WriteLine("Press 'enter' to exit ....");
//                //Console.ReadLine();
//            }

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