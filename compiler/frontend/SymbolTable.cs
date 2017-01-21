using System;
using System.Collections.Generic;

namespace compiler.frontend
{
	public class SymbolTable
	{
	    public Dictionary<string, int> Values { get; set; }

	    public Dictionary<int, string> Symbols { get; set; }

	    public int Count { get; private set; }

	    public SymbolTable()
		{
			Count = 0;
			Values = new Dictionary<string, int>();
			Symbols = new Dictionary<int, string>();
			Init();
		}

		public void Insert(string key)
		{
            //TODO: Add test to cover the exception
			if (Values.ContainsKey(key))
			{
				throw new Exception("Error: cannot insert duplicate symbols");
			}
			
			Values.Add(key, Count);
			Symbols.Add(Count, key);
            Count++;

        }

		public string Symbol(int key)
		{
            //TODO: no test covers this function
			return Symbols[key];
		}

		public int Val(string key)
		{
			return Values[key];
		}

		public bool Lookup(string key)
		{
			return Values.ContainsKey(key);
		}


		private void Init()
		{
            //TODO: add test to verify that the enum and this dict have identical values
            Insert(".unknown"); //0 safe since '.' is EOF char, nothing should ever come after it

			Insert("+");    //01
			Insert("-");
			Insert("*");
			Insert("/");    //04

			Insert("==");   //05
            Insert("!=");
            Insert("<");
            Insert("<=");
            Insert(">");
            Insert(">=");   //10

            Insert("<-");   //11
			Insert(";");
			Insert(",");    //13

			Insert("(");    //14
			Insert(")");
			Insert("[");
			Insert("]");
			Insert("{");
			Insert("}");    //19


            Insert("let");  //20
			Insert("call");
            Insert("if");   
            Insert("then");
            Insert("else");
            Insert("fi");
            Insert("while");
            Insert("do");
            Insert("od");   //28
            
			Insert("return");//29
			Insert("main");
            Insert("var");
            Insert("array"); //32

            Insert("function");//33
			Insert("procedure");
			Insert(".");
            Insert("//");
            // since '.' is the EOF symbol, .number and .identifier should be safe
            Insert(".number"); 
            Insert(".identifier"); //38           
		}

        
		public bool IsId(string s)
		{
            //TODO: add unit test
			// if the entry is an ID, it must come after keywords
			return ( Val(".identifier") < Val(s) );
		}


	}
}
