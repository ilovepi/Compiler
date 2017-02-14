using System;
using System.Collections.Generic;
using System.Linq;
using compiler.middleend.ir;

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
            ProgramCfg = new CFG();
        }

        public Token Tok { get; set; }

        public Lexer Scanner { get; set; }

        public int Pos => Scanner.Position;

        public int LineNo => Scanner.LineNo;

        public int CurrAddress { get; set; }


        /// <summary>
        ///     A stack of frame addresses -- esentially a list of frame pointers
        /// </summary>
        public List<int> AddressStack { get; set; }


        public CFG ProgramCfg { get; set; }

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
            {
                Next();
            }
            else
            {
                throw ParserException.CreateParserException(expected, Tok, LineNo, Pos, _filename);
            }
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
            var ret = new List<Instruction> {Identifier()};

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
            var ret = new List<Instruction>();

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

            while ((Tok == Token.TIMES) || (Tok == Token.DIVIDE))
            {
                // cache current arithmetic token
                IrOps op = Tok == Token.TIMES ? IrOps.mul : IrOps.div;

                // advance to next token
                Next();

                // add instructions for the next factor
                ret.AddRange(Factor());

                //cache the last instruction
                Instruction next = ret.Last();

                // create new instruction
                var newInst = new Instruction(op, new Operand(curr), new Operand(next));

                // insert new instruction to instruction list
                ret.Add(newInst);

                // update current instruction to latest instruction
                curr = ret.Last();
            }

            return ret;
        }


        public List<Instruction> Expression()
        {
            List<Instruction> ret = Term();

            Instruction curr = ret.Last();

            while ((Tok == Token.PLUS) || (Tok == Token.MINUS))
            {
                // cache current arithmetic token
                IrOps op = Tok == Token.PLUS ? IrOps.add : IrOps.sub;

                // advance to next token
                Next();

                // add instructions for the next term
                ret.AddRange(Term());

                //cache the last instruction
                Instruction next = ret.Last();

                // create new instruction
                var newInst = new Instruction(op, new Operand(curr), new Operand(next));

                // insert new instruction to instruction list
                ret.Add(newInst);

                // update current instruction to latest instruction
                curr = ret.Last();
            }

            return ret;
        }

        public List<Instruction> Assign()
        {
            //List<Instruction> ret = new List<Instruction>();

            //TODO: assign must use SSA, so our designator *MUST* give us access to an SSA variable


            GetExpected(Token.LET);

            List<Instruction> ret = Designator();
            Instruction curr = ret.Last();

            GetExpected(Token.ASSIGN);

            ret.AddRange(Expression());
            Instruction next = ret.Last();

            //TODO: Fix this!!!!

            // create new instruction
            var newInst = new Instruction(IrOps.store, new Operand(curr), new Operand(next));

            // insert new instruction to instruction list
            ret.Add(newInst);

            // update current instruction to latest instruction
            curr = ret.Last();


            return ret;
        }

        public CFG Computation()
        {
            GetExpected(Token.MAIN);

            var cfg = new CFG();

            while ((Tok == Token.VAR) || (Tok == Token.ARRAY))
            {
                VarDecl();
            }

            while ((Tok == Token.FUNCTION) || (Tok == Token.PROCEDURE))
            {
                // throw away cFG for now
                //cfg.Insert( FuncDecl() );
                FuncDecl();
            }

            GetExpected(Token.OPEN_CURL);

            cfg.Insert(StatementSequence());

            GetExpected(Token.CLOSE_CURL);

            GetExpected(Token.EOF);

            return cfg;
        }

        public List<Instruction> Relation()
        {
            var ret  = Expression();

            var arg1 = ret.Last();

            if (!IsRelOp())
            {
                FatalError();
            }

            Token comp = Tok;

            Next();
            ret.AddRange(Expression());
            var arg2 = ret.Last();

            ret.Add(new Instruction(IrOps.cmp, new Operand( arg1), new Operand( arg2)));
            ret.Add(new Instruction(IrOps.bne, new Operand(ret.Last()), new Operand(Operand.OpType.Constant, 0)));

            // set the correct IR op code
            switch (comp)
            {
                case Token.EQUAL:
                    ret.Last().Op = IrOps.bne;
                    break;
                case Token.NOT_EQUAL:
                    ret.Last().Op = IrOps.beq;
                    break;
                case Token.LESS:
                    ret.Last().Op = IrOps.bge;
                    break;
                case Token.LESS_EQ:
                    ret.Last().Op = IrOps.bgt;
                    break;
                case Token.GREATER:
                    ret.Last().Op = IrOps.ble;
                    break;
                case Token.GREATER_EQ:
                    ret.Last().Op = IrOps.blt;
                    break;
                default:
                    FatalError();
                    break;
            }

            return ret;
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
            //TODO: implement this function
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

        public CFG FuncDecl()
        {
            var cfg = new CFG();

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

            return cfg;
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


        public CFG Statement()
        {
            //TODO: CFG has trouble adding to new blocks, or inserting into CFG
            var cfgTemp = new CFG {Root = new Node(new BasicBlock("StatementBlock"))};

            switch (Tok)
            {
                case Token.LET:
                    cfgTemp.Root.BB.AddInstructionList(Assign());
                    break;
                case Token.CALL:
                    cfgTemp.Root.BB.AddInstructionList(FuncCall());
                    break;
                case Token.IF:
                    return IfStmt();
                case Token.WHILE:
                    return WhileStmt();
                case Token.RETURN:
                    cfgTemp.Root.BB.AddInstructionList(ReturnStmt());
                    break;
                default:
                    FatalError();
                    break;
            }

            return cfgTemp;
        }


        public CFG StatementSequence()
        {
            var cfg = new CFG();
            var bb = new BasicBlock("StatSequence");
            cfg.Root = new Node(bb);
            cfg.Insert(Statement());

            // TODO: fix consolodate()
            Node.Consolidate(cfg.Root);

            while (Tok == Token.SEMI_COLON)
            {
                Next();
                cfg.Insert(Statement());
                Node.Consolidate(cfg.Root);
            }

            return cfg;
        }


        public void RelOp()
        {
            // TODO implement comparisions (replace IsRelOp)
            if (!IsRelOp())
            {
                FatalError();
            }
            else
            {
                Next();
            }
        }


        public List<Instruction> FuncCall()
        {
            GetExpected(Token.CALL);

            var ret = new List<Instruction> {Identifier()};

            if (Tok == Token.OPEN_PAREN)
            {
                GetExpected(Token.OPEN_PAREN);

                if ((Tok == Token.IDENTIFIER) ||
                    (Tok == Token.NUMBER) ||
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

        public CFG IfStmt()
        {
            GetExpected(Token.IF);
            var ifBlock = new CFG();
            var compBlock = new CompareNode(new BasicBlock("CompareBlock"));

            var joinBlock = new JoinNode(new BasicBlock("JoinBlock"));
            Node falseBlock = joinBlock;

            compBlock.BB.AddInstructionList(Relation());

            GetExpected(Token.THEN);
            
            ifBlock.Insert(compBlock);

            Node trueBlock = StatementSequence().Root;

            compBlock.InsertTrue(trueBlock);
            trueBlock.InsertTrue(joinBlock);

            if (Tok == Token.ELSE)
            {
                Next();
                falseBlock = StatementSequence().Root;
                Node.Leaf(falseBlock).InsertFalse(joinBlock);
                Node.Consolidate(falseBlock);
            }


            compBlock.InsertFalse(falseBlock);

            GetExpected(Token.FI);

            //TODO: remove placeholder instruction and do something smarter
            joinBlock.BB.Instructions.Add(new Instruction(IrOps.phi, new Operand(Operand.OpType.Identifier, 0), new Operand(Operand.OpType.Identifier, 0)));
            
            compBlock.BB.Instructions.Last().Arg2 = new Operand( falseBlock.GetNextInstruction());
            Node.Leaf( trueBlock).BB.Instructions.Last().Arg2 = new Operand(joinBlock.BB.Instructions.First());

            return ifBlock;
        }


        public CFG WhileStmt()
        {
            GetExpected(Token.WHILE);

            // create cfg
            var whileBlock = new CFG();

            //crate compare block/loop header block
            var compBlock = new WhileNode(new BasicBlock("WhileCompareBlock"));

            // TODO: Correct placeholder Phi Instruction
            compBlock.BB.AddInstruction(new Instruction(IrOps.phi, new Operand(Operand.OpType.Identifier, 0), new Operand(Operand.OpType.Identifier, 0)));

            // insert compare block for while stmt
            whileBlock.Insert(compBlock);

            // add the relation/branch comparison into the loop header block
            compBlock.BB.AddInstructionList(Relation());

            GetExpected(Token.DO);

            // prepare basic block for loop body
            CFG stmts = StatementSequence();

            Node loopBlock = stmts.Root;
            loopBlock.BB.AddInstruction( new Instruction(IrOps.bra, new Operand(compBlock.GetNextInstruction()), null) );

            Node last = stmts.GetLeaf(stmts.Root);

            //TODO: try to refactor so that we don't have to insert on the false branch
            // insert the loop body on the true path
            compBlock.InsertTrue(loopBlock);

            last.Child = compBlock;

            compBlock.LoopParent = last;

            GetExpected(Token.OD);

            var followBlock = new Node(new BasicBlock("FollowBlock"));

            compBlock.InsertFalse(followBlock);


            //TODO: remove placeholder instruction and do something smarter
            followBlock.BB.AddInstruction(new Instruction(IrOps.phi, new Operand(Operand.OpType.Identifier, 0), new Operand(Operand.OpType.Identifier, 0)));

            last.GetLastInstruction().Arg2 = new Operand(compBlock.GetNextInstruction());

            // TODO: this is straight up wrong. we can leave this alone and fix it in the enclosing scope
            compBlock.BB.Instructions.Last().Arg2 = new Operand(followBlock.BB.Instructions.First());


            
            return whileBlock;
        }


        //TODO: Maybe pass in a return address?
        private List<Instruction> ReturnStmt()
        {
            GetExpected(Token.RETURN);

            var ret = new List<Instruction>();

            if ((Tok == Token.IDENTIFIER) || (Tok == Token.NUMBER) || (Tok == Token.OPEN_PAREN) || (Tok == Token.CALL))
            {
                ret.AddRange(Expression());
            }

            //TODO: probably want to make better instruction here, with a real address
            ret.Add(new Instruction(IrOps.bra, new Operand(Operand.OpType.Register, 0), null));
            return ret;
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
            ProgramCfg = Computation();
        }

        public void ThrowParserException(Token expected)
        {
            throw ParserException.CreateParserException(expected, Tok, LineNo, Pos, _filename);
        }

        protected virtual void Dispose(bool disposing)
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
