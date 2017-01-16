using NUnit.Framework;
using System;

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

            // Exact string contents of LExerTest1.txt
            string str = "Read some characters";

            foreach(char c in str)
            {
                char b = lex.next();
                //Console.WriteLine("Scanner expects '" + c + "', but read '" + b +"'");
                Assert.AreEqual(c, b);
            }

            // make sure we throw an exception for reading past the end of the file
            var ex = Assert.Throws<Exception>(() => lex.next());
            Assert.That(ex.Message, Is.EqualTo("Error: Lexer cannot read beyond the end of the file"));

        }
    }
}
