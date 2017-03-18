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

#endregion

namespace compiler.middleend.ir
{
    public class ArrayType : VariableType
    {
        public List<int> Dimensions { get; set; }

        public ArrayType() : base(string.Empty, 0, 0, 1, true)
        {
        }

        public ArrayType(List<int> dims) : base(string.Empty, 0, 0, 1, true)
        {
            Dimensions = new List<int>(dims);

            foreach (int dimension in Dimensions)
            {
                Size *= dimension;
            }
        }

        //constructor for arrays
        public ArrayType(int pId, int pOffset, List<int> dims) : base(string.Empty, pId, pOffset, 1, true)
        {
            Dimensions = new List<int>(dims);

            foreach (int dimension in Dimensions)
            {
                Size *= dimension;
            }
        }


        //constructor for arrays
        public ArrayType(string pName, int pId, int pOffset, List<int> dims) : base(pName, pId, pOffset, 1, true)
        {
            Dimensions = new List<int>(dims);

            foreach (int dimension in Dimensions)
            {
                Size *= dimension;
            }
        }

        public ArrayType(string pName, int pId, int pOffset, List<int> dims, bool pIsAray)
            : base(pName, pId, pOffset, 1, pIsAray)
        {
            Dimensions = new List<int>(dims);

            foreach (int dimension in Dimensions)
            {
                Size *= dimension;
            }
        }

        public override VariableType Clone()
        {
            return new ArrayType(Name, Id, Offset, Dimensions, IsArray);
        }
    }
}