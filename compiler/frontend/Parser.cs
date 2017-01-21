using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.frontend;

namespace compiler.frontend
{

    class Parser
    {
        public Token t;
        public Lexer s;
        string filename;
        int lineno;
        int pos;

        public void getExpected(Token expected)
        {
            if (t == expected)
            {
                next();
            }
            else {
                error();
            }
        }

        public void error(string str)
        {
            //TODO: determine location in file for error messages
            Console.WriteLine ("Error Parsing file: " + filename + ", " + str);
            error_fatal();
        }

        public void error_fatal(){
            //TODO: determine location in file for error messages
            throw new Exception("Fatal Error Parsing file: " + filename + ". Unable to continue";
        }


        public void next() {
            t = s.getNextToken();
        }

        public void Designator() {
            getExpected(Token.IDENTIFIER);
            getExpected(Token.OPEN_BRACKET);

            Expression();

            getExpected(Token.CLOSE_BRACKET);
        }

        public void Factor(){
            if ((t == Token.IDENTIFIER) || (t == Token.IDENTIFIER))
            {
                next();
            }
            else {
                error();
            }
        }

        public void Term(){
            
        }
        public void Expression(){
            
        }
        public void Relation(){
            
        }
			                    
		public void Assign()
		{

		}

    }
}
