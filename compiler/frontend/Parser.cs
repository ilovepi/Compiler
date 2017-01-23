﻿using System;

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
            Identifier();

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
                    Num();
                    break;
                case Token.IDENTIFIER:
                    Identifier();
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
            GetExpected(Token.LET);

            Designator();

            GetExpected(Token.ASSIGN);

            Expression();
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
            GetExpected(Token.IDENTIFIER);
        }

        public void Num()
        {
            GetExpected(Token.NUMBER);
        }

        public void VarDecl()
        {
            TypeDecl();

            Identifier();

            while (Tok == Token.COMMA)
            {
                Next();

                Identifier();
            }

            GetExpected(Token.SEMI_COLON);
        }


        public void TypeDecl()
        {
            if ((Tok != Token.VAR) && (Tok != Token.ARRAY))
            {
                FatalError();
            }

            Next();

            GetExpected(Token.OPEN_BRACKET);

            Num();

            GetExpected(Token.CLOSE_BRACKET);

            while (Tok == Token.OPEN_BRACKET)
            {
                Next();

                Num();

                GetExpected(Token.CLOSE_BRACKET);
            }
        }

        public void FuncDecl()
        {
            if ((Tok != Token.FUNCTION) && (Tok != Token.PROCEDURE))
            {
                FatalError();
            }

            Next();

            Identifier();

            if (Tok == Token.OPEN_PAREN)
            {
                FormalParams();
            }

            GetExpected(Token.SEMI_COLON);

            FuncBody();

            GetExpected(Token.SEMI_COLON);
        }

        public void FuncBody()
        {
            while ((Tok == Token.VAR) || (Tok == Token.ARRAY))
            {
                VarDecl();
            }

            GetExpected(Token.OPEN_CURL);

            if ((Tok == Token.LET) || (Tok == Token.CALL) || (Tok == Token.IF)
                || (Tok == Token.WHILE) || (Tok == Token.RETURN))
            {
                StatementSequence();
            }

            GetExpected(Token.CLOSE_CURL);
        }


        public void Statement()
        {
            if ((Tok != Token.LET) || (Tok != Token.CALL) || (Tok != Token.IF)
                || (Tok != Token.WHILE) || (Tok != Token.RETURN))
            {
                FatalError();
            }

            Next();
        }


        public void StatementSequence()
        {
            Statement();

            while (Tok == Token.SEMI_COLON)
            {
                Next();
                Statement();
            }
        }


        public void RelOp()
        {
        }


        public void FuncCall()
        {
            GetExpected(Token.CALL);

            Identifier();

            if (Tok == Token.OPEN_PAREN)
            {
                GetExpected(Token.OPEN_PAREN);

                if (Tok == Token.IDENTIFIER)
                {
                    Expression();

                    while (Tok == Token.COMMA)
                    {
                        Next();
                        Expression();
                    }
                }

                GetExpected(Token.CLOSE_PAREN);
            }
        }

        public void IfStmt()
        {
            GetExpected(Token.IF);

            Relation();

            GetExpected(Token.THEN);

            StatementSequence();

            if (Tok == Token.ELSE)
            {
                Next();
                StatementSequence();
            }

            GetExpected(Token.FI);
        }


        public void WhileStmt()
        {
            GetExpected(Token.WHILE);

            Relation();

            GetExpected(Token.DO);

            StatementSequence();

            GetExpected(Token.OD);
        }


        private void ReturnStmt()
        {
        }

        public void FormalParams()
        {
            GetExpected(Token.OPEN_PAREN);

            if (Tok == Token.IDENTIFIER)
            {
                Next();

                while (Tok == Token.COMMA)
                {
                    Next();

                    Identifier();
                }
            }

            GetExpected(Token.CLOSE_PAREN);
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