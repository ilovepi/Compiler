using System;
using System.IO;
using compiler.frontend;
using NUnit.Framework;

namespace NUnit.Tests.Frontend
{
    [TestFixture]
    public class LexerTest
    {
        public Lexer Lex { get; private set; }

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
            string a, b;
            do
            {
                t = Lex.GetNextToken();
                a = TokenHelper.PrintToken(t);
                b = TokenHelper.ToString(t);
                Assert.AreNotEqual(a, "UNKNOWN");
                Assert.AreNotEqual(b, "unknown");
                Assert.AreNotEqual(a, "ERROR");
                Assert.AreNotEqual(b, "ERROR");
                if (t != Token.EOF)
                {
                    Assert.AreNotEqual(a, b);
                }
                //Console.WriteLine(TokenHelper.PrintToken(t));
                Assert.AreNotEqual(t, Token.UNKNOWN);
            } while (t != Token.EOF);
        }

        [Test]
        public void BadFilenameTest()
        {
            // fake file should not exist
            Assert.Throws<FileNotFoundException>(() => Lex = new Lexer("fake-file.txt"));
        }


        [Test]
        public void ClassifyUnknownTest()
        {
            string filename = TestContext.CurrentContext.TestDirectory +
                              @"/Frontend/testdata/BadTokens.txt";

            Lex = new Lexer(filename);
            Token t;
            do
            {
                try
                {
                    t = Lex.GetNextToken();
                }
                catch (IOException exception)
                {
                    Console.WriteLine(exception.Message);
                    t = Token.EOF;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                    //t = Token.UNKNOWN;

                    throw;
                }

                Assert.True((t == Token.UNKNOWN) || (t == Token.EOF));
                if (t == Token.UNKNOWN)
                {
                    string a = TokenHelper.PrintToken(t);
                    string b = TokenHelper.ToString(t);
                    Assert.AreEqual("UNKNOWN", a);
                    Assert.AreEqual("unknown", b);
                }
            } while (t != Token.EOF);
        }


        [Test]
        public void DestructorTest()
        {
            // pFilename -- arbitrary
            string filename = TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/LexerTest1.txt";


            using (Lex = new Lexer(filename))
            {
                Assert.AreEqual(1, Lex.Position);
            }

            using (var l = new Lexer(filename))
            {
                Assert.AreEqual(1, l.LineNo);
            }

            // invoke garbage collector to ensure destructor runs now
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // check if the file is infact, closed
            var fileInfo = new FileInfo(filename);
            Assert.False(IsFileinUse(fileInfo));
        }


        [Test]
        public void GetNextTokenTest()
        {
            //TODO: we need another test like this that covers every part of the classifier
            Lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/test001.txt");
            Token[] expectedToks =
            {
                Token.COMMENT,
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
                Token t = Lex.GetNextToken();
                Assert.AreEqual(t, parsedToken);
            }
        }

        [Test]
        public void NextIncrementsPosition()
        {
            Lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/test001.txt");

            Token t;
            int currLine;
            int curPos;
            do
            {
                int prevPos = Lex.Position;
                int prevLine = Lex.LineNo;

                t = Lex.GetNextToken();

                curPos = Lex.Position;
                currLine = Lex.LineNo;


                // if the current line is greater, then we've wrapped, and the 
                // current position is no longer guranteed to remain increasing
                bool wrappedLine = currLine > prevLine;

                // the position should increment except if we're at the end of a file
                if (prevPos == curPos)
                {
                    if (t != Token.EOF)
                    {
                        Assert.AreNotEqual(prevLine, currLine);
                    }
                }
                else
                {
                    // if we've wrapped a line, expect the next line position to be
                    // less than the current position
                    Assert.AreNotEqual(prevPos, curPos);
                    if (!wrappedLine)
                    {
                        Assert.Less(prevPos, curPos);
                    }
                    else
                    {
                        Assert.Greater(prevPos, curPos);
                    }
                }
            } while (t != Token.EOF);

            Assert.AreEqual(8, currLine);
            Assert.AreEqual(3, curPos);
        }

        [Test]
        //[DeploymentItem("LexerTest1.txt", "targetFolder")]
        public void NextTest()
        {
            Lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/LexerTest1.txt");

            // Exact string contents of LExerTest1.txt without the '.'
            const string str = "Read some characters";

            foreach (char c in str)
            {
                //Console.WriteLine("Scanner expects '" + c + "', but read '" + b +"'");
                Assert.AreEqual(c, Lex.C);
                Lex.Next();
            }

            // make sure we throw an exception for reading past the end of the file
            var ex = Assert.Throws<IOException>(() => Lex.Next());
            Assert.That(ex.Message, Is.EqualTo("Error: Lexer cannot read beyond the end of the file"));
        }
    }
}