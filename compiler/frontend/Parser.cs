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
        private readonly bool _copyPropagationEnabled;

        public Parser(string pFileName, bool pCopyPropEnabled)
        {
            _filename = pFileName;
            _copyPropagationEnabled = pCopyPropEnabled;

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

        private void FatalError()
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
                //id = variables[id.IdKey].Value
                /*if (temp != null)
                {
                    id = new Operand(temp);
                }//*/
            }

            return new ParseResult(id, instructions, variables);
        }

        private ParseResult Factor(VarTbl variables)
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
                    ParseResult des = Designator(variables);
                    instructions.AddRange(des.Instructions);
                    //Operand arg2 = (instructions.Count == 0) ? null : new Operand(instructions.Last())
                    if (_copyPropagationEnabled && (des.Operand.Kind == Operand.OpType.Variable))
                    {
                        id = new Operand(des.Operand.Variable.Location);
						if ((id.Inst != null) && (id.Inst.Op == IrOps.Ssa))
                        {
                            // TODO: please solve how to do copy propagation -- next 3 lines
                            //id = new Operand(id.Inst.Arg2.Variable.Location); // doesn't propagate to original value 
                            //id = id.Inst.Arg2.Variable.Value;
                            //id = id.Inst.Arg2.Variable.Location.Arg2.Variable.Value; // kills references to aliased variables

                            id = new Operand(id.Inst.Arg2.Variable);
                        }
                    }
                    else
                    {
                        var baseAddr = new Instruction(IrOps.Load, des.Operand, null);
                        instructions.Add(baseAddr);
                        id = new Operand(baseAddr);
                    }
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


        private ParseResult Term(VarTbl variables)
        {
            ParseResult factor1 = Factor(variables);
            List<Instruction> instructions = factor1.Instructions;
            //Instruction curr = instructions.Last();
            VarTbl locals = factor1.VarTable;

            while ((Tok == Token.TIMES) || (Tok == Token.DIVIDE))
            {
                // cache current arithmetic token
                IrOps op = Tok == Token.TIMES ? IrOps.Mul : IrOps.Div;

                // advance to next token
                Next();

                // add instructions for the next factor
                ParseResult factor2 = Factor(locals);
                instructions.AddRange(factor2.Instructions);

                Operand id;
                if ((factor2.Operand.Kind == Operand.OpType.Constant) &&
                    (factor1.Operand.Kind == Operand.OpType.Constant))
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


        private ParseResult Expression(VarTbl variables)
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

        private ParseResult Assign(ref VarTbl variables)
        {
            GetExpected(Token.LET);

            ParseResult id = Designator(variables);
            VarTbl locals = variables;

            GetExpected(Token.ASSIGN);

            ParseResult expValue = Expression(locals);

            // create new instruction
            // TODO: decide if this is ssa, and change irops.store to irops.ssa
            Instruction newInst = new Instruction(IrOps.Store, expValue.Operand, id.Operand);
            Instruction prev = null;
            string name = Scanner.SymbolTble.Symbols[id.Operand.IdKey];


            id.Instructions.AddRange(expValue.Instructions);

            Operand arg;

            // check symbol "tables"
            if (locals.ContainsKey(id.Operand.IdKey))
            {
                prev = locals[id.Operand.IdKey].Location;

                var ssa = new SsaVariable(id.Operand.IdKey, newInst, prev, name);
                id.Operand.Inst = newInst;
                id.Operand.Variable = ssa;

                newInst.Arg2.Inst = newInst;
				newInst.Op = IrOps.Ssa;

                // try to use ssa value
                //ssa.Value = newInst.Arg1;
                ssa.Value = newInst.Arg1.OpenOperand();

                if (_copyPropagationEnabled && (ssa.Value.Kind == Operand.OpType.Constant))
                {
                    ssa.Value = new Operand(ssa.Location);
                }

                locals[id.Operand.IdKey] = ssa;
                //arg = new Operand(ssa);
                arg = ssa.Value;
            }
            else
            {
                //Otherwise it must be an array
                arg = new Operand(newInst);
            }

            // insert new instruction to instruction list
            id.Instructions.Add(newInst);

            return new ParseResult(arg, id.Instructions, locals);
        }

        private Cfg Computation(VarTbl varTble)
        {
            GetExpected(Token.MAIN);

            var cfg = new Cfg();

            while ((Tok == Token.VAR) || (Tok == Token.ARRAY))
            {
                varTble = VarDecl(varTble);
            }

            while ((Tok == Token.FUNCTION) || (Tok == Token.PROCEDURE))
            {
                Cfg func = FuncDecl(new VarTbl(varTble));
                if (func.Root != null)
                {
                    FunctionsCfgs.Add(func);
                }
            }

            GetExpected(Token.OPEN_CURL);

            cfg.Insert(StatementSequence(ref varTble));

            GetExpected(Token.CLOSE_CURL);

            GetExpected(Token.EOF);
            var end = new Instruction(IrOps.End, null, null);
            end.Arg1 = new Operand(end);
            cfg.Root.Leaf().Bb.AddInstruction(end);

            return cfg;
        }

        private ParseResult Relation(VarTbl variables)
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

        private Operand Identifier()
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


        private int NextAddress()
        {
            //TODO: implement this function
            return 0;
        }

        private Operand Num()
        {
            GetExpected(Token.NUMBER);

            //return new Instruction(IrOps.load, new Operand(Operand.OpType.Constant, Scanner.Val), null);
            return new Operand(Operand.OpType.Constant, Scanner.Val);
        }


        private VarTbl VarDecl(VarTbl varTble)
        {
            // TODO: allocate variables here
            int size = TypeDecl();

            // TODO: this is where we need to set variable addresses
            //CreateIdentifier();
            Operand id = Identifier();
            varTble.Add(id.IdKey, new SsaVariable(id.IdKey, null, null, Scanner.SymbolTble.Symbols[id.IdKey]));

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

        private int TypeDecl()
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

        private Cfg FuncDecl(VarTbl variables)
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

        private Cfg FuncBody(VarTbl ssaTable)
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


        private Cfg Statement(ref VarTbl variables)
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


        private Cfg StatementSequence(ref VarTbl variables)
        {
            var cfg = new Cfg();
            var bb = new BasicBlock("StatSequence");
            cfg.Root = new Node(bb);
            Cfg stmt = Statement(ref variables);
            cfg.Insert(stmt);

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


        private ParseResult FuncCall(VarTbl variables)
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

        private Cfg IfStmt(VarTbl variables)
        {
            GetExpected(Token.IF);
            var ifBlock = new Cfg();
            var compBlock = new CompareNode(new BasicBlock("CompareBlock"));

            var joinBlock = new JoinNode(new BasicBlock("JoinBlock"));
            Node falseBlock = joinBlock;
            compBlock.Join = joinBlock;

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
            var elseBranch = false;
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

            AddPhiInstructions(variables, trueSsa, falseSsa, joinBlock, false);

            if (joinBlock.Bb.Instructions.Count == 0)
            {
                var fakePhi = new Instruction(IrOps.Phi, new Operand(Operand.OpType.Identifier, 0),
                    new Operand(Operand.OpType.Identifier, 0));
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

        private static void AddPhiInstructions(VarTbl variables, VarTbl trueSsa, VarTbl falseSsa, Node phiBlock,
            bool isLoop)
        {
            var phiList = new List<Instruction>();
            // insert Phi instructions where items from true ssa and false ssa are different
            foreach (KeyValuePair<int, SsaVariable> trueVar in trueSsa)
            {
                //throw exception if size is different
                if ((trueSsa.Count != falseSsa.Count) || (trueSsa.Count != variables.Count))
                {
                    throw new Exception(
                        "SSA Variable Tables are different sizes. You added something you shouldnt have.");
                }

                SsaVariable falseVar = falseSsa[trueVar.Key];
                if (falseVar != trueVar.Value)
                {
                    // This top construction seems to be correct, and should give the best answer, but doesnt
                    var newInst = new Instruction(IrOps.Phi, trueVar.Value.Value,
                        falseVar?.Value ?? new Operand(falseVar.Location));
                    //var newInst = new Instruction(IrOps.Phi, new Operand(trueVar.Value.Location), new Operand(falseVar.Location));

                    // handle these commented out lines in the constructor for instruction
                    //trueVar.Value.Location.Uses.Add(newInst.Arg1);
                    //falseVar.Location.Uses.Add(newInst.Arg2);

                    phiList.Add(newInst);
                    if (isLoop)
                    {
                        FixLoopPhi(phiBlock.Child, newInst);
                    }

                    // use object initializer
                    var temp = new SsaVariable(variables[trueVar.Key])
                    {
                        Location = newInst,
                        Value = new Operand(newInst)
                    };

                    // Assume trueSsa and falseSsa are both the same size
                    variables[trueVar.Key] = temp;
                }
            }
            phiBlock.Bb.InsertInstructionList(0, phiList);
        }


        private Cfg WhileStmt(VarTbl variables)
        {
            GetExpected(Token.WHILE);

            var loopSsa = new VarTbl(variables);
            var headerSsa = new VarTbl(variables);

            // create cfg
            var whileBlock = new Cfg();

            //crate compare block/loop header block
            var compBlock = new WhileNode(new BasicBlock("LoopHeader"));

            // insert compare block for while stmt
            whileBlock.Insert(compBlock);

            // add the relation/branch comparison into the loop header block
            // HACK: should we use the opperand of the relation?
            compBlock.Bb.AddInstructionList(Relation(headerSsa).Instructions);

            GetExpected(Token.DO);

            // prepare basic block for loop body
            Cfg stmts = StatementSequence(ref loopSsa);

            Node loopBlock = stmts.Root;
            loopBlock.Consolidate();

            Node last = loopBlock.Leaf();
            last.Bb.AddInstruction(new Instruction(IrOps.Bra, new Operand(compBlock.GetNextInstruction()), null));

            // insert the loop body on the true path
            compBlock.InsertTrue(loopBlock);

            last.Child = compBlock;
            compBlock.LoopParent = last;

            GetExpected(Token.OD);

            var followBlock = new Node(new BasicBlock("FollowBlock")) {Colorname = "palegreen"};

            compBlock.InsertFalse(followBlock);

            AddPhiInstructions(variables, loopSsa, headerSsa, compBlock, true);

            //TODO: remove placeholder instruction and do something smarter
            followBlock.Bb.AddInstruction(new Instruction(IrOps.Phi, new Operand(Operand.OpType.Identifier, 0),
                new Operand(Operand.OpType.Identifier, 0)));

            Instruction inst = last.Bb.Instructions.Last();

            if (inst.Op != IrOps.Bra)
            {
                inst.Arg2 = new Operand(compBlock.GetNextInstruction());
            }

            // TODO: this is straight up wrong. we can leave this alone and fix it in the enclosing scope
            compBlock.Bb.Instructions.Last().Arg2 = new Operand(followBlock.Bb.Instructions.First());

            return whileBlock;
        }

        // TODO: Loops must have the instructions referenced in their phi's updated
        private static void FixLoopPhi(Node n, Instruction phi)
        {
            var visited = new HashSet<Node>();
            LoopFix(n, phi, visited);
        }


        private static void LoopFix(Node n, Instruction phi, HashSet<Node> visited)
        {
            // base case
            if (visited.Contains(n) || (n == null))
            {
                return;
            }

            // recursive case
            visited.Add(n);

            // loop through instructions looking for places to replace ref with phi instructions
            foreach (Instruction inst in n.Bb.Instructions)
            {
                if (inst.Num != phi.Num)
                {
                    if (CheckOperand(inst.Arg1, phi.Arg1) || CheckOperand(inst.Arg1, phi.Arg2))
                    {
                        inst.Arg1 = new Operand(phi);
                    }

                    if (CheckOperand(inst.Arg2, phi.Arg1) || CheckOperand(inst.Arg2, phi.Arg2))
                    {
						if (inst.Op != IrOps.Ssa)
                        {
                            inst.Arg2 = new Operand(phi);
                        }
                    }
                }
            }

            List<Node> children = n.GetAllChildren();
            foreach (Node child in children)
            {
                LoopFix(child, phi, visited);
            }
        }

        private static bool CheckOperand(Operand checkedOp, Operand phiArg)
        {
            if (checkedOp == phiArg)
            {
                return true;
            }

            if (checkedOp == null)
            {
                return false;
            }

            if (checkedOp.Kind == Operand.OpType.Variable)
            {
                if (checkedOp.Variable.Location == phiArg?.Inst)
                {
                    return true;
                }
            }
            return false;
        }


        public Tuple<BasicBlock, int> FindInstruction(Instruction inst, Node n)
        {
            if (n == null)
            {
                return null;
            }

            List<Instruction> instList = n.Bb.Instructions;

            for (var i = 0; i < instList.Count; i++)
            {
                if (inst == instList[i])
                {
                    return new Tuple<BasicBlock, int>(n.Bb, i);
                }
            }

            return FindInstruction(inst, n.Parent);
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

        private List<Operand> FormalParams()
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