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
        string _filename;

        public Parser(string pFileName)
        {
            _filename = pFileName;
            t = Token.UNKNOWN;
            s = new Lexer(_filename);
        }

        public void GetExpected(Token expected)
        {
            if (t == expected)
            {
                Next();
            }
            else {
				Error("Error in file: " + _filename + " at line " + s.LineNo + ", pos " + s.Position +
					  "\n\tFound: " + TokenHelper.toString(t) + " but Expected: " + 
				      TokenHelper.toString(expected));
            }
        }

        public void Error(string str)
        {
            //TODO: determine location in file for error messages
            Console.WriteLine ("Error Parsing file: " + _filename + ", " + str);
            FatalError();
        }

        public void FatalError(){
            //TODO: determine location in file for error messages
			throw new Exception("Fatal Error Parsing file: " + _filename + ". Unable to continue");
        }


        public void Next() {
            t = s.GetNextToken();
        }

        public void Designator() {
            GetExpected(Token.IDENTIFIER);
            GetExpected(Token.OPEN_BRACKET);

            Expression();

            GetExpected(Token.CLOSE_BRACKET);
        }

        public void Factor(){
            if ((t == Token.IDENTIFIER) || (t == Token.IDENTIFIER))
            {
                Next();
            }
            else {
                FatalError();
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
