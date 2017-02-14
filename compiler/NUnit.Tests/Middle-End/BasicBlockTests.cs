using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.middleend.ir;

namespace NUnit.Tests.Middle_End
{
    [TestFixture]
    public class BasicBlockTests
    {

        public BasicBlock Block { get; set; }

        [SetUp]
        public void Init()
        {
            Block = new BasicBlock();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.Null(Block.Name);
            Block = new BasicBlock("Test Block");
            Assert.AreEqual("Test Block", Block.Name);
        }

    }
}
