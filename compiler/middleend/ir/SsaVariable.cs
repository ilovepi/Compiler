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

namespace compiler.middleend.ir
{
    public class SsaVariable
    {
        public VariableType Identity { get; set; }

        public int UuId { get; set; }

        public Instruction Location { get; set; }

        /// <summary>
        ///     Previous Instruction
        /// </summary>
        public Instruction Prev { get; set; }

        public string Name { get; set; }

        public Operand Value { get; set; }

        public SsaVariable(SsaVariable other)
        {
            Name = other.Name;
            Prev = other.Prev;
            Location = new Instruction(other.Location);
            Value = other.Value;
            UuId = other.UuId;
            Identity = other.Identity;
        }

        public SsaVariable(int puuid, Instruction plocation, Instruction pPrev, string pName)
        {
            Name = pName;
            Prev = pPrev;
            Location = plocation;
            UuId = puuid;
        }

        public SsaVariable(int puuid, Instruction plocation, Instruction pPrev, string pName, VariableType variable)
        {
            Identity = variable;
            Name = pName;
            Prev = pPrev;
            Location = plocation;
            UuId = puuid;
        }

        public override string ToString()
        {
            return Name + Location?.Num + "=" + Value;
        }
    }
}