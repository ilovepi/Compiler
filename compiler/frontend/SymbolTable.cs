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

using System.Collections.Generic;
using System.Data;

#endregion

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
            Insert(".unknown"); //0 safe since '.' is EOF char, nothing should ever come after it

            Insert("+"); //01
            Insert("-");
            Insert("*");
            Insert("/"); //04

            Insert("=="); //05
            Insert("!=");
            Insert("<");
            Insert("<=");
            Insert(">");
            Insert(">="); //10

            Insert("<-"); //11
            Insert(";");
            Insert(","); //13

            Insert("("); //14
            Insert(")");
            Insert("[");
            Insert("]");
            Insert("{");
            Insert("}"); //19


            Insert("let"); //20
            Insert("call");
            Insert("if");
            Insert("then");
            Insert("else");
            Insert("fi");
            Insert("while");
            Insert("do");
            Insert("od"); //28

            Insert("return"); //29
            Insert("main");
            Insert("var");
            Insert("array"); //32

            Insert("function"); //33
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
            return Values[".identifier"] < Values[s];
        }
    }
}