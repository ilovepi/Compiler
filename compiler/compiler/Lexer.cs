using System;
using System.IO;

namespace compiler
{
    public class Lexer
    {
        public char input;
        StreamReader fr;

        public Lexer(string filename)
        {            
            fr = new StreamReader(filename);

        }


    }
}
