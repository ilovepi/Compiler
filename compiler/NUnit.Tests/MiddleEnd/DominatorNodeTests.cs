using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.middleend.ir;

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class DominatorNodeTests
    {
        private DominatorNode d;
        [SetUp]
        public void Init()
        {
            d  = new DominatorNode(new BasicBlock("poo"));

        }

        [Test]
        public void TestEquals()
        {
           
            Assert.True(d.Equals(d));
            Assert.False(d.Equals(null));
            var e = new DominatorNode(d.Bb);
            Assert.True(d.Equals(e));
            d.Children.Add(e);
            Assert.False(d.Equals(e));

            d.RemoveChild(e);
            e.Bb = null;
            Assert.False(d.Equals(e));
            d.InsertChild(e);
            e.Bb = new BasicBlock("This");
            Assert.False(d.Equals(e));

            d = new DominatorNode(new BasicBlock("poo"));
            e = new DominatorNode(d.Bb);
            d.InsertChild(new DominatorNode(new BasicBlock("that")));
            e.Children.Add(d.Children.First());
            Assert.True(d.Equals(e));

            e.Children.Clear();
            e.InsertChild(new DominatorNode(new BasicBlock("the other thing")));
            Assert.False(d.Equals(e));

        }



        [Test]
        public void TestIsRoot()
        {
            Assert.True(d.IsRoot());
            var e = new DominatorNode(new BasicBlock("poo"));
            e.InsertChild(d);
            Assert.False(d.IsRoot());
            
        }
    }
}
