using System;
using compiler.frontend;

namespace Program
{
    class Program
    {
        //TODO: adjust main to use the parser when it is complete
        static void Main(string[] args)
        {
            using (Lexer l = new Lexer(@"../../testdata/big.txt"))
            {
                Token t;
                do
                {
                    t = l.GetNextToken();
                    Console.WriteLine(TokenHelper.PrintToken(t));

                } while (t != Token.EOF);

                // necessary when testing on windows with visual studio
                //Console.WriteLine("Press 'enter' to exit ....");
                //Console.ReadLine();
            }
        }
    }
}
