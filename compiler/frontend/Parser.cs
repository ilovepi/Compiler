using System;
using System.Runtime.CompilerServices;

namespace compiler.frontend
{

    public class Parser : IDisposable
    {
        public Token Tok { get; set; }
        public Lexer Scanner { get; set; }
        private readonly string _filename;

        public Parser(string pFileName)
        {
            _filename = pFileName;
            Tok = Token.UNKNOWN;
            Scanner = new Lexer(_filename);
        }
		public bool isRelOp()
		{
			switch (t)
			{
				case Token.EQUAL:
					return true;
				case Token.NOT_EQUAL:
					return true;
				case Token.LESS:
					return true;
				case Token.LESS_EQ:
					return true;
				case Token.GREATER:
					return true;
				case Token.GREATER_EQ:
					return true;
			}
			return false;
		}


        ~Parser()
        {
            Dispose(false);
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
			switch (t)
			{
				case Token.NUMBER:
					//TODO: Record number value
					next();
					break;
				case Token.IDENTIFIER:
					//TODO: Record identifier
					Designator();
					break;
				case Token.OPEN_PAREN:
					next();
					Expression();
					getExpected(Token.CLOSE_BRACKET);
					break;
				case Token.CALL:
					next();
					FuncCall();
					break;
				default:
					error_fatal();
					break;
			}
        }

        private void Term(){
			Factor();
			while ((t == Token.TIMES) || (t == Token.DIVIDE))
			{
				next();
				Factor();
			}
        }

        private void Assign()
		{

		}

       

        public void Expression()
		{
			Term();
			while ((t == Token.PLUS) || (t == Token.MINUS))
			{
				next();
				Term();
			}
}
        private void Assign()
		{

		}

        private void Computation()
        {
        }

        public void Relation()
		{
			Expression();
			if (!isRelOp())
			{
				error_fatal();
			}
			next();
			Expression();
        private void Identifier()
        {
        }

        private void Num()
        {
            
        }

		public void FuncCall()
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


        private void RelOp()
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






        public void Parse()
        {
            try
            {
                Computation();
            }
            catch (Exception e)
            {
                throw new NotImplementedException(e.Message);
            }
        }







        public void Dispose() 
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual  void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Scanner != null)
                {
                    Scanner.Dispose();
                    Scanner = null;
                }
            }
            
        }


    }
}
