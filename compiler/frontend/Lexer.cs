using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace compiler.frontend
{
    class Lexer
    {
        StreamReader sr;
        public char c;

        public Lexer(string filename)
        {
            try
            {
                sr = new StreamReader(filename);
            }
            catch(FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
            //next();

        }

        ~Lexer()
        {
            if(sr != null)
            {
                sr.Close();
                sr = null;
            }
        }

        public char next()
        {
            if(sr.Peek() == -1)
            {
                throw new Exception("Error: Lexer cannot read beyond the end of the file");
            }
            c = (char)sr.Read();
            return c;
        }


    }
}
