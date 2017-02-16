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
            FunctionsCfgs = new List<CFG>();
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

        public List<CFG> FunctionsCfgs { get; set; }

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

        public Tuple<Operand, List<Instruction>> Designator()
        {
            //public Operand Designator()
            var originalId = Identifier();
            var id = originalId;
            var instructions = new List<Instruction>();
            //Tuple<Operand, List<Instruction>> ret = new Tuple<Operand, List<Instruction>>(id,instructions);

            // gen load addr of id
            //var baseAddr = new Instruction(IrOps.load, new Operand(Operand.OpType.Identifier, Scanner.Id), null);

            //TODO handle generating array addresses
            while (Tok == Token.OPEN_BRACKET)
            {
                // load array base address

                var baseAddr = new Instruction(IrOps.load, id, null);
                instructions.Add(baseAddr);
                id = new Operand(baseAddr);


                GetExpected(Token.OPEN_BRACKET);

                // calulate offset

                var exp = Expression();
                instructions.AddRange(exp.Item2);

                //add offset to addr of id
                //instructions.Add(new Instruction(IrOps.adda, id,  new Operand(instructions.Last())));
                instructions.Add(new Instruction(IrOps.adda, id, exp.Item1));

                // Delay the indirect load until the top of the loop so we can do the last
                // load or store at the reference site.

                //inderect load result of basaddr + offset
                //baseAddr = new Instruction(IrOps.load, new Operand(instructions.Last()), null);
                //instructions.Add(baseAddr);
                //id = new Operand(baseAddr);


                //get bracket
                GetExpected(Token.CLOSE_BRACKET);

                // set the current operand to the last adda inst, so we can get the load right at the end
                id = new Operand(instructions.Last());
            }

            return new Tuple<Operand, List<Instruction>>(id, instructions);

        }

        public Tuple<Operand, List<Instruction>> Factor()
        {
            Tuple<Operand, List<Instruction>> factor;
            var instructions = new List<Instruction>();
            Operand id;

            switch (Tok)
            {
                case Token.NUMBER:
                    id = Num();
                    factor = new Tuple<Operand, List<Instruction>>(id, new List<Instruction>() );
                    break;
                case Token.IDENTIFIER:
                    var des =  Designator();
                    instructions.AddRange(des.Item2);
                    //Operand arg2 = (instructions.Count == 0) ? null : new Operand(instructions.Last())
                    var baseAddr = new Instruction(IrOps.load, des.Item1, null);
                    instructions.Add(baseAddr);
                    id = new Operand(baseAddr);
                    factor = new Tuple<Operand, List<Instruction>>(id,instructions);
                    break;
                case Token.OPEN_PAREN:
                    Next();
                    factor = Expression();
                    GetExpected(Token.CLOSE_PAREN);
                    break;
                case Token.CALL:
                    factor = FuncCall();
                    break;
                default:
                    FatalError();
                    factor = new Tuple<Operand, List<Instruction>>(new Operand(Operand.OpType.Constant,0xDEAD), instructions);
                    break;
            }

            return factor;
        }


        public Tuple<Operand, List<Instruction>> Term()
        {
            var factor1 = Factor();
            var id = factor1.Item1;
            var instructions = factor1.Item2;
            //Instruction curr = instructions.Last();

            while ((Tok == Token.TIMES) || (Tok == Token.DIVIDE))
            {
                // cache current arithmetic token
                IrOps op = Tok == Token.TIMES ? IrOps.mul : IrOps.div;

                // advance to next token
                Next();

                // add instructions for the next factor
                var factor2 = Factor();
                instructions.AddRange(factor2.Item2);

                //TODO: find a good way of making this so we can support immediate actions
                if ((factor2.Item1.Kind == Operand.OpType.Constant) && (factor1.Item1.Kind == Operand.OpType.Constant))
                {
                    var arg2 = factor2.Item1.Val;
                    var arg1 = factor1.Item1.Val;
                    var res = (op == IrOps.mul) ? (arg1 * arg2) : (arg1 / arg2);
                    id = new Operand(Operand.OpType.Constant, res);
                    //var ret = new Tuple<Operand, List<Instruction>>(new Operand(Operand.OpType.Constant, res), new List<Instruction>() );

                }
                else
                {
                    var newInst = new Instruction(op, factor1.Item1, factor2.Item1);

                    // insert new instruction to instruction list
                    instructions.Add(newInst);
                    id = new Operand(newInst);
                }

                factor1 = new Tuple<Operand, List<Instruction>>(id, instructions);
            }

            return factor1;
        }


        public Tuple<Operand, List<Instruction> > Expression()
        {
            var term1 = Term();
            var id = term1.Item1;
            var instructions = term1.Item2;

            //Instruction curr = term1.Item2.Last();

            while ((Tok == Token.PLUS) || (Tok == Token.MINUS))
            {
                // cache current arithmetic token
                IrOps op = Tok == Token.PLUS ? IrOps.add : IrOps.sub;

                // advance to next token
                Next();

                // add instructions for the next term
                var term2 = Term();

                //cache the last instruction
                //Instruction next = term1.Last();

                // create new instruction
                //var newInst = new Instruction(op, new Operand(curr), new Operand(next));
                instructions.AddRange(term2.Item2);

                if ((term2.Item1.Kind == Operand.OpType.Constant) && (term1.Item1.Kind == Operand.OpType.Constant))
                {
                    var arg2 = term2.Item1.Val;


                    var arg1 = term1.Item1.Val;
                    var res = (op == IrOps.add) ? (arg1 + arg2) : (arg1 - arg2);
                    id = new Operand(Operand.OpType.Constant, res);
                    //var ret = new Tuple<Operand, List<Instruction>>(new Operand(Operand.OpType.Constant, res), new List<Instruction>() );

                }
                else
                {
                    var newInst = new Instruction(op, id, term2.Item1);

                    // insert new instruction to instruction list
                    instructions.Add(newInst);
                    id = new Operand(newInst);
                }

                term1 = new Tuple<Operand, List<Instruction>>( id, instructions);
            }

            return term1;
        }

        public Tuple<Operand, List<Instruction>> Assign()
        {
            //List<Instruction> ret = new List<Instruction>();

            //TODO: assign must use SSA, so our designator *MUST* give us access to an SSA variable


            GetExpected(Token.LET);

            var id = Designator();
            //Instruction curr = ret.Last();

            GetExpected(Token.ASSIGN);

            //ret.AddRange(Expression());
            //Instruction next = ret.Last();

            var expValue = Expression();

            //TODO: Fix this!!!!

            // create new instruction
            var newInst = new Instruction(IrOps.store, expValue.Item1, id.Item1);

            id.Item2.AddRange(expValue.Item2);

            // insert new instruction to instruction list
            id.Item2.Add(newInst);

            // update current instruction to latest instruction
            //curr = ret.Last();

            return new Tuple<Operand, List<Instruction>>(new Operand(newInst), id.Item2 );
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
                var func = FuncDecl();
                if (func.Root != null)
                {
                    FunctionsCfgs.Add(func);
                }
                //FuncDecl();
            }

            GetExpected(Token.OPEN_CURL);

            cfg.Insert(StatementSequence());

            GetExpected(Token.CLOSE_CURL);

            GetExpected(Token.EOF);

            return cfg;
        }

        public Tuple<Operand,  List<Instruction>> Relation()
        {
            
            var leftVal  = Expression();
            // copy instructions from first expression
            var instructions = new List<Instruction>(leftVal.Item2);
            var arg1 = leftVal.Item1;

            if (!IsRelOp())
            {
                FatalError();
            }

            Token comp = Tok;

            Next();

            var rightVal = Expression();

            // copy instructions from right expression
            instructions.AddRange(rightVal.Item2);
            var arg2 = rightVal.Item1;

            //var newOperand = new Operand(instructions);

            var compare = new Instruction(IrOps.cmp, arg1, arg2);
            instructions.Add(compare);
            var branch = new Instruction(IrOps.bne, new Operand(compare), new Operand(Operand.OpType.Constant, 0));
            instructions.Add(branch);


            // set the correct IR op code
            switch (comp)
            {
                case Token.EQUAL:
                    branch.Op = IrOps.bne;
                    break;
                case Token.NOT_EQUAL:
                   branch.Op = IrOps.beq;
                    break;
                case Token.LESS:
                    branch.Op = IrOps.bge;
                    break;
                case Token.LESS_EQ:
                   branch.Op = IrOps.bgt;
                    break;
                case Token.GREATER:
                    branch.Op = IrOps.ble;
                    break;
                case Token.GREATER_EQ:
                   branch.Op = IrOps.blt;
                    break;
                default:
                    FatalError();
                    break;
            }

            return new Tuple<Operand, List<Instruction>>(new Operand(branch), instructions);
        }

        public Operand Identifier()
        {
            var id = Scanner.Id;
            GetExpected(Token.IDENTIFIER);

            return new Operand(Operand.OpType.Identifier, id);
        }

        public void CreateIdentifier()
        {
            var id = Scanner.Id;
            GetExpected(Token.IDENTIFIER);
            Scanner.SymbolTble.InsertAddress(id, NextAddress());
        }


        public int NextAddress()
        {
            //TODO: implement this function
            return 0;
        }

        public Operand Num()
        {
            GetExpected(Token.NUMBER);

            //return new Instruction(IrOps.load, new Operand(Operand.OpType.Constant, Scanner.Val), null);
            return new Operand(Operand.OpType.Constant, Scanner.Val);
        }


        public void VarDecl()
        {
            // TODO: allocate variables here
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

        public int TypeDecl()
        {
            // TODO: determine size of allocation required
            int size = 4;
            if (Tok == Token.VAR)
            {
                Next();
                //size = 4;
            }
            else if (Tok == Token.ARRAY)
            {
                Next();

                GetExpected(Token.OPEN_BRACKET);

                var n = Num();
                size *= n.Val;

                GetExpected(Token.CLOSE_BRACKET);

                while (Tok == Token.OPEN_BRACKET)
                {
                    Next();

                    n = Num();
                    size *= (n.Val);

                    GetExpected(Token.CLOSE_BRACKET);
                }
            }
            else
            {
                // TODO: replace
                FatalError();
            }
            return size;
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
            var id = Identifier();
            List<Operand> paramList = null;

            if (Tok == Token.OPEN_PAREN)
            {
                paramList = FormalParams();
            }

            GetExpected(Token.SEMI_COLON);

            var fb = FuncBody();
            

            fb.Name = Scanner.SymbolTble.Symbols[id.IdKey];

            cfg.Insert(fb);
            cfg.Name = fb.Name;

            GetExpected(Token.SEMI_COLON);

            return cfg;
        }

        public CFG FuncBody()
        {
            CFG cfg = null;
            while ((Tok == Token.VAR) || (Tok == Token.ARRAY))
            {
                VarDecl();
            }

            GetExpected(Token.OPEN_CURL);

            if ((Tok == Token.LET) || (Tok == Token.CALL) || (Tok == Token.IF)
                || (Tok == Token.WHILE) || (Tok == Token.RETURN))
            {
                cfg = StatementSequence();
            }

            GetExpected(Token.CLOSE_CURL);

            return cfg;
        }


        public CFG Statement()
        {
            //TODO: CFG has trouble adding to new blocks, or inserting into CFG
            var cfgTemp = new CFG {Root = new Node(new BasicBlock("StatementBlock"))};
            // TODO: address what to do with return opperand;
            Tuple<Operand, List<Instruction>> stmt = null;


            switch (Tok)
            {
                case Token.LET:
                    stmt = Assign();
                    cfgTemp.Root.BB.AddInstructionList(stmt.Item2);
                    break;
                case Token.CALL:
                    stmt = FuncCall();
                    cfgTemp.Root.BB.AddInstructionList(stmt.Item2);
                    //cfgTemp.Root.BB.AddInstructionList(FuncCall());
                    break;
                case Token.IF:
                    return IfStmt();
                case Token.WHILE:
                    return WhileStmt();
                case Token.RETURN:
                    stmt = ReturnStmt();
                    cfgTemp.Root.BB.AddInstructionList(stmt.Item2);
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
           cfg.Root.Consolidate();

            while (Tok == Token.SEMI_COLON)
            {
                Next();
                cfg.Insert(Statement());

                cfg.Root.Consolidate();
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

        public Tuple<Operand, List<Instruction>> MergeTuple(Tuple<Operand, List<Instruction>> a, Tuple<Operand, List<Instruction>> b)
        {
            var inst = new List<Instruction>(a.Item2);
            inst.AddRange(b.Item2);
            return new Tuple<Operand, List<Instruction>>(b.Item1, inst);
        }


        public Tuple<Operand, List<Instruction>> FuncCall()
        {
            GetExpected(Token.CALL);

            var id = Identifier();
            var instructions = new List<Instruction>() ;

            List<Tuple<Operand, List<Instruction>>> paramList = new List<Tuple<Operand, List<Instruction>>>();

            if (Tok == Token.OPEN_PAREN)
            {
                GetExpected(Token.OPEN_PAREN);

                if ((Tok == Token.IDENTIFIER) ||
                    (Tok == Token.NUMBER) ||
                    (Tok == Token.OPEN_PAREN) ||
                    (Tok == Token.CALL))
                {

                    paramList.Add(Expression());

                    while (Tok == Token.COMMA)
                    {
                        Next();
                        paramList.Add(Expression());
                    }

                    // do something with the param list to push items on stack for call

                }

                GetExpected(Token.CLOSE_PAREN);
                //TODO: jump to call
            }

            foreach (var item in paramList)
            {
                instructions.AddRange(item.Item2);
            }

            var call = new Instruction(IrOps.bra, id, null);

            instructions.Add(call);


            return new Tuple<Operand, List<Instruction>>(id, instructions);
        }

        public CFG IfStmt()
        {
            GetExpected(Token.IF);
            var ifBlock = new CFG();
            var compBlock = new CompareNode(new BasicBlock("CompareBlock"));

            var joinBlock = new JoinNode(new BasicBlock("JoinBlock"));
            Node falseBlock = joinBlock;

            // HACK: should we use the operand of the relation?
            compBlock.BB.AddInstructionList(Relation().Item2);

            GetExpected(Token.THEN);
            
            ifBlock.Insert(compBlock);

            Node trueBlock = StatementSequence().Root;
            trueBlock.Consolidate();

            compBlock.InsertTrue(trueBlock);
            trueBlock.Leaf().InsertJoinTrue(joinBlock);

            if (Tok == Token.ELSE)
            {
                Next();
                falseBlock = StatementSequence().Root;
                Node.Leaf(falseBlock).InsertJoinFalse(joinBlock);
                falseBlock.Consolidate();
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
            // HACK: should we use the opperand of the relation?
            compBlock.BB.AddInstructionList(Relation().Item2);

            GetExpected(Token.DO);

            // prepare basic block for loop body
            CFG stmts = StatementSequence();

            Node loopBlock = stmts.Root;
            loopBlock.BB.AddInstruction( new Instruction(IrOps.bra, new Operand(compBlock.GetNextInstruction()), null) );
            loopBlock.Consolidate();
            var last = loopBlock.Leaf();

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
        private Tuple<Operand, List<Instruction>> ReturnStmt()
        {
            GetExpected(Token.RETURN);

            var instructions = new List<Instruction>();

            Tuple<Operand, List<Instruction>> retStmt = null;

            if ((Tok == Token.IDENTIFIER) || (Tok == Token.NUMBER) || (Tok == Token.OPEN_PAREN) || (Tok == Token.CALL))
            {
                retStmt =  Expression();
                instructions = retStmt.Item2;
            }

            //TODO: probably want to make better instruction here, with a real address
            var branchBack = new Instruction(IrOps.bra, new Operand(Operand.OpType.Register, 0), null);
            instructions.Add(branchBack);
            if (retStmt == null)
            {
                retStmt = new Tuple<Operand, List<Instruction>>(new Operand(instructions.Last()), instructions);
            }
            else
            {
                retStmt = new Tuple<Operand, List<Instruction>>(new Operand(branchBack), instructions );
            }

            return retStmt;
        }

        public List<Operand> FormalParams()
        {
            GetExpected(Token.OPEN_PAREN);

            List<Operand> paramList = new List<Operand>();

            if (Tok == Token.IDENTIFIER)
            {
                //TODO: handle parameters????
                // CreateIdentifier();
                paramList.Add(Identifier());

                while (Tok == Token.COMMA)
                {
                    Next();

                    //not sure this is correct per above
                    //CreateIdentifier();
                    paramList.Add(Identifier());
                }
            }

            GetExpected(Token.CLOSE_PAREN);

            return paramList;
        }


        public void Parse()
        {
            Next();
            ProgramCfg = Computation();
            FunctionsCfgs.Add(ProgramCfg);
            ProgramCfg.Name = "Main";
            ProgramCfg.Sym = this.Scanner.SymbolTble;
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
