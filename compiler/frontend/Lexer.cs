using System;
using System.IO;

namespace compiler.frontend
{
	class Lexer
	{
		StreamReader sr;// file reader
		public char c; // current char
		SymbolTable symbolTble; // symbol table
        int sym; // current token
        int val; // numeric value
        int id; // identifier

		public Lexer(string filename)
		{
			try
			{
				sr = new StreamReader(filename);
			}
			catch (FileNotFoundException e)
			{
				Console.WriteLine(e.Message);
				throw e;
			}

			symbolTble = new SymbolTable();
		}

		~Lexer()
		{
			if (sr != null)
			{
				sr.Close();
				sr = null;
			}
		}

		public char next()
		{
			if (sr.Peek() == -1)
			{
				throw new Exception("Error: Lexer cannot read beyond the end of the file");
			}
			c = (char)sr.Read();
			return c;
		}


		public int getNextToken()
		{
			findNextToken();

			if (char.IsDigit(c))
			{
				return number();
			}
			else if (char.IsLetter(c))
			{
				return symbol();
			}
            else if(char.IsSymbol(c))
            {
                return punctuation();
            }

			throw new Exception("Error: unable to parse next token");

		}

		public int number()
		{
			
			string s = string.Empty;

			while (char.IsDigit(c))
			{
				s += c;
				next();
			}

			
			val = int.Parse(s);
			return 0xffff;
		}

		public int punctuation()
		{
			Result ret = new Result();
			if (c == '=')
			{
				next();
				if (c != '=')
				{
					throw new Exception("Error: '=' is not a valid token, " +
										"must be one of: '==', '>=', '<=', '!='");
				}

				ret.id = symbolTble.val("==");

			}
			else if (c == '!')
			{
				next();
				if (c != '=')
				{
					throw new Exception("Error: '!' is not a valid token, " +
										"must be one of: '==', '>=', '<=', '!='");
				}

				ret.id = symbolTble.val("!=");
			}
			else if (c == '<')
			{
				next();
				if (c != '=')
				{
					ret.id = symbolTble.val("<");
				}
				else if (c == '-')
				{
					ret.id = symbolTble.val("<-");
				}
				else
				{
					ret.id = symbolTble.val("<=");
				}
			}
			else if (c == '<')
			{
				next();
				if (c != '=')
				{
					ret.id = symbolTble.val("<");
				}
				else
				{
					ret.id = symbolTble.val("<=");
				}
			}

			return ret.id;
		}

		public int symbol()
		{
            Result ret = new Result();

			string s = string.Empty;
			s += c;
			next();

			while (char.IsLetterOrDigit(c))
			{
				s += c;
				next();
			}

            ret.kind = (int)kind.variable;

			if (symbolTble.lookup(s))
			{
				return symbolTble.val(s);
			}
			else
			{
				symbolTble.insert(s);
				ret.id = symbolTble.val(s);
			}

			return ret.id;
		}

		/// <summary>
		/// Finds the next token. Scans forward through whitespace.
		/// </summary>
		public void findNextToken()
		{
			while (char.IsWhiteSpace(c))
			{
				next();
			}
		}


	}
}
