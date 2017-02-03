using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Instruction> Designator()
        {
            var ret = new List<Instruction>();
            ret.Add(Identifier());

            // gen load addr of id


            //TODO handle generating array addresses
            while (Tok == Token.OPEN_BRACKET)
            {
                GetExpected(Token.OPEN_BRACKET);

                // calulate offset
                ret.AddRange(Expression());

                //add offset to addr of id

                //load result of addition

                //get bracket
                GetExpected(Token.CLOSE_BRACKET);
            }

            return ret;
        }

        public List<Instruction> Factor()
        {
            List<Instruction> ret = new List<Instruction>();

            switch (Tok)
            {
                case Token.NUMBER:
                    ret.Add(Num());
                    break;
                case Token.IDENTIFIER:
                    ret.AddRange(Designator());
                    break;
                case Token.OPEN_PAREN:
                    Next();
                    ret.AddRange(Expression());
                    GetExpected(Token.CLOSE_PAREN);
                    break;
                case Token.CALL:
                    ret.AddRange(FuncCall());
                    break;
                default:
                    FatalError();
                    break;
            }

            return ret;
        }


        public List<Instruction> Term()
        {
            List<Instruction> ret = Factor();
            Instruction curr = ret.Last();

            while (Tok == Token.TIMES || Tok == Token.DIVIDE)
            {
                // cache current arithmetic token
                IrOps op = (Tok == Token.TIMES) ? IrOps.mul : IrOps.div;

                // advance to next token
                Next();

                // add instructions for the next factor
                ret.AddRange(Factor());

                //cache the last instruction
                Instruction next = ret.Last();

                // create new instruction
                Instruction newInst = new Instruction(op, new Operand(curr), new Operand(next));

                // insert new instruction to instruction list
                ret.Add(newInst);

                // update current instruction to latest instruction
                curr = ret.Last();
            }

            return ret;
        }


        public List<Instruction> Expression()
        {
            var ret  = Term();

            Instruction curr = ret.Last();

            while (Tok == Token.PLUS || Tok == Token.MINUS)
            {

                // cache current arithmetic token
                IrOps op = (Tok == Token.PLUS) ? IrOps.add : IrOps.sub;

                // advance to next token
                Next();

                // add instructions for the next term
                ret.AddRange(Term());

                //cache the last instruction
                Instruction next = ret.Last();

                // create new instruction
                Instruction newInst = new Instruction(op, new Operand(curr), new Operand(next));

                // insert new instruction to instruction list
                ret.Add(newInst);

                // update current instruction to latest instruction
                curr = ret.Last();

            }

            return ret;
        }

        public void Assign()
        {

            // assign must use SSA, so our designator *MUST* give us access to an SSA variable


            GetExpected(Token.LET);

            var ret = Designator();
            var curr = ret.Last();

            GetExpected(Token.ASSIGN);

            ret.AddRange( Expression() );
            var next = ret.Last();

            //TODO: Fix this!!!!

                // create new instruction
                Instruction newInst = new Instruction(IrOps.store, new Operand(curr), new Operand(next));

                // insert new instruction to instruction list
                ret.Add(newInst);

                // update current instruction to latest instruction
                curr = ret.Last();
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

        public Instruction Identifier()
        {
            GetExpected(Token.IDENTIFIER);

            return new Instruction(IrOps.load, new Operand(Operand.OpType.Identifier, Scanner.Id), null);
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

        public Instruction Num()
        {
            GetExpected(Token.NUMBER);

            return new Instruction(IrOps.load, new Operand(Operand.OpType.Constant, Scanner.Val), null);
        }

        public void VarDecl()
        {
            TypeDecl();

            // TODO: this is where we need to set variable addresses
            //CreateIdentifier();
            Identifier();

            while (Tok == Token.COMMA)
            {
                Next();

                //CreateIdentifier();
                Identifier();

            }

            GetExpected(Token.SEMI_COLON);
        }


        public void TypeDecl()
        {

            if (Tok == Token.VAR)
            {
                Next();
            }
            else if (Tok == Token.ARRAY)
            {
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
            //CreateIdentifier();
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


        public List<Instruction> FuncCall()
        {

            GetExpected(Token.CALL);

            var ret = new List<Instruction>();
            ret.Add(Identifier());

            if (Tok == Token.OPEN_PAREN)
            {
                GetExpected(Token.OPEN_PAREN);

                if ((Tok == Token.IDENTIFIER) ||
                    (Tok == Token.NUMBER)     ||
                    (Tok == Token.OPEN_PAREN) || 
                    (Tok == Token.CALL))
                {
                   ret.AddRange(Expression());

                    while (Tok == Token.COMMA)
                    {
                        Next();
                        ret.AddRange(Expression());
                    }
                }

                GetExpected(Token.CLOSE_PAREN);
                //TODO: jump to call
            }

            return ret;
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
               // CreateIdentifier();
               Identifier();

                while (Tok == Token.COMMA)
                {
                    Next();

                    //not sure this is correct per above
                    //CreateIdentifier();
                    Identifier();
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