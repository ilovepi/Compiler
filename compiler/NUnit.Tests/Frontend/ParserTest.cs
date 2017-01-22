using compiler.frontend;
using NUnit.Framework;

namespace NUnit.Tests.Frontend
{
    [TestFixture]
    public class ParserUnitTest
    {
        [Test]
        public void IsRelOpTest()
        {

            for (Token t = Token.UNKNOWN; t <= Token.IDENTIFIER; t++)
            {
                using (var p = new Parser(TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/test001.txt")) 
                {
                    p.Tok = t;
                    var expected = ((t >= Token.EQUAL) && (t <= Token.GREATER_EQ));
                    Assert.AreEqual(expected, p.IsRelOp());
                }
            }
        }






        
    }







}
