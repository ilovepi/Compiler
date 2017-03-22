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



#endregion

namespace compiler.middleend.ir
{
    public class VariableType
    {
        public const int Dword = 4;
        public static int CurrOffset;

        public readonly bool IsArray;
        public bool IsGlobal;

        public string Name { get; set; }

        public int Id { get; set; }

        public int Size { get; set; }

        public int Offset { get; set; }

        public int Address { get; set; }

        public VariableType()
        {
        }

        /*
        // constructor for normal variables
        public VariableType(int pId, int pOffset)
        {
            Name = String.Empty;
            Id = pId;
            Offset = pOffset;
            IsArray = false;
            Size = 1;
            Address = 0;
        }


        // constructor for normal variables
        public VariableType(string pName, int pId, int pOffset)
        {
            Name = pName;
            Id = pId;
            Offset = pOffset;
            IsArray = false;
            Size = 1;
            Address = 0;
        }
        */

        // constructor for normal variables
        public VariableType(string pName, int pId)
        {
            Name = pName;
            Id = pId;
            IsArray = false;
            Offset = CurrOffset;
            Size = VariableType.Dword;
            CurrOffset += Size;
            Address = 0;
        }

        public VariableType(string pName, int pId, int pOffset, int size, bool pIsArray)
        {
            Name = pName;
            Id = pId;
            Offset = pOffset;
            IsArray = pIsArray;
            Size = size;
            Address = 0;
        }

        public virtual VariableType Clone()
        {
            return new VariableType(Name, Id, Offset, Size, IsArray);
        }

        /*
        public void Allocate(int baseAddr)
        {
            Address = baseAddr + (Offset * Dword);
        }
        */
    }
}