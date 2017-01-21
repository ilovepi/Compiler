using NUnit.Framework;
using System;
using System.IO;

namespace compiler.frontend.test
{
    [TestFixture]
    public class LexerTest
    {
        Lexer _lex;

        [Test]
        public void BadFilenameTest()
        {
            // fake file should not exist
            Assert.Throws<FileNotFoundException>(() => _lex = new Lexer("fake-file.txt"));
        }

        [Test]
        //[DeploymentItem("LexerTest1.txt", "targetFolder")]
        public void NextTest()
        {
            _lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/frontend/test/LexerTest1.txt");

            // Exact string contents of LExerTest1.txt without the '.'
            string str = "Read some characters";

            foreach (char c in str)
            {
                //Console.WriteLine("Scanner expects '" + c + "', but read '" + b +"'");
                Assert.AreEqual(c, _lex.C);
                _lex.Next();
            }

            // make sure we throw an exception for reading past the end of the file
            var ex = Assert.Throws<Exception>(() => _lex.Next());
            Assert.That(ex.Message, Is.EqualTo("Error: Lexer cannot read beyond the end of the file"));
        }


        [Test]
        public void GetNextTokenTest()
        {
            //TODO: we need another test like this that covers every part of the classifier
            _lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/frontend/test/testdata/test001.txt");
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
                t = _lex.GetNextToken();
                Assert.AreEqual(t, parsedToken);
            }    
        }

    }
}
