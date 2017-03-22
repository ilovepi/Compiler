#region Basic header

// MIT License
// 
// Copyright (c) 2016 Paul Kirth
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using compiler.middleend.ir;
using VarTbl = System.Collections.Generic.SortedDictionary<int, compiler.middleend.ir.SsaVariable>;

#endregion

namespace compiler.frontend
{
    public class Parser : IDisposable
    {
        public static int nestingDepth = 1;
        private readonly bool _copyPropagationEnabled;
        private readonly string _filename;

        public HashSet<string> Callgraph;

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

        private void FatalError(string msg)
        {
            throw ParserException.CreateParserException(msg, LineNo, Pos, _filename);
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

            if (variables.ContainsKey(id.IdKey))
            {
                SsaVariable cached = variables[id.IdKey];
                id = new Operand(cached);
            }


            var indiciesList = new List<Operand>();
            var arrayCount = 0;
            ArrayType ary = null;

            while (Tok == Token.OPEN_BRACKET)
            {
                // throw error if the variable isn't an array
                if (!id.Variable.Identity.IsArray)
                {
                    FatalError();
                }

                if (ary == null)
                {
                    ary = (ArrayType) id.Variable.Identity;
                }

                GetExpected(Token.OPEN_BRACKET);


                // calulate offset

                ParseResult exp = Expression(variables);
                instructions.AddRange(exp.Instructions);

                // if we arn't the last index, generate a multiply
                if (arrayCount < ary.Dimensions.Count)
                {
                    var offset = 1;
                    for (int i = arrayCount + 1; i < ary.Dimensions.Count; i++)
                    {
                        offset *= ary.Dimensions[i];
                    }

                    var mulInst = new Instruction(IrOps.Mul, exp.Operand, new Operand(Operand.OpType.Constant, offset));
                    instructions.Add(mulInst);
                    exp.Operand = new Operand(mulInst);


                    if (arrayCount != 0)
                    {
                        var addInst = new Instruction(IrOps.Add, indiciesList.Last(), exp.Operand);
                        instructions.Add(addInst);
                        var addOp = new Operand(addInst);
                        indiciesList.Add(addOp);
                        exp.Operand = addOp;
                        if (arrayCount == (ary.Dimensions.Count - 1))
                        {
                            instructions.Add(new Instruction(IrOps.Adda, id, exp.Operand));
                            exp.Operand.Variable = id.Variable;
                        }
                    }
                    else
                    {
                        if (arrayCount == (ary.Dimensions.Count - 1))
                        {
                            instructions.Add(new Instruction(IrOps.Adda, id, exp.Operand));
                            exp.Operand.Variable = id.Variable;
                        }
                        indiciesList.Add(exp.Operand);
                    }
                }


                //get bracket
                GetExpected(Token.CLOSE_BRACKET);
                arrayCount++;
            }

            if (arrayCount > 0)
            {
                id = new Operand(instructions.Last());
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
                            id = new Operand(id.Inst.Arg2.Variable);
                        }
                    }
                    else
                    {
                        //TODO: determine how to reinstate loads for parameters
                        var baseAddr = new Instruction(IrOps.Load, des.Operand, null);
                        //baseAddr.Arg1.Variable.Identity;
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
                if (false && (factor2.Operand.Kind == Operand.OpType.Constant) &&
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

                if (false && (term2.Operand.Kind == Operand.OpType.Constant) && (term1.Operand.Kind == Operand.OpType.Constant))
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
            var newInst = new Instruction(IrOps.Store, expValue.Operand, id.Operand);

            string name = Scanner.SymbolTble.Symbols[id.Operand.IdKey];


            id.Instructions.AddRange(expValue.Instructions);

            Operand arg;

            // check symbol "tables"
            if (locals.ContainsKey(id.Operand.IdKey))
            {
                Instruction prev = locals[id.Operand.IdKey].Location;

                var ssa = new SsaVariable(id.Operand.IdKey, newInst, prev, name)
                {
                    Identity = id.VarTable[id.Operand.IdKey].Identity
                };
                id.Operand.Inst = newInst;
                id.Operand.Variable = ssa;

                newInst.Arg2.Inst = newInst;
                newInst.Op = IrOps.Ssa;

                // try to use ssa value
                ssa.Value = newInst.Arg1;
                //ssa.Value = newInst.Arg1.OpenOperand();

                if (_copyPropagationEnabled && (ssa.Value.Kind == Operand.OpType.Constant))
                {
                    //ssa.Value = new Operand(ssa.Location);
                }

                locals[id.Operand.IdKey] = ssa;
                //arg = new Operand(ssa);
                arg = ssa.Value;
            }
            else
            {
                //Otherwise it must be an array
                arg = new Operand(newInst);

                if ((newInst.Arg2.Kind == Operand.OpType.Instruction) && (newInst.Arg2.Inst.Op == IrOps.Adda))
                {
                    Instruction temp = newInst.Arg2.Inst;
                    id.Instructions.Remove(temp);
                    id.Instructions.Add(temp);
                }
            }

            // insert new instruction to instruction list
            id.Instructions.Add(newInst);

            return new ParseResult(arg, id.Instructions, locals);
        }

        private Cfg Computation(VarTbl varTble)
        {
            GetExpected(Token.MAIN);

            var cfg = new Cfg
            {
                Locals = new List<VariableType>(),
                Globals = new List<VariableType>(),
                Parameters = new List<VariableType>()
            };

            while ((Tok == Token.VAR) || (Tok == Token.ARRAY))
            {
                cfg.Globals.AddRange(VarDecl(varTble));
            }

            foreach (VariableType global in cfg.Globals)
            {
                global.IsGlobal = true;
            }

            while ((Tok == Token.FUNCTION) || (Tok == Token.PROCEDURE))
            {
                Cfg func = FuncDecl(new VarTbl(varTble), cfg.Globals);
                func.Globals = cfg.Globals;
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

            var op = new Operand(Operand.OpType.Identifier, id)
            {
                Name = Scanner.SymbolTble.Symbols[id]
            };
            return op;
        }

        /*
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
        */

        private Operand Num()
        {
            GetExpected(Token.NUMBER);

            //return new Instruction(IrOps.load, new Operand(Operand.OpType.Constant, Scanner.Val), null);
            return new Operand(Operand.OpType.Constant, Scanner.Val);
        }


        public List<VariableType> VarDecl(VarTbl varTble)
        {
            VariableType varType = TypeDecl();
            var variableList = new List<VariableType>();

            CreateIdentifier(varTble, varType, variableList);

            while (Tok == Token.COMMA)
            {
                Next();

                //CreateIdentifier();
                CreateIdentifier(varTble, varType, variableList);
            }

            GetExpected(Token.SEMI_COLON);

            return variableList;
        }

        private void CreateIdentifier(VarTbl varTble, VariableType varType, List<VariableType> variableList)
        {
            Operand id = Identifier();
            string name = Scanner.SymbolTble.Symbols[id.IdKey];

            VariableType temp = varType.Clone();
            temp.Name = name;
            temp.Id = id.IdKey;
            temp.Offset = VariableType.CurrOffset;
            VariableType.CurrOffset += varType.Size;
            variableList.Add(temp);
            var ssa = new SsaVariable(id.IdKey, null, null, name, temp);
            varTble.Add(id.IdKey, ssa);
        }

        private VariableType TypeDecl()
        {
            VariableType newVar = null;

            if (Tok == Token.VAR)
            {
                Next();
                newVar = new VariableType();
                newVar.Size = VariableType.Dword;
            }
            else if (Tok == Token.ARRAY)
            {
                Next();
                var dims = new List<int>();
                while (Tok == Token.OPEN_BRACKET)
                {
                    Next();

                    int size = Num().Val;
                    if (size < 0)
                    {
                        throw new ParserException("Array Dimensions must be greater than zero");
                    }
                    dims.Add(size);

                    GetExpected(Token.CLOSE_BRACKET);
                }

                newVar = new ArrayType(dims);
            }
            else
            {
                // TODO: replace
                FatalError();
            }
            return newVar;
        }

        private Cfg FuncDecl(VarTbl variables, List<VariableType> globals)
        {
            var cfg = new Cfg {Parameters = new List<VariableType>()};
            cfg.Globals = globals;
            cfg.UsedGlobals = new HashSet<VariableType>();

            FunctionsCfgs.Add(cfg);

            var isProcedure = false;

            if ((Tok != Token.FUNCTION) && (Tok != Token.PROCEDURE))
            {
                FatalError();
            }
            else
            {
                isProcedure = Tok == Token.PROCEDURE;

            }

            cfg.isProcedure = isProcedure;

            Next();

            //TODO: Need a special address thing for functions
            //CreateIdentifier();
            Operand id = Identifier();

            var loads = new List<Instruction>();


            var prologue = new Node(new BasicBlock("Prologue", nestingDepth));
            cfg.Root = prologue;


            foreach (VariableType global in cfg.Globals)
            {
                SsaVariable temp = variables[global.Id];
                Instruction prologInst;

                if (global.IsArray)
                {
                    prologInst = new Instruction(IrOps.Ssa, new Operand(Operand.OpType.Constant, global.Id),
                        null);
                    prologInst.VArId = global;
                }
                else
                {
                    prologInst = new Instruction(IrOps.Load, new Operand(Operand.OpType.Identifier, global.Id),
                        null);
                    prologInst.VArId = global;
                    loads.Add(prologInst);
                }


                cfg.Root.Bb.AddInstruction(prologInst);
                temp.Value = new Operand(prologInst);

                var ssa = new SsaVariable(temp.UuId, prologInst, null, temp.Name) {Identity = global};
                temp.Value.Inst = prologInst;
                temp.Value.Variable = ssa;

                ssa.Value = new Operand(prologInst);

                prologInst.Arg2 = ssa.Value;


                variables[global.Id] = ssa;
                //arg = new Operand(ssa);
            }


            if (Tok == Token.OPEN_PAREN)
            {
                cfg.Parameters = FormalParams(variables);

                //*
                foreach (VariableType parameter in cfg.Parameters)
                {
                    SsaVariable temp = variables[parameter.Id];

                    var loadInst = new Instruction(IrOps.Load, new Operand(Operand.OpType.Identifier, parameter.Id),
                        null);
                    loadInst.VArId = parameter;
                    cfg.Root.Bb.AddInstruction(loadInst);
                    temp.Value = new Operand(loadInst);

                    var ssa = new SsaVariable(temp.UuId, loadInst, null, temp.Name) {Identity = parameter};
                    temp.Value.Inst = loadInst;
                    temp.Value.Variable = ssa;

                    ssa.Value = new Operand(loadInst);

                    //loadInst.Arg1 = ssa.Value;

                    variables[parameter.Id] = ssa;
                    //arg = new Operand(ssa);
                }

                //*/
            }

            GetExpected(Token.SEMI_COLON);

            Callgraph = new HashSet<string>();

            Cfg fb = FuncBody(variables, isProcedure);

            if (fb != null)
            {
                fb.Name = Scanner.SymbolTble.Symbols[id.IdKey];
                cfg.Insert(fb);
                cfg.Name = fb.Name;
                cfg.Locals = fb.Locals;
            }

            GetExpected(Token.SEMI_COLON);

            Instruction ret = cfg.Root.Leaf().Bb.Instructions.Last();
            cfg.Root.Leaf().Bb.Instructions.Remove(ret);
            var epilogue = new Node(new BasicBlock("Epilogue", nestingDepth));

            foreach (Instruction globalLoad in loads)
            {
                SsaVariable temp = variables[globalLoad.Arg1.IdKey];
                if (temp.Location != globalLoad)
                {
                    var newInst = new Instruction(IrOps.Store, temp.Value,
                        new Operand(Operand.OpType.Constant, temp.UuId));

                    epilogue.Bb.AddInstruction(newInst);
                    cfg.UsedGlobals.Add(temp.Identity);
                }
            }

            epilogue.Bb.AddInstruction(ret);

            cfg.Insert(epilogue);
            cfg.Callgraph = Callgraph;

            return cfg;
        }

        private Cfg FuncBody(VarTbl ssaTable, bool isProcedure)
        {
            var cfg = new Cfg {Locals = new List<VariableType>()};
            while ((Tok == Token.VAR) || (Tok == Token.ARRAY))
            {
                cfg.Locals.AddRange(VarDecl(ssaTable));
            }

            GetExpected(Token.OPEN_CURL);

            if ((Tok == Token.LET) || (Tok == Token.CALL) || (Tok == Token.IF)
                || (Tok == Token.WHILE) || (Tok == Token.RETURN))
            {
                cfg.Insert(StatementSequence(ref ssaTable));
            }

            GetExpected(Token.CLOSE_CURL);
            if (isProcedure)
            {
                var branchBack = new Instruction(IrOps.Ret, new Operand(Operand.OpType.Register, 31), null);
                cfg.Root.Leaf().Bb.AddInstruction(branchBack);
            }

            if (cfg.Root.Leaf().Bb.Instructions.Last().Op != IrOps.Ret)
            {
                FatalError("Functions must have a return statement");
            }

            return cfg;
        }


        private Cfg Statement(ref VarTbl variables)
        {
            var cfgTemp = new Cfg {Root = new Node(new BasicBlock("StatementBlock", nestingDepth))};

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
            var bb = new BasicBlock("StatSequence", nestingDepth);
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

        /*
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
        */


        private ParseResult FuncCall(VarTbl variables)
        {
            GetExpected(Token.CALL);

            Operand id = Identifier();
            var instructions = new List<Instruction>();

            var paramList = new List<ParseResult>();
            var localsList = new List<VariableType>();

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
            }

            foreach (Cfg func in FunctionsCfgs)
            {
                if (func.Name == id.Name)
                {
                    if (func.Parameters.Count != paramList.Count)
                    {
                        FatalError("Function '" + func.Name + "' takes " + func.Parameters.Count + " parameters, but " +
                                   paramList.Count + " were provided.");
                    }

                    localsList = func.Locals;
                }
            }

            foreach (ParseResult item in paramList)
            {
                instructions.AddRange(item.Instructions);
            }

            Instruction call;
            if (id.Name == "InputNum")
            {
                call = new Instruction(IrOps.Read, id, null);
            }
            else if (id.Name == "OutputNum")
            {
                call = new Instruction(IrOps.Write, paramList.First().Operand, id);
            }
            else if (id.Name == "OutputNewLine")
            {
                call = new Instruction(IrOps.WriteNl, id, null);
            }
            else
            {
                call = new Instruction(IrOps.Call, id, null);
                Callgraph.Add(id.Name);
            }

            id = new Operand(call);

            foreach (ParseResult result in paramList)
            {
                call.Parameters.Add(result.Operand);
                if (result.Operand.Kind == Operand.OpType.Instruction)
                {
                    result.Operand.Inst.Uses.Add(id);
                    result.Operand.Inst.UsesLocations.Add(call);
                }
                else if (result.Operand.Kind == Operand.OpType.Variable)
                {
                    result.Operand.Variable.Location.Uses.Add(id);
                    result.Operand.Variable.Location.UsesLocations.Add(call);
                }
            }

            call.Locals = localsList;
            instructions.Add(call);
            return new ParseResult(id, instructions, variables);
        }

        private Cfg IfStmt(VarTbl variables)
        {
            GetExpected(Token.IF);
            var ifBlock = new Cfg();
            var compBlock = new CompareNode(new BasicBlock("CompareBlock", nestingDepth));

            var joinBlock = new JoinNode(new BasicBlock("JoinBlock", nestingDepth));
            Node falseBlock = joinBlock;
            compBlock.Join = joinBlock;

            var trueSsa = new VarTbl(variables);
            var falseSsa = new VarTbl(variables);

            compBlock.Bb.AddInstructionList(Relation(variables).Instructions);

            GetExpected(Token.THEN);

            ifBlock.Insert(compBlock);

            // pass in a copy of variables so the original stays pristine
            Node trueBlock = StatementSequence(ref trueSsa).Root;
            trueBlock.Consolidate();

            compBlock.InsertTrue(trueBlock);
            trueBlock.Leaf().InsertJoinTrue(joinBlock);
            if (Tok == Token.ELSE)
            {
                Next();
                falseBlock = StatementSequence(ref falseSsa).Root;
                Node.Leaf(falseBlock).InsertJoinFalse(joinBlock);
                falseBlock.Consolidate();
            }


            //compBlock.InsertFalse(falseBlock);
            compBlock.FalseNode = falseBlock;
            if (falseBlock == joinBlock)
            {
                joinBlock.FalseParent = compBlock;
            }
            else
            {
                falseBlock.Parent = compBlock;
            }


            GetExpected(Token.FI);
            try
            {
                AddPhiInstructions(variables, trueSsa, falseSsa, joinBlock, false);
            }
            catch (ArgumentNullException)
            {
                throw ParserException.CreateParserException("Variable not initialized on all paths", LineNo, Pos,
                    _filename);
            }


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
                    var newInst = new Instruction(IrOps.Phi, new Operand(trueVar.Value.Location),
                        new Operand(falseVar.Location));




                    if ((newInst.Arg1.Inst == null) || (newInst.Arg2.Inst == null))
                    {
                        // throw new ArgumentNullException();
                    }

                    newInst.VArId = variables[trueVar.Value.UuId].Identity;


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
            var loopHeaderBlock = new WhileNode(new BasicBlock("LoopHeader", nestingDepth));

            // insert compare block for while stmt
            whileBlock.Insert(loopHeaderBlock);

            // add the relation/branch comparison into the loop header block
            loopHeaderBlock.Bb.AddInstructionList(Relation(headerSsa).Instructions);

            GetExpected(Token.DO);

            nestingDepth++;
            // prepare basic block for loop body
            Cfg stmts = StatementSequence(ref loopSsa);
            nestingDepth--;

            Node loopBlock = stmts.Root;

            Node last = loopBlock.Leaf();


            // insert the loop body on the true path
            loopHeaderBlock.InsertTrue(loopBlock);


            GetExpected(Token.OD);

            var followBlock = new Node(new BasicBlock("FollowBlock", nestingDepth)) {Colorname = "palegreen"};
            var branchBlock = new Node(new BasicBlock("BranchBack", nestingDepth));
            loopHeaderBlock.LoopParent = branchBlock;

            loopHeaderBlock.InsertFalse(followBlock);

            last.Child = branchBlock;
            branchBlock.Parent = last;
            branchBlock.Bb.AddInstruction(new Instruction(IrOps.Bra, new Operand(loopHeaderBlock.GetNextNonPhi()),
                null));
            branchBlock.Child = loopHeaderBlock;
            //loopBlock.Consolidate();


            try
            {
                AddPhiInstructions(variables, loopSsa, headerSsa, loopHeaderBlock, true);
            }
            catch (ArgumentNullException)
            {
                throw ParserException.CreateParserException("Variable not initialized on all paths", LineNo, Pos,
                    _filename);
            }

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
            if ((n == null) || visited.Contains(n))
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
                        phi.Uses.Add(inst.Arg1);
                        phi.UsesLocations.Add(inst);
                        inst.Arg1 = new Operand(phi);
                    }

                    if (CheckOperand(inst.Arg2, phi.Arg1) || CheckOperand(inst.Arg2, phi.Arg2))
                    {
                        if (inst.Op != IrOps.Ssa)
                        {
                            phi.Uses.Add(inst.Arg2);
                            phi.UsesLocations.Add(inst);
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
            var branchBack = new Instruction(IrOps.Ret, new Operand(Operand.OpType.Register, 31), retStmt?.Operand);
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

        private List<VariableType> FormalParams(VarTbl varTble)
        {
            GetExpected(Token.OPEN_PAREN);

            var paramList = new List<VariableType>();

            if (Tok == Token.IDENTIFIER)
            {
                CreateParameter(paramList, varTble);

                while (Tok == Token.COMMA)
                {
                    Next();
                    CreateParameter(paramList, varTble);
                }
            }

            GetExpected(Token.CLOSE_PAREN);

            return paramList;
        }

        private void CreateParameter(List<VariableType> paramList, VarTbl varTble)
        {
            Operand id = Identifier();
            string name = Scanner.SymbolTble.Symbols[id.IdKey];
            var newVar = new VariableType(name, id.IdKey);
            paramList.Add(newVar);

            var ssa = new SsaVariable(id.IdKey, null, null, name, newVar);
            if (varTble.ContainsKey(id.IdKey))
            {
                FatalError("Naming conflict with Global Variable:" + id.Name);
            }
            varTble.Add(id.IdKey, ssa);
        }


        public void Parse()
        {
            Next();
            var variables = new VarTbl();
            ProgramCfg = Computation(variables);

            // invoke before addin main to list of functions
            FixCallGraphs(variables);

            FunctionsCfgs.Insert(0, ProgramCfg);
            ProgramCfg.Name = "Main";
            ProgramCfg.Sym = Scanner.SymbolTble;
            ProgramCfg.UsedGlobals = new HashSet<VariableType>();
        }


        public void FixCallGraphs(VarTbl variables)
        {
            foreach (Cfg function in FunctionsCfgs)
            {
                var visitedhHashSet = new HashSet<string>();
                function.UsedGlobals.UnionWith(CheckCalls(visitedhHashSet, function));
                Node epilogue = function.Root.Leaf();
                var ret = epilogue.Bb.Instructions.Last();
                epilogue.Bb.Instructions.Remove(ret);

                foreach (VariableType global in function.UsedGlobals)
                {
                    SsaVariable temp = variables[global.Id];
                    var newInst = new Instruction(IrOps.Store, temp.Value,
                        new Operand(Operand.OpType.Constant, global.Id));
                    epilogue.Bb.AddInstruction(newInst);
                }

                epilogue.Bb.Instructions.Add(ret);
            }
        }

        public HashSet<VariableType> CheckCalls(HashSet<string> visited, Cfg func)
        {
            var newGlobals = new HashSet<VariableType>(func.UsedGlobals);
            if (!visited.Contains(func.Name))
            {
                visited.Add(func.Name);
                foreach (string name in func.Callgraph)
                {
                    Cfg found = FunctionsCfgs.Find(current => current.Name == name);
                    newGlobals.UnionWith(CheckCalls(visited, found));
                }
            }

            return newGlobals;
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