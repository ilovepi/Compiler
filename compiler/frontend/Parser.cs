using System;
using System.Collections;
using System.Collections.Generic;
using compiler.middleend.ir;
using compiler.middlend.ir;

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

        public int CurrAddress { get; set; }

        /// <summary>
        /// A stack of frame addresses -- esentially a list of frame pointers
        /// </summary>
        public List<int> AddressStack { get; set; }


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
                throw ParserException.CreateParserException(expected, Tok, LineNo, Pos, _filename);
                /*Error("Error in file: " + _filename + " at line " + LineNo + ", pos " + Pos +
                      "\n\tFound: " + TokenHelper.ToString(Tok) + " but Expected: " +
                      TokenHelper.ToString(expected));*/
        }

        public void Error(string str)
        {
            //TODO: determine location in file for error messages
            Console.WriteLine("Error Parsing file: " + _filename + ", " + str);
            FatalError();
        }

        public void FatalError()
        {
            throw ParserException.CreateParserException(Tok, LineNo, Pos, _filename);
        }


        public void Next()
        {
            do
            {
                Tok = Scanner.GetNextToken();
            } while (Tok == Token.COMMENT);
        }

        public void Designator(Result res)
        {
            Identifier(res);

            // gen load addr of id


            //TODO handle generating array addresses
            while (Tok == Token.OPEN_BRACKET)
            {
                GetExpected(Token.OPEN_BRACKET);

                // calulate offset
                Expression();

                //add offset to addr of id

                //load result of addition

                //get bracket
                GetExpected(Token.CLOSE_BRACKET);
            }
        }

        public Instruction Factor()
        {
            Instruction x;

            switch (Tok)
            {
                case Token.NUMBER:
                    x = Num();
                    break;
                case Token.IDENTIFIER:
                    x = Designator();
                    break;
                case Token.OPEN_PAREN:
                    Next();
                    x = Expression();
                    GetExpected(Token.CLOSE_PAREN);
                    break;
                case Token.CALL:
                    x = FuncCall();
                    break;
                default:
                    FatalError();
                    break;
            }

            return x;
        }


        public Instruction Term()
        {
            Factor();
            while (Tok == Token.TIMES || Tok == Token.DIVIDE)
            {
                Next();
                f2 =Factor();
            }
        }


        public List<Instruction> Expression()
        {
            var instList = new List<Instruction>();

            Term();
            while (Tok == Token.PLUS || Tok == Token.MINUS)
            {
                Next();
                Term();
            }
        }

        public void Assign()
        {
            var res = new Result();
            GetExpected(Token.LET);

            Designator(res);

            GetExpected(Token.ASSIGN);

            Expression();
        }

        public void Computation()
        {
            GetExpected(Token.MAIN);

            while ((Tok == Token.VAR) || (Tok == Token.ARRAY))
            {
                VarDecl();
            }

            while ((Tok == Token.FUNCTION) || (Tok == Token.PROCEDURE))
            {
                FuncDecl();
            }

            GetExpected(Token.OPEN_CURL);

            StatementSequence();

            GetExpected(Token.CLOSE_CURL);

            GetExpected(Token.EOF);
        }

        public void Relation()
        {
            Expression();

            if (!IsRelOp())
            {
                FatalError();
            }
            Next();
            Expression();
        }

        public void Identifier(Result res)
        {
            GetExpected(Token.IDENTIFIER);
            res.Kind = Kind.Variable;
            //res.Addr = Scanner.SymbolTble.AddressTble[Scanner.Id];
        }

        public void CreateIdentifier()
        {
            GetExpected(Token.IDENTIFIER);
            Scanner.SymbolTble.InsertAddress(Scanner.Id, NextAddress());
            
        }


        
        public int NextAddress()
        {
            //TDOO: implement this function
            return 0;
        }

        public Insruction Num(Result result)
        {
            GetExpected(Token.NUMBER);
            int n = 0;
            return new Instruction(n,  , Operand.OpType.Constant, Scanner.Val);
        }

        public void VarDecl()
        {
            TypeDecl();

            // TODO: this is where we need to set variable addresses
            CreateIdentifier();

            while (Tok == Token.COMMA)
            {
                Next();

                CreateIdentifier();
            }

            GetExpected(Token.SEMI_COLON);
        }


        public void TypeDecl()
        {
            var res = new Result();

            if (Tok == Token.VAR)
            {
                Next();
            }
            else if (Tok == Token.ARRAY)
            {
                Next();

                GetExpected(Token.OPEN_BRACKET);

                Num(res);

                GetExpected(Token.CLOSE_BRACKET);

                while (Tok == Token.OPEN_BRACKET)
                {
                    Next();

                    Num(res);

                    GetExpected(Token.CLOSE_BRACKET);
                }
            }
            else
            {
                // TODO: replace
                FatalError();
            }
        }

        public void FuncDecl()
        {
            if ((Tok != Token.FUNCTION) && (Tok != Token.PROCEDURE))
            {
                FatalError();
            }

            Next();

            //TODO: Need a special address thing for functions
            CreateIdentifier();

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
            if (Tok == Token.LET) {
                Assign();
            } else if (Tok == Token.CALL)
            {
                FuncCall();
            } else if (Tok == Token.IF)
            {
                IfStmt();
            } else if (Tok == Token.WHILE)
            {
                WhileStmt();
            } else if (Tok == Token.RETURN)
            {
                ReturnStmt();
            } else
            {
                FatalError();
            }
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
            // TODO implement comparisions (replace IsRelOp)
            if (!IsRelOp())
            {
                FatalError();
            } else
            {
                Next();
            }
        }


        public void FuncCall()
        {

            GetExpected(Token.CALL);

            Result res = new Result();

            Identifier(res);

            if (Tok == Token.OPEN_PAREN)
            {
                GetExpected(Token.OPEN_PAREN);

                if ((Tok == Token.IDENTIFIER) ||
                    (Tok == Token.NUMBER) || 
                    (Tok == Token.OPEN_PAREN) || 
                    (Tok == Token.CALL))
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
            GetExpected(Token.RETURN);

            if ((Tok == Token.IDENTIFIER) || (Tok == Token.NUMBER) || (Tok == Token.OPEN_PAREN) || (Tok == Token.CALL))
            {
                Expression();
            }
        }

        public void FormalParams()
        {
            GetExpected(Token.OPEN_PAREN);

            if (Tok == Token.IDENTIFIER)
            {

                //TODO: handle parameters????
                CreateIdentifier();//Identifier();

                while (Tok == Token.COMMA)
                {
                    Next();

                    //not sure this is correct per above
                    CreateIdentifier();
                }
            }

            GetExpected(Token.CLOSE_PAREN);
        }


        public void Parse()
        {
            Next();
            Computation();
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