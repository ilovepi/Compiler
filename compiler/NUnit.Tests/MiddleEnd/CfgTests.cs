using compiler.frontend;
using NUnit.Framework;
using compiler.middleend.ir;

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class CfgTests
    {
        public Cfg TestCfg { get; set; }

        [SetUp]
        public void Init()
        {
            TestCfg = new Cfg(new SymbolTable());
        }

        [Test]
        public void TestMethod()
        {
            // TODO: Add your test code here
            Assert.Pass("Your first passing test");
        }
    }
}
