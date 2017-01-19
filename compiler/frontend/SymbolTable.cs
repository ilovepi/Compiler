using System;

using System.Collections.Generic;

using compiler.frontend;


namespace compiler
{
	public class SymbolTable
	{
		Dictionary<string, int> values;
	
		Dictionary<int, string> symbols;

		int count;

		public SymbolTable()
		{
			count = 0;
			values = new Dictionary<string, int>();
			symbols = new Dictionary<int, string>();
			init();
		}

		public void insert(string key)
		{
            //TODO: Add test to cover the exception
			if (values.ContainsKey(key))
			{
				throw new Exception("Error: cannot insert duplicate symbols");
			}
			
			values.Add(key, count);
			symbols.Add(count, key);
            count++;

        }

		public string symbol(int key)
		{
            //TODO: no test covers this function
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
            //TODO: add test to verify that the enum and this dict have identical values
            insert(".unknown"); //0 safe since '.' is EOF char, nothing should ever come after it

			insert("+");    //01
			insert("-");
			insert("*");
			insert("/");    //04

			insert("==");   //05
            insert("!=");
            insert("<");
            insert("<=");
            insert(">");
            insert(">=");   //10

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
            insert("if");   
            insert("then");
            insert("else");
            insert("fi");
            insert("while");
            insert("do");
            insert("od");   //28
            
			insert("return");//29
			insert("main");
            insert("var");
            insert("array"); //32

            insert("function");//33
			insert("procedure");
			insert(".");
            insert("//");
            // since '.' is the EOF symbol, .number and .identifier should be safe
            insert(".number"); 
            insert(".identifier"); //38           
		}

        
		public bool isId(string s)
		{
            //TODO: add unit test
			// if the entry is an ID, it must come after keywords
			return ( val(".identifier") < val(s) );
		}


	}
}
