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
            Lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/LexerTest1.txt");

            // Exact string contents of LExerTest1.txt without the '.'
            string str = "Read some characters";

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
        public void NextIncrementsPosition()
        {
            Lex = new Lexer(TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/test001.txt");

            bool increasing;
            Token t;
            int curr_line;
            int prev_line;
            int cur_pos;
            int prev_pos;
            do
            {
                prev_pos = Lex.Position;
                prev_line = Lex.LineNo;
                
                t = Lex.GetNextToken();
                
                cur_pos = Lex.Position;
                curr_line = Lex.LineNo;

                if (curr_line > prev_line)
                {
                    increasing = false;
                }
                else
                {
                    increasing = true;
                }

                if (prev_pos == cur_pos)
                {
                    if(t != Token.EOF)
                        Assert.AreNotEqual(prev_line, curr_line);
                }
                else
                {
                    Assert.AreNotEqual(prev_pos,cur_pos);
                    if (increasing)
                    {
                        Assert.Less(prev_pos, cur_pos);
                    }
                    else
                    {
                        Assert.Greater(prev_pos, cur_pos);
                    }
                }

            } while (t != Token.EOF);

            Assert.AreEqual(8,curr_line);
            Assert.AreEqual(2,cur_pos);
        }



        [Test]
        public void DestructorTest()
        {
            
                // pFilename -- arbitrary
            string filename = TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/LexerTest1.txt";
            { 
                // create a lexer
                Lex = new Lexer(filename);

                // release the lexer, so that the file will close
                Lex = null;
                Assert.Null(Lex);

                // invoke garbage collector to ensure destructor runs now
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

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
                    t = Token.UNKNOWN;
                    throw;
                }

                Assert.True( (t == Token.UNKNOWN) || (t == Token.EOF ) );
                if (t == Token.UNKNOWN)
                {

                    var a = TokenHelper.PrintToken(t);
                    var b = TokenHelper.ToString(t);
                    Assert.AreEqual( "UNKNOWN", a);
                    Assert.AreEqual( "unknown", b);
                }

            } while (t != Token.EOF);

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
                Assert.AreNotEqual(b,"unknown");
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

       

    }
}
