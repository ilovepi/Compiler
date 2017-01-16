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
        char c;

        Lexer(string filename)
        {
            sr = new StreamReader(filename);

            next();

        }

        ~Lexer()
        {
            if(sr != null)
            {
                sr.Close();
                sr = null;
            }
        }

        public void next()
        {
            if(sr.Peek() == -1)
            {
                throw new Exception("Error: Lexer cannot read beyond the end of the file");
            }
            c = (char)sr.Read();
        }
    }
}
