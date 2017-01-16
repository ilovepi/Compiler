using System;
using System.IO;

namespace compiler.frontend
{
	class Lexer
	{
		StreamReader sr;
		public char c;
		SymbolTable sym;

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

			sym = new SymbolTable();
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


		public Result getNextToken()
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

			throw new Exception("Error: unable to parse next token");

		}

		public Result number()
		{
			Result ret = new Result();
			string s = string.Empty;

			while (char.IsDigit(c))
			{
				s += c;
				next();
			}

			ret.kind = (int)kind.constant;
			ret.value = int.Parse(s);
			return ret;
		}


		public Result symbol()
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

			if (sym.lookup(s))
			{
				ret.id = sym.val(s);
			}
			else
			{
				sym.insert(s);
				ret.id = sym.val(s);
			}

			return ret;
		}

		public void findNextToken()
		{
			while (char.IsWhiteSpace(c))
			{
				next();
			}
		}



	}
}