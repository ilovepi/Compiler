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


		public void getExpected(Token expected)
		{
			if (t == expected)
			{
				next();
			}
			error();
		}

		public void error(string str)
		{
			//TODO: determine location in file
			Console.WriteLine ("Error Parsing file: " + filename + ", " + str);
			error_fatal();
		}

		public void error_fatal(){
			//TODO: determine location in file
			throw new Exception("Fatal Error Parsing file: " + filename + ". Unable to continue";
		}


        public void next() {
            t = s.getNextToken();
        }

        public void Designator() {
            if (t == Token.IDENTIFIER)
			{
                next();
            }
            else error();

			if (t == Token.OPEN_BRACKET)
			{
				next();
			}
			else error();

			while (t != Token.CLOSE_BRACKET)
			{
				Expression();
				next();
			}

        }
        public void Factor(){
			if ((t == Token.IDENTIFIER) || (t == Token.IDENTIFIER))
			{
				next();
			}
			else error();
        }
        public void Term(){
            
        }
        public void Expression(){
            
        }
        public void Relation(){
            
        }

    }
}
