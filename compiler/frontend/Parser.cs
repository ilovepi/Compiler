using System;
using System.Collections.Generic;
using System.Linq;
using compiler.middleend.ir;

using VarTbl = System.Collections.Generic.SortedDictionary<int, compiler.middleend.ir.SsaVariable>;

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
            ProgramCfg = new Cfg();
            FunctionsCfgs = new List<Cfg>();
            VarTable = new VarTbl();
        }

        public Token Tok { get; set; }

        public Lexer Scanner { get; set; }

        public int Pos => Scanner.Position;

        public int LineNo => Scanner.LineNo;

        public int CurrAddress { get; set; }

        public VarTbl VarTable { get; set; }


        /// <summary>
        ///     A stack of frame addresses -- esentially a list of frame pointers
        /// </summary>
        public List<int> AddressStack { get; set; }


        public Cfg ProgramCfg { get; set; }

        public List<Cfg> FunctionsCfgs { get; set; }

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

        public ParseResult Designator(VarTbl variables)
        {
            //public Operand Designator()
            Operand originalId = Identifier();
            Operand id = originalId;
            var instructions = new List<Instruction>();
            //ParseResult ret = new ParseResult(id,instructions);
            
            // gen load addr of id
            //var baseAddr = new Instruction(IrOps.load, new Operand(Operand.OpType.Identifier, Scanner.Id), null);

            //TODO handle generating array addresses
            while (Tok == Token.OPEN_BRACKET)
            {
                // load array base address

                var baseAddr = new Instruction(IrOps.Load, id, null);
                instructions.Add(baseAddr);
                id = new Operand(baseAddr);


                GetExpected(Token.OPEN_BRACKET);

                // calulate offset

                ParseResult exp = Expression(variables);
                instructions.AddRange(exp.Instructions);

                //add offset to addr of id
                //instructions.Add(new Instruction(IrOps.adda, id,  new Operand(instructions.Last())));
                instructions.Add(new Instruction(IrOps.Adda, id, exp.Operand));

                // Delay the indirect load until the top of the loop so we can do the last
                // load or store at the reference site.

                //inderect load result of basaddr + offset
                //baseAddr = new Instruction(IrOps.load, new Operand(instructions.Last()), null);
                //instructions.Add(baseAddr);
                //id = new Operand(baseAddr);


                //get bracket
                GetExpected(Token.CLOSE_BRACKET);

                // set the current operand to the last adda inst, so we can get the load/store right at the end
                id = new Operand(instructions.Last());
            }

            if (variables.ContainsKey(id.IdKey))
            {
				id = new Operand(variables[id.IdKey]);
                /*if (temp != null)
                {
                    id = new Operand(temp);
                }//*/
            }

            return new ParseResult(id, instructions, variables);
        }

        public ParseResult Factor(VarTbl variables)
        {
            ParseResult factor;
            var instructions = new List<Instruction>();
            Operand id;

            switch (Tok)
            {
                case Token.NUMBER:
                    id = Num();
                    factor = new ParseResult(id, new List<Instruction>(), variables);
                    break;
                case Token.IDENTIFIER:
                    var des = Designator(variables);
                    instructions.AddRange(des.Instructions);
                    //Operand arg2 = (instructions.Count == 0) ? null : new Operand(instructions.Last())

                    var baseAddr = new Instruction(IrOps.Load, des.Operand, null);
                    instructions.Add(baseAddr);
                    id = new Operand(baseAddr);
                    factor = new ParseResult(id, instructions, des.VarTable);
                    break;
                case Token.OPEN_PAREN:
                    Next();
                    factor = Expression(variables);
                    GetExpected(Token.CLOSE_PAREN);
                    break;
                case Token.CALL:
                    factor = FuncCall(variables);
                    break;
                default:
                    FatalError();
                    factor = new ParseResult(new Operand(Operand.OpType.Constant, 0xDEAD),
                        instructions, variables);
                    break;
            }

            return factor;
        }


        public ParseResult Term(VarTbl variables)
        {
            ParseResult factor1 = Factor(variables);
            List<Instruction> instructions = factor1.Instructions;
            //Instruction curr = instructions.Last();
            var locals = factor1.VarTable;

            while ((Tok == Token.TIMES) || (Tok == Token.DIVIDE))
            {
                // cache current arithmetic token
                IrOps op = Tok == Token.TIMES ? IrOps.Mul : IrOps.Div;

                // advance to next token
                Next();

                // add instructions for the next factor
                ParseResult factor2 = Factor(locals);
                instructions.AddRange(factor2.Instructions);

                //TODO: find a good way of making this so we can support immediate actions
                Operand id;
                if ((factor2.Operand.Kind == Operand.OpType.Constant) && (factor1.Operand.Kind == Operand.OpType.Constant))
                {
                    int arg2 = factor2.Operand.Val;
                    int arg1 = factor1.Operand.Val;
                    int res = op == IrOps.Mul ? arg1 * arg2 : arg1 / arg2;
                    id = new Operand(Operand.OpType.Constant, res);
                    //var ret = new ParseResult(new Operand(Operand.OpType.Constant, res), new List<Instruction>() );
                }
                else
                {
                    var newInst = new Instruction(op, factor1.Operand, factor2.Operand);

                    // insert new instruction to instruction list
                    instructions.Add(newInst);
                    id = new Operand(newInst);
                }

                factor1 = new ParseResult(id, instructions, locals);
            }

            return factor1;
        }


        public ParseResult Expression(VarTbl variables)
        {
            ParseResult term1 = Term(variables);
            Operand id = term1.Operand;
            List<Instruction> instructions = term1.Instructions;
            var locals = new VarTbl(term1.VarTable);

            //Instruction curr = term1.Item2.Last();

            while ((Tok == Token.PLUS) || (Tok == Token.MINUS))
            {
                // cache current arithmetic token
                IrOps op = Tok == Token.PLUS ? IrOps.Add : IrOps.Sub;

                // advance to next token
                Next();

                // add instructions for the next term
                ParseResult term2 = Term(locals);
                instructions.AddRange(term2.Instructions);

                if ((term2.Operand.Kind == Operand.OpType.Constant) && (term1.Operand.Kind == Operand.OpType.Constant))
                {
                    int arg2 = term2.Operand.Val;
                    int arg1 = term1.Operand.Val;
                    int res = op == IrOps.Add ? arg1 + arg2 : arg1 - arg2;
                    id = new Operand(Operand.OpType.Constant, res);
                    //var ret = new ParseResult(new Operand(Operand.OpType.Constant, res), new List<Instruction>() );
                }
                else
                {
                    var newInst = new Instruction(op, id, term2.Operand);

                    // insert new instruction to instruction list
                    instructions.Add(newInst);
                    id = new Operand(newInst);
                }

                term1 = new ParseResult(id, instructions, locals);
            }

            return term1;
        }

        public ParseResult Assign(ref VarTbl variables)
        {
            //List<Instruction> ret = new List<Instruction>();

            //TODO: assign must use SSA, so our designator *MUST* give us access to an SSA variable


            GetExpected(Token.LET);

            ParseResult id = Designator( variables);
			var locals = variables;
            //var locals = new VarTbl(id.VarTable);
            //Instruction curr = ret.Last();

            GetExpected(Token.ASSIGN);

            //ret.AddRange(Expression());
            //Instruction next = ret.Last();

            ParseResult expValue = Expression(locals);

            //TODO: Fix this!!!!

            // create new instruction
            var newInst = new Instruction(IrOps.Store, expValue.Operand, id.Operand);
            Instruction prev = null;
            string name = Scanner.SymbolTble.Symbols[id.Operand.IdKey];

            if (locals.ContainsKey(id.Operand.IdKey))
            {
                prev = locals[id.Operand.IdKey].Location;
                
            }

            SsaVariable ssa = new SsaVariable(id.Operand.IdKey, newInst, prev, name);
            id.Operand.Inst = newInst;
            id.Operand.Variable = ssa;
			var ssaArg = new Operand(ssa);
			newInst.Arg2.Inst = newInst;



            locals[id.Operand.IdKey] = ssa;

            id.Instructions.AddRange(expValue.Instructions);

            // insert new instruction to instruction list
            id.Instructions.Add(newInst);

			// update current instruction to latest instruction
			//curr = ret.Last();

			return new ParseResult(ssaArg, id.Instructions, locals);
        }

        public Cfg Computation(VarTbl varTble)
        {
            GetExpected(Token.MAIN);

            var cfg = new Cfg();

            while ((Tok == Token.VAR) || (Tok == Token.ARRAY))
            {
                varTble = VarDecl(varTble);
            }

            while ((Tok == Token.FUNCTION) || (Tok == Token.PROCEDURE))
            {
				
                // throw away cFG for now
                //cfg.Insert( FuncDecl() );
                Cfg func = FuncDecl(new VarTbl(varTble));
                if (func.Root != null)
                {
                    FunctionsCfgs.Add(func);
                }
                //FuncDecl();
            }

            GetExpected(Token.OPEN_CURL);

            cfg.Insert(StatementSequence(ref varTble));

            GetExpected(Token.CLOSE_CURL);

            GetExpected(Token.EOF);

            return cfg;
        }

        public ParseResult Relation(VarTbl variables)
        {
            ParseResult leftVal = Expression(variables);
            // copy instructions from first expression
            var instructions = new List<Instruction>(leftVal.Instructions);
            Operand arg1 = leftVal.Operand;

            if (!IsRelOp())
            {
                FatalError();
            }

            Token comp = Tok;

            Next();

            ParseResult rightVal = Expression(variables);

            // copy instructions from right expression
            instructions.AddRange(rightVal.Instructions);
            Operand arg2 = rightVal.Operand;

            //var newOperand = new Operand(instructions);

            var compare = new Instruction(IrOps.Cmp, arg1, arg2);
            instructions.Add(compare);
            var branch = new Instruction(IrOps.Bne, new Operand(compare), new Operand(Operand.OpType.Constant, 0));
            instructions.Add(branch);


            // set the correct IR op code
            switch (comp)
            {
                case Token.EQUAL:
                    branch.Op = IrOps.Bne;
                    break;
                case Token.NOT_EQUAL:
                    branch.Op = IrOps.Beq;
                    break;
                case Token.LESS:
                    branch.Op = IrOps.Bge;
                    break;
                case Token.LESS_EQ:
                    branch.Op = IrOps.Bgt;
                    break;
                case Token.GREATER:
                    branch.Op = IrOps.Ble;
                    break;
                case Token.GREATER_EQ:
                    branch.Op = IrOps.Blt;
                    break;
                default:
                    FatalError();
                    break;
            }

            return new ParseResult(new Operand(branch), instructions, variables);
        }

        public Operand Identifier()
        {
            int id = Scanner.Id;
            GetExpected(Token.IDENTIFIER);

            return new Operand(Operand.OpType.Identifier, id);
        }

        public void CreateIdentifier()
        {
            int id = Scanner.Id;
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


        public VarTbl VarDecl(VarTbl varTble)
        {
            // TODO: allocate variables here
            var size = TypeDecl();

            // TODO: this is where we need to set variable addresses
            //CreateIdentifier();
            var id = Identifier();
            varTble.Add(id.IdKey, new SsaVariable(id.IdKey,null,null, Scanner.SymbolTble.Symbols[id.IdKey]));

            while (Tok == Token.COMMA)
            {
                Next();

                //CreateIdentifier();
                id = Identifier();
                varTble.Add(id.IdKey, new SsaVariable(id.IdKey, null, null, Scanner.SymbolTble.Symbols[id.IdKey]));
            }
            
            GetExpected(Token.SEMI_COLON);
            
            return varTble;
        }

        public int TypeDecl()
        {
            // TODO: determine size of allocation required
            var size = 4;
            if (Tok == Token.VAR)
            {
                Next();
                //size = 4;
            }
            else if (Tok == Token.ARRAY)
            {
                Next();

                GetExpected(Token.OPEN_BRACKET);

                Operand n = Num();
                size *= n.Val;

                GetExpected(Token.CLOSE_BRACKET);

                while (Tok == Token.OPEN_BRACKET)
                {
                    Next();

                    n = Num();
                    size *= n.Val;

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

        public Cfg FuncDecl(VarTbl variables)
        {
            var cfg = new Cfg();

            if ((Tok != Token.FUNCTION) && (Tok != Token.PROCEDURE))
            {
                FatalError();
            }

            Next();

            //TODO: Need a special address thing for functions
            //CreateIdentifier();
            Operand id = Identifier();
            List<Operand> paramList = null;

            if (Tok == Token.OPEN_PAREN)
            {
                paramList = FormalParams();
            }

            GetExpected(Token.SEMI_COLON);

            Cfg fb = FuncBody(variables);
            
            if (fb != null)
            {
                fb.Name = Scanner.SymbolTble.Symbols[id.IdKey];
                cfg.Insert(fb);
                cfg.Name = fb.Name;
            }

            GetExpected(Token.SEMI_COLON);

            return cfg;
        }

        public Cfg FuncBody(VarTbl ssaTable)
        {
            Cfg cfg = null;
            while ((Tok == Token.VAR) || (Tok == Token.ARRAY))
            {
                ssaTable = VarDecl(ssaTable);
            }

            GetExpected(Token.OPEN_CURL);

            if ((Tok == Token.LET) || (Tok == Token.CALL) || (Tok == Token.IF)
                || (Tok == Token.WHILE) || (Tok == Token.RETURN))
            {
                cfg = StatementSequence(ref ssaTable);
            }

            GetExpected(Token.CLOSE_CURL);

            return cfg;
        }


        public Cfg Statement(ref VarTbl variables)
        {
            //TODO: CFG has trouble adding to new blocks, or inserting into CFG
            var cfgTemp = new Cfg {Root = new Node(new BasicBlock("StatementBlock"))};
            // TODO: address what to do with return opperand;
            ParseResult stmt;


            switch (Tok)
            {
                case Token.LET:
                    stmt = Assign(ref variables);
                    cfgTemp.Root.Bb.AddInstructionList(stmt.Instructions);
                    break;
                case Token.CALL:
                    stmt = FuncCall(variables);
                    cfgTemp.Root.Bb.AddInstructionList(stmt.Instructions);
                    //cfgTemp.Root.BB.AddInstructionList(FuncCall());
                    break;
                case Token.IF:
                    return IfStmt(variables);
                case Token.WHILE:
                    return WhileStmt(variables);
                case Token.RETURN:
                    stmt = ReturnStmt(variables);
                    cfgTemp.Root.Bb.AddInstructionList(stmt.Instructions);
                    break;
                default:
                    FatalError();
                    break;
            }

            return cfgTemp;
        }


        public Cfg StatementSequence(ref VarTbl variables)
        {
            var cfg = new Cfg();
            var bb = new BasicBlock("StatSequence");
            cfg.Root = new Node(bb);
            var stmt = Statement(ref variables);
            cfg.Insert(stmt);

            // TODO: fix consolodate()
            cfg.Root.Consolidate();

            while (Tok == Token.SEMI_COLON)
            {
                Next();
                stmt = Statement(ref variables);
                cfg.Insert(stmt);

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
        

        public ParseResult FuncCall(VarTbl variables)
        {
            GetExpected(Token.CALL);

            Operand id = Identifier();
            var instructions = new List<Instruction>();

            var paramList = new List<ParseResult>();

            if (Tok == Token.OPEN_PAREN)
            {
                GetExpected(Token.OPEN_PAREN);

                if ((Tok == Token.IDENTIFIER) ||
                    (Tok == Token.NUMBER) ||
                    (Tok == Token.OPEN_PAREN) ||
                    (Tok == Token.CALL))
                {
                    paramList.Add(Expression(variables));

                    while (Tok == Token.COMMA)
                    {
                        Next();
                        paramList.Add(Expression(variables));
                    }

                    // do something with the param list to push items on stack for call
                }

                GetExpected(Token.CLOSE_PAREN);
                //TODO: jump to call
            }

            foreach (ParseResult item in paramList)
            {
                instructions.AddRange(item.Instructions);
            }

            var call = new Instruction(IrOps.Bra, id, null);
            id = new Operand(call);

            instructions.Add(call);


            return new ParseResult(id, instructions, variables);
        }

        public Cfg IfStmt(VarTbl variables)
        {
            GetExpected(Token.IF);
            var ifBlock = new Cfg();
            var compBlock = new CompareNode(new BasicBlock("CompareBlock"));

            var joinBlock = new JoinNode(new BasicBlock("JoinBlock"));
            Node falseBlock = joinBlock;

            var trueSsa = new VarTbl(variables);
            var falseSsa = new VarTbl(variables);

            // HACK: should we use the operand of the relation?
            compBlock.Bb.AddInstructionList(Relation(variables).Instructions);

            GetExpected(Token.THEN);

            ifBlock.Insert(compBlock);

            // pass in a copy of variables so the original stays pristine
            Node trueBlock = StatementSequence(ref trueSsa).Root;
            trueBlock.Consolidate();

            compBlock.InsertTrue(trueBlock);
            trueBlock.Leaf().InsertJoinTrue(joinBlock);
			bool elseBranch = false;
            if (Tok == Token.ELSE)
            {
                Next();
                falseBlock = StatementSequence(ref falseSsa).Root;
                Node.Leaf(falseBlock).InsertJoinFalse(joinBlock);
				falseBlock.Consolidate();
				elseBranch = true;
            }


            compBlock.InsertFalse(falseBlock);

            GetExpected(Token.FI);

            // insert Phi instructions where items from true ssa and false ssa are different
            foreach (var trueVar in trueSsa)
            {
                var falseVar = falseSsa[trueVar.Key];
                if ( falseVar != trueVar.Value)
                {
					var newInst = new Instruction(IrOps.Phi, new Operand(trueVar.Value.Location), new Operand(falseVar.Location));
                    joinBlock.Bb.Instructions.Add(newInst);

					var temp = new SsaVariable(variables[trueVar.Key]);
					temp.Location = newInst;
					// Assume trueSsa and falseSsa are both the same size
					variables[trueVar.Key] = temp;

                }
            }

			//throw exception if size is different
			if (trueSsa.Count != falseSsa.Count)
			{
				throw new Exception("SSA Variable Tables are different sizes. You added something you shouldnt have.");
			}

			if (joinBlock.Bb.Instructions.Count == 0)
			{
				var fakePhi = new Instruction(IrOps.Phi, new Operand(Operand.OpType.Identifier, 0), new Operand(Operand.OpType.Identifier, 0));
				joinBlock.Bb.Instructions.Add(fakePhi);
			}

			if (elseBranch)
			{
				// The branch location isn't known yet, so delay it
				trueBlock.Bb.AddInstruction(new Instruction(IrOps.Bra, new Operand(joinBlock.GetNextInstruction()), null));
			}

            compBlock.Bb.Instructions.Last().Arg2 = new Operand(falseBlock.GetNextInstruction());

            return ifBlock;
        }





        public Cfg WhileStmt(VarTbl variables)
        {
            GetExpected(Token.WHILE);

			var loopSsa = new VarTbl(variables);
			var followSsa = new VarTbl(variables);

            // create cfg
            var whileBlock = new Cfg();

            //crate compare block/loop header block
            var compBlock = new WhileNode(new BasicBlock("WhileCompareBlock"));

            // TODO: Correct placeholder Phi Instruction
            compBlock.Bb.AddInstruction(new Instruction(IrOps.Phi, new Operand(Operand.OpType.Identifier, 0),
                new Operand(Operand.OpType.Identifier, 0)));

            // insert compare block for while stmt
            whileBlock.Insert(compBlock);

            // add the relation/branch comparison into the loop header block
            // HACK: should we use the opperand of the relation?
            compBlock.Bb.AddInstructionList(Relation(variables).Instructions);

            GetExpected(Token.DO);

            // prepare basic block for loop body
            Cfg stmts = StatementSequence(ref variables);

            Node loopBlock = stmts.Root;
            loopBlock.Bb.AddInstruction(new Instruction(IrOps.Bra, new Operand(compBlock.GetNextInstruction()), null));
            loopBlock.Consolidate();
            Node last = loopBlock.Leaf();

            //TODO: try to refactor so that we don't have to insert on the false branch
            // insert the loop body on the true path
            compBlock.InsertTrue(loopBlock);

            last.Child = compBlock;
            compBlock.LoopParent = last;

            GetExpected(Token.OD);

            var followBlock = new Node(new BasicBlock("FollowBlock"));

            compBlock.InsertFalse(followBlock);


            //TODO: remove placeholder instruction and do something smarter
            followBlock.Bb.AddInstruction(new Instruction(IrOps.Phi, new Operand(Operand.OpType.Identifier, 0),
                new Operand(Operand.OpType.Identifier, 0)));

            last.GetLastInstruction().Arg2 = new Operand(compBlock.GetNextInstruction());

            // TODO: this is straight up wrong. we can leave this alone and fix it in the enclosing scope
			compBlock.GetLastInstruction().Arg2 = new Operand(followBlock.Bb.Instructions.First());

            return whileBlock;
        }


        //TODO: Maybe pass in a return address?
        private ParseResult ReturnStmt(VarTbl variables)
        {
            GetExpected(Token.RETURN);

            var instructions = new List<Instruction>();

            ParseResult retStmt = null;

            if ((Tok == Token.IDENTIFIER) || (Tok == Token.NUMBER) || (Tok == Token.OPEN_PAREN) || (Tok == Token.CALL))
            {
                retStmt = Expression(variables);
                instructions = retStmt.Instructions;
            }

            //TODO: probably want to make better instruction here, with a real address
            var branchBack = new Instruction(IrOps.Bra, new Operand(Operand.OpType.Register, 0), null);
            instructions.Add(branchBack);
            if (retStmt == null)
            {
                retStmt = new ParseResult(new Operand(instructions.Last()), instructions, variables);
            }
            else
            {
                retStmt = new ParseResult(new Operand(branchBack), instructions, variables);
            }

            return retStmt;
        }

        public List<Operand> FormalParams()
        {
            GetExpected(Token.OPEN_PAREN);

            var paramList = new List<Operand>();

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
            ProgramCfg = Computation(new VarTbl());
            FunctionsCfgs.Add(ProgramCfg);
            ProgramCfg.Name = "Main";
            ProgramCfg.Sym = Scanner.SymbolTble;
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
