using System;
using compiler.frontend;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer l = new Lexer(@"../../testdata/big.txt");
            Token t;
            do
            {
                t = l.GetNextToken();
                Console.WriteLine( TokenHelper.printToken(t) );

            } while (t != Token.EOF);

            // necessary when testing on windows with visual studio
            //Console.WriteLine("Press 'enter' to exit ....");
            //Console.ReadLine();
        }
    }
}
