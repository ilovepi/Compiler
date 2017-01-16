using System;

using System.Collections.Generic;
namespace compiler
{
	public class SymbolTable
	{
		Dictionary<string, int> values;
	
		Dictionary<int, string> symbols;

		int count;
		int keywords;

		public SymbolTable()
		{
			count = 0;
			values = new Dictionary<string, int>();
			symbols = new Dictionary<int, string>();
			init();
			keywords = count;
		}

		public void insert(string key)
		{
			if (values.ContainsKey(key))
			{
				throw new Exception("Error: cannot insert duplicate symbols");
			}
			count++;
			values.Add(key, count);
			symbols.Add(count, key);
			
		}

		public string symbol(int key)
		{
			return symbols[key];
		}

		public int val(string key)
		{
			return values[key];
		}

		public bool lookup(string key)
		{
			return values.ContainsKey(key);
		}


		private void init()
		{
			insert("+");    //01
			insert("-");
			insert("*");
			insert("/");    //04

			insert("==");   //05
			insert("<=");
			insert("!=");
			insert(">=");
			insert("<");
			insert(">");    //10

            insert("<-");   //11
			insert(";");
			insert(",");    //13

			insert("(");    //14
			insert(")");
			insert("[");
			insert("]");
			insert("{");
			insert("}");    //19


            insert("let");  //20
			insert("call");
			insert("var");  //21

			insert("if");   //22
			insert("then");
			insert("else"); //23

			insert("fi");   //24
			insert("while");
			insert("do");
			insert("od");   //27

			insert("return");//28
			insert("main");
			insert("function");
			insert("procedure");
			insert(".");    //32
		}

		public bool isId(string s)
		{
			// if the entry is an ID, it must come after keywords
			return ( keywords < val(s) );
		}


	}
}
