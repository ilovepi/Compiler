using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using compiler.frontend;

namespace compiler.frontend.test
{
    [TestFixture]
    public class LexerTest
    {
        Lexer lex;

        [Test]
        //[DeploymentItem("LexerTest1.txt", "targetFolder")]
        public void TestMethod()
        {
            lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"\frontend\test\LexerTest1.txt");            

            string str = "Read some character";

            foreach(char c in str)
            {
                char b = lex.next();
                Console.WriteLine("Scanner expects '" + c + "', but read '" + b +"'");
                Assert.AreEqual( c, b);
            }

            //Assert.Throws( )
        }
    }
}
