using System.Collections.Generic;
using System.Data;

namespace compiler.frontend
{
	public class SymbolTable
	{
	    public Dictionary<string, int> Values { get; set; }

	    public Dictionary<int, string> Symbols { get; set; }

        public Dictionary<int, int> AddressTble { get; set; }

        public int Count { get; private set; }

	    public SymbolTable()
		{
			Count = 0;
			Values = new Dictionary<string, int>();
			Symbols = new Dictionary<int, string>();
            AddressTble = new Dictionary<int, int>();
			Init();
		}

		public void Insert(string key)
		{
            //TODO: Add test to cover the exception
			if (Values.ContainsKey(key))
			{
			    throw new DuplicateNameException("Error: Symbol Table cannot contain duplicate symbols: " + key);
			}
			
			Values.Add(key, Count);
			Symbols.Add(Count, key);
            Count++;

        }

        //TODO: We may only want/need the address version
	    public void Insert(string key, int address)
	    {
	        Insert(key);
            InsertAddress(key, address);
	    }


        //TODO: determine if this is necessary
	    public void InsertAddress(string key, int address)
	    {
            AddressTble.Add(Values[key], address);
	    }

        public void InsertAddress(int key, int address)
        {
            AddressTble.Add(key, address);
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
   
			// if the entry is an ID, it must come after keywords
			return ( Values[".identifier"] < Values[s]);
		}


	}
}
