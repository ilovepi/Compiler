using System;
using System.Runtime.CompilerServices;

namespace compiler.frontend
{

    public class Parser : IDisposable
    {
        public Token Tok { get; set; }
        public Lexer Scanner { get; set; }

        private readonly string _filename;

        public int Pos => Scanner.Position;

        public int LineNo => Scanner.LineNo;

        public Parser(string pFileName)
        {
            _filename = pFileName;
            Tok = Token.UNKNOWN;
            Scanner = new Lexer(_filename);
        }

		public bool IsRelOp()
		{
            switch (Tok)
            {
                case Token.EQUAL:
                case Token.NOT_EQUAL:
                case Token.LESS:
                case Token.LESS_EQ:
                case Token.GREATER:
                case Token.GREATER_EQ:
                    return true;
                default:
                    return false;
            }
        }


        ~Parser()
        {
            Dispose(false);
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
			switch (Tok)
			{
				case Token.NUMBER:
					//TODO: Record number value
					Next();
					break;
				case Token.IDENTIFIER:
					//TODO: Record identifier
					Designator();
					break;
				case Token.OPEN_PAREN:
					Next();
					Expression();
					GetExpected(Token.CLOSE_BRACKET);
					break;
				case Token.CALL:
					Next();
					FuncCall();
					break;
				default:
                    FatalError();;
					break;
			}
        }


        private void Term(){
			Factor();
			while ((Tok == Token.TIMES) || (Tok == Token.DIVIDE))
			{
				Next();
				Factor();
			}
        }


       

        public void Expression()
		{
			Term();
			while ((Tok == Token.PLUS) || (Tok == Token.MINUS))
			{
				Next();
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
            if (
                !
                    IsRelOp())
            {
                FatalError();
            }
            Next();
            Expression();
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

        public void ThrowParserException(Token Expected)
        {
            ParserException e;
            throw CreateParserException(Expected, Tok, )
            
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
