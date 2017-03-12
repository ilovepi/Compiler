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

using compiler.middleend.ir;
using compiler.middleend.optimization;
using NUnit.Framework;

#endregion

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class BasicBlockTests
    {
        [SetUp]
        public void Init()
        {
            Block = new BasicBlock();
        }

        public BasicBlock Block { get; set; }

        [Test]
        public void ConstructorTest()
        {
            Assert.Null(Block.Name);
            Block = new BasicBlock("Test Block");
            Assert.AreEqual("Test Block", Block.Name);
        }


        [Test]
        public void SearchTest()
        {
            var graphCfg = new Cfg {Root = new Node(new BasicBlock("MyBlock"))};

            var inst1 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            graphCfg.Root.Bb.AddInstruction(inst1);
            graphCfg.Root.Bb.AddInstruction(inst1);
            graphCfg.Root.Bb.AddInstruction(inst2);


            graphCfg.Root.Insert(new Node(new BasicBlock("Child Block")));
            graphCfg.Root.Child.Bb.AddInstruction(inst2);
            CsElimination.Eliminate(graphCfg.Root);


            //graphCfg.Root.Insert(new Node(new BasicBlock("Child Block 2")));
            //graphCfg.Root.Insert(new Node(new BasicBlock("Child Block 2")));

            Assert.NotNull(graphCfg.Root.Leaf().AnchorSearch(inst1));
        }
    }
}