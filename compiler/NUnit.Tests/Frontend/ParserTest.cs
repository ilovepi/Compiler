using NUnit.Framework;
using compiler.frontend;



namespace NUnit.Tests.Frontend
{
    [TestFixture]
    public class ParserTest
    {

        [TestCase(@"/Frontend/testdata/test001.txt")]
        [TestCase(@"/Frontend/testdata/test002.txt")]
        [TestCase(@"/Frontend/testdata/test003.txt")]
        [TestCase(@"/Frontend/testdata/test004.txt")]
        [TestCase(@"/Frontend/testdata/test005.txt")]
        [TestCase(@"/Frontend/testdata/test006.txt")]
        [TestCase(@"/Frontend/testdata/test007.txt")]
        [TestCase(@"/Frontend/testdata/test008.txt")]
        [TestCase(@"/Frontend/testdata/test009.txt")]
        [TestCase(@"/Frontend/testdata/test010.txt")]
        [TestCase(@"/Frontend/testdata/test011.txt")]
        [TestCase(@"/Frontend/testdata/test012.txt")]
        [TestCase(@"/Frontend/testdata/test013.txt")]
        [TestCase(@"/Frontend/testdata/test014.txt")]
        [TestCase(@"/Frontend/testdata/test015.txt")]
        [TestCase(@"/Frontend/testdata/test016.txt")]
        [TestCase(@"/Frontend/testdata/test017.txt")]
        [TestCase(@"/Frontend/testdata/test018.txt")]
        [TestCase(@"/Frontend/testdata/test019.txt")]
        [TestCase(@"/Frontend/testdata/test020.txt")]
        [TestCase(@"/Frontend/testdata/test021.txt")]
        [TestCase(@"/Frontend/testdata/test022.txt")]
        [TestCase(@"/Frontend/testdata/test023.txt")]
        [TestCase(@"/Frontend/testdata/test024.txt")]
        [TestCase(@"/Frontend/testdata/test025.txt")]
        [TestCase(@"/Frontend/testdata/test026.txt")]
        [TestCase(@"/Frontend/testdata/test027.txt")]
        [TestCase(@"/Frontend/testdata/test028.txt")]
        [TestCase(@"/Frontend/testdata/test029.txt")]
        [TestCase(@"/Frontend/testdata/test030.txt")]
        [TestCase(@"/Frontend/testdata/test031.txt")]
        [TestCase(@"/Frontend/testdata/big.txt")]
        [TestCase(@"/Frontend/testdata/cell.txt")]
        [TestCase(@"/Frontend/testdata/factorial.txt")]
        public void TokenizingTest(string pFilename)
        {
            
            string filename = TestContext.CurrentContext.TestDirectory + pFilename;

            using (Parser p = new Parser(filename))
            {
                p.Parse();
                
            }
            Assert.Fail("Test and class not implemented ...");
        }

    }
}
