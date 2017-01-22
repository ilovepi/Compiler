using System;

namespace compiler.frontend
{

    class Parser:IDisposable
    {
        public Token Tok { get; set; }
        public Lexer Scanner { get; }
        private readonly string _filename;

        public Parser(string pFileName)
        {
            _filename = pFileName;
            Tok = Token.UNKNOWN;
            Scanner = new Lexer(_filename);
        }

        public void GetExpected(Token expected)
        {
            if (Tok == expected)
            {
                Next();
            }
            else {
				Error("Error in file: " + _filename + " at line " + Scanner.LineNo + ", pos " + Scanner.Position +
					  "\n\tFound: " + TokenHelper.ToString(Tok) + " but Expected: " + 
				      TokenHelper.ToString(expected));
            }
        }

        public void Error(string str)
        {
            //TODO: determine location in file for error messages
            Console.WriteLine("Error Parsing file: " + _filename + ", " + str);
            FatalError();
        }

        public void FatalError(){
            //TODO: determine location in file for error messages
			throw new Exception("Fatal Error Parsing file: " + _filename + ". Unable to continue");
        }


        public void Next() {
            Tok = Scanner.GetNextToken();
        }

        public void Designator() {
            GetExpected(Token.IDENTIFIER);
            GetExpected(Token.OPEN_BRACKET);

            Expression();

            GetExpected(Token.CLOSE_BRACKET);
        }

        public void Factor(){
            if ((Tok == Token.IDENTIFIER) || (Tok == Token.IDENTIFIER))
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

        public void Dispose()
        {
            Scanner?.Dispose();
        }
    }
}
