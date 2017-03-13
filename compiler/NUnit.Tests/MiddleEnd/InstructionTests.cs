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
using compiler.middleend.ir;
using NUnit.Framework;

#endregion

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class InstructionTests
    {
        [SetUp]
        public void Init()
        {
            Inst1 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            Inst2 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 05),
                new Operand(Inst1));

            Inst3 = new Instruction(IrOps.Bra, new Operand(Inst1), null);
        }

        private Instruction Inst1 { get; set; }

        private Instruction Inst2 { get; set; }

        private Instruction Inst3 { get; set; }

        [Test]
        public void EqualsNotEqualTest()
        {
            object l = null;
            Assert.AreNotEqual(Inst2, Inst1);
            Assert.AreNotEqual(Inst2, null);
            Assert.AreNotEqual(Inst1, null);

            Assert.AreNotEqual(Inst1, new List());
            Assert.False(Inst1.Equals(null));
            Assert.False(Inst1.Equals(l));
            l = new List();
            Assert.AreNotEqual(Inst1, l);

            Assert.AreNotEqual(Inst1, Inst3);
        }


        [Test]
        public void EqualsTest()
        {
            Assert.AreEqual(Inst2, Inst2);

            object l = Inst1;
            Assert.True(Inst1.Equals(l));

            l = new Instruction(Inst1);
            Assert.True(Inst1.Equals(l));
        }


        [Test]
        public void HashcodeTest()
        {
            var dict = new Dictionary<Instruction, int>();
            dict[Inst1] = 1;
            dict[Inst2] = 2;
            Assert.AreNotEqual(Inst1.GetHashCode(), Inst2.GetHashCode());
            Assert.AreEqual(Inst1.GetHashCode(), Inst1.GetHashCode());
            Assert.AreNotEqual(dict[Inst1], dict[Inst2]);
        }


        [Test]
        public void ToStringTest()
        {
            Assert.AreEqual(Inst3.Num + ": Bra (" + Inst1.Num + ")  : 0", Inst3.ToString().Trim());
            Assert.AreEqual(Inst2.Num + ": Add #5 (" + Inst1.Num + ") : 0", Inst2.ToString());
        }
    }
}