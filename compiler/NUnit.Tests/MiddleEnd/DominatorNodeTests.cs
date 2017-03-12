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
        private DominatorNode _d;
        [SetUp]
        public void Init()
        {
            _d  = new DominatorNode(new BasicBlock("poo"));

        }

        [Test]
        public void TestEquals()
        {
           
            Assert.NotNull(_d);
            Assert.True(_d.Equals(_d));
            Assert.False(_d.Equals(null));
            var e = new DominatorNode(_d.Bb);
            Assert.True(_d.Equals(e));
            _d.Children.Add(e);
            Assert.False(_d.Equals(e));

            _d.RemoveChild(e);
            e.Bb = null;
            Assert.False(_d.Equals(e));
            _d.InsertChild(e);
            e.Bb = new BasicBlock("This");
            Assert.False(_d.Equals(e));

            _d = new DominatorNode(new BasicBlock("poo"));
            e = new DominatorNode(_d.Bb);
            _d.InsertChild(new DominatorNode(new BasicBlock("that")));
            e.Children.Add(_d.Children.First());
            Assert.True(_d.Equals(e));

            e.Children.Clear();
            e.InsertChild(new DominatorNode(new BasicBlock("the other thing")));
            Assert.False(_d.Equals(e));

        }



        [Test]
        public void TestIsRoot()
        {
            Assert.True(_d.IsRoot());
            var e = new DominatorNode(new BasicBlock("poo"));
            e.InsertChild(_d);
            Assert.False(_d.IsRoot());
            
        }
    }
}
