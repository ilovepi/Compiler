using System;
using System.IO;
using NUnit.Framework;
using compiler.frontend;

namespace NUnit.Tests.Frontend
{
    [TestFixture]
    public class LexerTest
    {
        public Lexer Lex { get; private set; }

        [Test]
        public void BadFilenameTest()
        {
            // fake file should not exist
            Assert.Throws<FileNotFoundException>(() => Lex = new Lexer("fake-file.txt"));
        }

        [Test]
        //[DeploymentItem("LexerTest1.txt", "targetFolder")]
        public void NextTest()
        {
            Lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/Frontend/LexerTest1.txt");

            // Exact string contents of LExerTest1.txt without the '.'
            string str = "Read some characters";

            foreach (char c in str)
            {
                //Console.WriteLine("Scanner expects '" + c + "', but read '" + b +"'");
                Assert.AreEqual(c, Lex.C);
                Lex.Next();
            }

            // make sure we throw an exception for reading past the end of the file
            var ex = Assert.Throws<Exception>(() => Lex.Next());
            Assert.That(ex.Message, Is.EqualTo("Error: Lexer cannot read beyond the end of the file"));
        }


        [Test]
        public void GetNextTokenTest()
        {
            //TODO: we need another test like this that covers every part of the classifier
            Lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/test001.txt");
            Token[] expectedToks = new Token[]{Token.COMMENT,
                                                    Token.MAIN,
                                                    Token.VAR,
                                                    Token.IDENTIFIER,
                                                    Token.COMMA,
                                                    Token.IDENTIFIER,
                                                    Token.SEMI_COLON,
                                                    Token.OPEN_CURL,
                                                    Token.LET,
                                                    Token.IDENTIFIER,
                                                    Token.ASSIGN,
                                                    Token.NUMBER,
                                                    Token.SEMI_COLON,
                                                    Token.LET,
                                                    Token.IDENTIFIER,
                                                    Token.ASSIGN,
                                                    Token.NUMBER,
                                                    Token.TIMES,
                                                    Token.IDENTIFIER,
                                                    Token.SEMI_COLON,
                                                    Token.CALL,
                                                    Token.IDENTIFIER,
                                                    Token.OPEN_PAREN,
                                                    Token.IDENTIFIER,
                                                    Token.CLOSE_PAREN,
                                                    Token.CLOSE_CURL,
                                                    Token.EOF
                                                    };

            Token t;

            foreach (Token parsedToken in expectedToks)
            {
                t = Lex.GetNextToken();
                Assert.AreEqual(t, parsedToken);
            }    
        }

    }
}
