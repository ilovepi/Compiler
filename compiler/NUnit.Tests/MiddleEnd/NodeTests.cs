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

using System;
using System.Linq;
using compiler.middleend.ir;
using NUnit.Framework;

#endregion

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class NodeTests
    {
        private int nestingDepth = 1;
        [SetUp]
        public void Init()
        {
            Root = new Node(new BasicBlock("Test Block", nestingDepth));
        }

        public Node Root { get; set; }

        [Test]
        public void CheckEnqueTest()
        {
        }

        [Test]
        public void ConsolidateCircularExceptionTest()
        {
            Root.Child = Root;
            var ex = Assert.Throws<Exception>(() => Root.Consolidate());
            Assert.That(ex.Message, Is.EqualTo("Circular reference in basic block!!"));
        }


        [Test]
        public void ConsolidateTest()
        {
            var inst1 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            Root.Bb.Instructions.Add(inst1);

            Root.Insert(new Node(new BasicBlock("Child Block", nestingDepth)));
            Root.Child.Bb.Instructions.Add(inst2);
            Root.Insert(new Node(new BasicBlock("Child Block 2", nestingDepth)));

            Root.Consolidate();
            Assert.AreEqual(Root, Node.Leaf(Root));
            Assert.AreEqual(2, Root.Bb.Instructions.Count);
        }


        [Test]
        public void GetLastInstructionTest()
        {
            Assert.Null(Root.GetLastInstruction());

            var inst1 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            Root.Bb.Instructions.Add(inst1);

            Root.Insert(new Node(new BasicBlock("Child Block", nestingDepth)));
            Root.Child.Bb.Instructions.Add(inst2);


            Assert.AreEqual(Root.Child.Bb.Instructions.Last(), Root.GetLastInstruction());
            Root.Insert(new Node(new BasicBlock("Child Block 2", nestingDepth)));

            Root.Leaf().Bb.Instructions.Add(inst2);
            Root.Leaf().Bb.Instructions.Add(inst1);

            Assert.AreEqual(inst1, Root.GetLastInstruction());

            Root.Insert(new Node(new BasicBlock("Almost Last Block", nestingDepth)));
            Root.Insert(new Node(new BasicBlock("lastBlock", nestingDepth)));

            Assert.AreEqual(inst1, Root.GetLastInstruction());
        }


        [Test]
        public void GetNextInstructionTest()
        {
            var inst1 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            Root.Bb.Instructions.Add(inst1);

            Root.Insert(new Node(new BasicBlock("Child Block", nestingDepth)));
            Root.Child.Bb.Instructions.Add(inst2);
            Root.Insert(new Node(new BasicBlock("Child Block 2", nestingDepth)));

            Assert.AreEqual(Root.Bb.Instructions.First(), Root.GetNextInstruction());
        }

        [Test]
        public void InsertionTest()
        {
            Assert.IsTrue(Root.IsRoot());

            Root.Insert(new Node(new BasicBlock("Child Block", nestingDepth)));

            Assert.IsFalse(Root.Child.IsRoot());

            Root.Insert(new Node(new BasicBlock("Child Block 2", nestingDepth)));
        }

        [Test]
        public void IsRootTest()
        {
            Assert.IsTrue(Root.IsRoot());

            Root.Insert(new Node(new BasicBlock("Child Block", nestingDepth)));

            Assert.IsFalse(Root.Child.IsRoot());
        }


        [Test]
        public void LeafReturnsChildTest()
        {
            Root.Insert(new Node(new BasicBlock("Child Block", nestingDepth)));
            Root.Insert(new Node(new BasicBlock("Child Block 2", nestingDepth)));
            Assert.AreNotEqual(Root, Node.Leaf(Root));
            Assert.AreNotEqual(Root.Child, Node.Leaf(Root));
        }


        [Test]
        public void LeafReturnsNullTest()
        {
            Root = null;
            Assert.Null(Node.Leaf(Root));
        }


        [Test]
        public void LeafReturnsRootTest()
        {
            Assert.AreEqual(Root, Node.Leaf(Root));
        }


        [Test]
        public void NodeTeypeTest()
        {
            var inst1 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            Root.Bb.Instructions.Add(inst1);

            Root.Insert(new Node(new BasicBlock("Child Block", nestingDepth)));
            Root.Child.Bb.Instructions.Add(inst2);

            Node.NodeTypes temp = Root.NodeType;
            Assert.AreEqual(Node.NodeTypes.BB, temp);
        }


        [Test]
        public void WhileNodeTest()
        {
            var n = new WhileNode(new BasicBlock("BB", nestingDepth));
            Assert.AreEqual(n, n.Leaf());
            Instruction i = n.GetLastInstruction();
            Assert.Null(i);
            n.Insert(new WhileNode(new BasicBlock("BB", nestingDepth)));
            i = n.GetLastInstruction();
            Assert.Null(i);
            n.Bb.AddInstruction(new Instruction(IrOps.Add, null, null));
            i = n.GetLastInstruction();
            Assert.NotNull(i);
            n.Consolidate();
            n.FalseNode = null;
            i = n.GetLastInstruction();
            Assert.NotNull(i);
            n.Consolidate();
        }
    }
}