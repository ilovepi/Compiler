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

            foreach (Token parsedToken in expectedToks)
            {
                var t = Lex.GetNextToken();
                Assert.AreEqual(t, parsedToken);
            }    
        }


        [Test]
        public void DestructorTest()
        {
            // pFilename -- arbitrary
            string filename = TestContext.CurrentContext.TestDirectory + @"/Frontend/LexerTest1.txt";

            // create a lexer
            Lex = new Lexer(filename);

            // release the lexer, so that the file will close
            Lex = null;
            Assert.Null(Lex);

            // invoke garbage collector to ensure destructor runs now
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // check if the file is infact, closed
            var fileInfo = new FileInfo(filename);
            Assert.False(IsFileinUse(fileInfo));
        }

        protected virtual bool IsFileinUse(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                stream?.Close();
            }
            return false;
        }


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

            Lex = new Lexer(filename);

            Token t;
            do
            {
                t = Lex.GetNextToken();
                Console.WriteLine(TokenHelper.PrintToken(t));
                Assert.AreNotEqual(t, Token.UNKNOWN);
            } while (t != Token.EOF);

        }

    }
}
