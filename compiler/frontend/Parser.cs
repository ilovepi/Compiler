using System;

namespace compiler.frontend
{

    public class Parser:IDisposable
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

        private void GetExpected(Token expected)
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

        private void Error(string str)
        {
            //TODO: determine location in file for error messages
            Console.WriteLine("Error Parsing file: " + _filename + ", " + str);
            FatalError();
        }

        private void FatalError(){
            //TODO: determine location in file for error messages
			throw new Exception("Fatal Error Parsing file: " + _filename + ". Unable to continue");
        }


        private void Next() {
            Tok = Scanner.GetNextToken();
        }

        private void Designator() {
            GetExpected(Token.IDENTIFIER);
            GetExpected(Token.OPEN_BRACKET);

            Expression();

            GetExpected(Token.CLOSE_BRACKET);
        }

        private void Factor(){
            if ((Tok == Token.IDENTIFIER) || (Tok == Token.IDENTIFIER))
            {
                Next();
            }
            else {
                FatalError();
            }
        }

        private void Term(){
            
        }
        private void Expression(){
            
        }
        private void Relation(){
            
        }

        private void Assign()
		{

		}

        public void Dispose()
        {
            Scanner.Dispose();
        }


        private void Computation()
        {
        }

        private void Identifier()
        {
        }

        private void Num()
        {
            
        }


        private void VarDecl()
        {
        }


        private void TypeDecl()
        {
        }

        private void FuncDecl()
        {
            
        }

        private void FuncBody()
        {
        }


        private void Statement()
        {
        }


        private void relOp()
        {
        }


        private void FuncCall()
        {
        }

        private void IfStmt()
        {
        }


        private  void WhileStmt()
        {
            
        }



        private void ReturnStmt()
        {
        }

        private void FormalParams()
        { }






        public void parse()
        {
           
            Computation();


            throw new NotImplementedException();
        }
    }
}
