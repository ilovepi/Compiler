using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using compiler.frontend;

namespace Program
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer l = new Lexer(@"..\..\testdata\big.txt");
            Token t;
            do
            {
                t = l.getNextToken();
                Console.WriteLine( TokenHelper.printToken(t) );

            } while (t != Token.EOF);

            // necessary when testing on windows with visual studio
            //Console.WriteLine("Press 'enter' to exit ....");
            //Console.ReadLine();
        }
    }
}
