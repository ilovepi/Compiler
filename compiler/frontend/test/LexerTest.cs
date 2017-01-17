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
        public void nextTest()
        {
            lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/frontend/test/LexerTest1.txt");            

            // Exact string contents of LExerTest1.txt without the '.'
            string str = "Read some characters";

            foreach(char c in str)
            {
                //Console.WriteLine("Scanner expects '" + c + "', but read '" + b +"'");
                Assert.AreEqual(c, lex.c);
                lex.next();
            }

            // make sure we throw an exception for reading past the end of the file
            var ex = Assert.Throws<Exception>(() => lex.next());
            Assert.That(ex.Message, Is.EqualTo("Error: Lexer cannot read beyond the end of the file"));
        }

        [Test]
        public void getNextTokenTest()
        {
            Assert.Fail();
        }

    }
}
