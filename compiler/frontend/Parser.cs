using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.frontend;

namespace compiler.frontend
{

    class Parser
    {
        public Token t;
        public Lexer s;
        string filename;

		public bool isRelOp()
		{
			switch (t)
			{
				case Token.EQUAL:
					return true;
				case Token.NOT_EQUAL:
					return true;
				case Token.LESS:
					return true;
				case Token.LESS_EQ:
					return true;
				case Token.GREATER:
					return true;
				case Token.GREATER_EQ:
					return true;
			}
			return false;
		}

        public void getExpected(Token expected)
        {
            if (t == expected)
            {
                next();
            }
            else {
				error();
            }
        }

        public void error(string str)
        {
            //TODO: determine location in file
            Console.WriteLine ("Error Parsing file: " + filename + ", " + str);
            error_fatal();
        }

        public void error_fatal()
		{
            //TODO: determine location in file
			throw new Exception("Fatal Error Parsing file: " + filename + ". Unable to continue");
        }


        public void next() {
            t = s.getNextToken();
        }

        public void Designator() 
		{
			getExpected(Token.IDENTIFIER);
			while (t == Token.OPEN_BRACKET)
			{
				next();
				Expression();
				getExpected(Token.CLOSE_BRACKET);
			}
        }

        public void Factor()
		{
			switch (t)
			{
				case Token.NUMBER:
					//TODO: Record number value
					next();
					break;
				case Token.IDENTIFIER:
					//TODO: Record identifier
					Designator();
					break;
				case Token.OPEN_PAREN:
					next();
					Expression();
					getExpected(Token.CLOSE_BRACKET)
					break;
				case Token.CALL:
					next();
					FuncCall();
					break;
				default:
					error();
			}
        }

        public void Term()
		{
			Factor();
			while ((t == Token.TIMES) || (t == Token.DIVIDE))
			{
				next();
				Factor();
			}
        }

        public void Expression()
		{
			Term();
			while ((t == Token.PLUS) || (t == Token.MINUS))
			{
				next();
				Term();
			}
        }

        public void Relation()
		{
			Expression();
			if (!isRelOp())
			{
				error();
			}
			next();
			Expression();
        }
			                    
		public void Assign()
		{

		}

		public void FunctionCall()
		{
			
		}

    }
}
