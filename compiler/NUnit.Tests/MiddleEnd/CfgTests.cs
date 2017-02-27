using compiler.frontend;
using compiler.middleend.ir;
using NUnit.Framework;

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class CfgTests
    {
        private Cfg TestCfg { get; set; }

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