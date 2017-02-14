using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler;
using compiler.middleend.ir;

namespace NUnit.Tests.Middle_End
{
    [TestFixture]
    public class NodeTests
    {
        public Node Root { get; set; }

        [SetUp]
        public void Init()
        {
            Root = new Node(new BasicBlock("Test Block"));
        }

        [Test]
        public void IsRootTest()
        {
            Assert.IsTrue(Root.IsRoot());

            Root.Insert(new Node(new BasicBlock("Child Block")));

            Assert.IsFalse(Root.Child.IsRoot());

        }

        [Test]
        public void InsertionTest()
        {
            Assert.IsTrue(Root.IsRoot());

            Root.Insert(new Node(new BasicBlock("Child Block")));

            Assert.IsFalse(Root.Child.IsRoot());

            Root.Insert(new Node(new BasicBlock("Child Block 2")));
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
        public void LeafReturnsChildTest()
        {
            Root.Insert(new Node(new BasicBlock("Child Block")));
            Root.Insert(new Node(new BasicBlock("Child Block 2")));
            Assert.AreNotEqual(Root, Node.Leaf(Root));
            Assert.AreNotEqual(Root.Child, Node.Leaf(Root));
        }



        [Test]
        public void ConsolidateTest()
        {
            var inst1 = new Instruction(IrOps.add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            Root.BB.Instructions.Add(inst1);

            Root.Insert(new Node(new BasicBlock("Child Block")));
            Root.Child.BB.Instructions.Add(inst2);
            Root.Insert(new Node(new BasicBlock("Child Block 2")));

           Root.Consolidate();
            Assert.AreEqual(Root, Node.Leaf(Root));
            Assert.AreEqual(2, Root.BB.Instructions.Count);
        }

        [Test]
        public void ConsolidateCircularExceptionTest()
        {
            Root.Child = Root;
            var ex = Assert.Throws<Exception>(() => Root.Consolidate());
            Assert.That(ex.Message, Is.EqualTo("Circular reference in basic block!!"));
        }


        [Test]
        public void GetNextInstructionTest()
        {
            var inst1 = new Instruction(IrOps.add, new Operand(Operand.OpType.Constant, 10),
               new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            Root.BB.Instructions.Add(inst1);

            Root.Insert(new Node(new BasicBlock("Child Block")));
            Root.Child.BB.Instructions.Add(inst2);
            Root.Insert(new Node(new BasicBlock("Child Block 2")));

            Assert.AreEqual(Root.BB.Instructions.First(), Root.GetNextInstruction());

        }



        [Test]
        public void GetLastInstructionTest()
        {
            Assert.Null(Root.GetLastInstruction());

            var inst1 = new Instruction(IrOps.add, new Operand(Operand.OpType.Constant, 10),
               new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            Root.BB.Instructions.Add(inst1);

            Root.Insert(new Node(new BasicBlock("Child Block")));
            Root.Child.BB.Instructions.Add(inst2);
            

            Assert.AreEqual(Root.Child.BB.Instructions.Last(), Root.GetLastInstruction());
            Root.Insert(new Node(new BasicBlock("Child Block 2")));

            Root.Leaf().BB.Instructions.Add(inst2);
            Root.Leaf().BB.Instructions.Add(inst1);

            Assert.AreEqual(inst1, Root.GetLastInstruction());

        }



    }
}
