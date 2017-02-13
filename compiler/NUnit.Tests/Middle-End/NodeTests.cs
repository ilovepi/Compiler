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
        public Node root { get; set; }

        [SetUp]
        public void Init()
        {
            root = new Node(new BasicBlock("Test Block"));
        }

        [Test]
        public void IsRootTest()
        {
            Assert.IsTrue(root.IsRoot());

            root.Insert(new Node(new BasicBlock("Child Block")));

            Assert.IsFalse(root.Child.IsRoot());

        }

        [Test]
        public void InsertionTest()
        {
            Assert.IsTrue(root.IsRoot());

            root.Insert(new Node(new BasicBlock("Child Block")));

            Assert.IsFalse(root.Child.IsRoot());

            root.Insert(new Node(new BasicBlock("Child Block 2")));
        }


        [Test]
        public void LeafReturnsNullTest()
        {
            root = null;
            Assert.Null(Node.Leaf(root));
        }


        [Test]
        public void LeafReturnsRootTest()
        {
            Assert.AreEqual(root, Node.Leaf(root));
        }


        [Test]
        public void LeafReturnsChildTest()
        {
            root.Insert(new Node(new BasicBlock("Child Block")));
            root.Insert(new Node(new BasicBlock("Child Block 2")));
            Assert.AreNotEqual(root, Node.Leaf(root));
            Assert.AreNotEqual(root.Child, Node.Leaf(root));
        }



    }
}
