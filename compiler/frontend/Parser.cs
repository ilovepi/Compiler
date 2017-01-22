using System;

namespace compiler.frontend
{
    public class Parser : IDisposable
    {
        private readonly string _filename;

        public Parser(string pFileName)
        {
            _filename = pFileName;
            Tok = Token.UNKNOWN;
            Scanner = new Lexer(_filename);
        }

        public Token Tok { get; set; }
        public Lexer Scanner { get; set; }

        public int Pos => Scanner.Position;

        public int LineNo => Scanner.LineNo;


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
                Next();
            else
                Error("Error in file: " + _filename + " at line " + LineNo + ", pos " + Pos +
                      "\n\tFound: " + TokenHelper.ToString(Tok) + " but Expected: " +
                      TokenHelper.ToString(expected));
        }

        public void Error(string str)
        {
            //TODO: determine location in file for error messages
            Console.WriteLine("Error Parsing file: " + _filename + ", " + str);
            FatalError();
        }

        public void FatalError()
        {
            //TODO: determine location in file for error messages
            throw new ParserException("Fatal Error Parsing file: " + _filename + ". Unable to continue");
        }


        public void Next()
        {
            Tok = Scanner.GetNextToken();
        }

        public void Designator()
        {
            GetExpected(Token.IDENTIFIER);
            while (Tok == Token.OPEN_BRACKET)
            {
                GetExpected(Token.OPEN_BRACKET);

                Expression();

                GetExpected(Token.CLOSE_BRACKET);
            }
        }

        public void Factor()
        {
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
                    FatalError();
                    break;
            }
        }


        public void Term()
        {
            Factor();
            while (Tok == Token.TIMES || Tok == Token.DIVIDE)
            {
                Next();
                Factor();
            }
        }


        public void Expression()
        {
            Term();
            while (Tok == Token.PLUS || Tok == Token.MINUS)
            {
                Next();
                Term();
            }
        }

        public void Assign()
        {
        }

        public void Computation()
        {
        }

        public void Relation()
        {
            Expression();

            if (!IsRelOp())
                FatalError();
            Next();
            Expression();
        }

        public void Identifier()
        {
        }

        public void Num()
        {
        }

        public void VarDecl()
        {
        }


        public void TypeDecl()
        {
        }

        public void FuncDecl()
        {
        }

        public void FuncBody()
        {
        }


        public void Statement()
        {
        }


        public void RelOp()
        {
        }


        public void FuncCall()
        {
        }

        public void IfStmt()
        {
        }


        public void WhileStmt()
        {
        }


        private void ReturnStmt()
        {
        }

        public void FormalParams()
        {
        }


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

        public void ThrowParserException(Token expected)
        {
            throw ParserException.CreateParserException(expected, Tok, LineNo, Pos, _filename);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                if (Scanner != null)
                {
                    Scanner.Dispose();
                    Scanner = null;
                }
        }
    }
}