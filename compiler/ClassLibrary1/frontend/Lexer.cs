using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace compiler.frontend
{    
    public class Lexer
    {
        public char input;
        StreamReader fr;

        Lexer(string filename)
        {
            fr = new StreamReader(filename);
        }

        ~Lexer()
        {
            if(fr != null)
            {
                fr.Close();
                fr = null;
            }
        }


        public void next()
        {
            int ret = fr.Read();
            if(ret == -1)
            {
                throw new Exception("Error: Scanner cannot read beyond the end of the file.");
            }

            input = (char)ret;
        }

    }
    
}
