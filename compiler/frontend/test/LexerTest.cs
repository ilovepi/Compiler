using NUnit.Framework;
using System;
using System.IO;

namespace compiler.frontend.test
{
    [TestFixture]
    public class LexerTest
    {
        Lexer lex;

        [Test]
        public void badFilenameTest()
        {
            // fake file should not exist
            var ex = Assert.Throws<FileNotFoundException>(() => lex = new Lexer("fake-file.txt"));
        }

        [Test]
        //[DeploymentItem("LexerTest1.txt", "targetFolder")]
        public void nextTest()
        {
            lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/frontend/test/LexerTest1.txt");

            // Exact string contents of LExerTest1.txt without the '.'
            string str = "Read some characters";

            foreach (char c in str)
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
            //TODO: we need another test like this that covers every part of the classifier
            lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/frontend/test/testdata/test001.txt");
            Token[] expected_toks = new Token[]{Token.COMMENT,
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

            foreach (Token parsed_token in expected_toks)
            {
                t = lex.getNextToken();
                Assert.AreEqual(t, parsed_token);
            }    
        }

    }
}
