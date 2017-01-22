using System;
using compiler.frontend;
using NUnit.Framework;

namespace NUnit.Tests.Frontend
{
    [TestFixture]
    public class ParserUnitTest
    {
        public string ProgramPath { get; private set; }

        public Parser Checker { get; set; }



        [SetUp]
        public void Init()
        {
            ProgramPath = TestContext.CurrentContext.TestDirectory + @"/Frontend/testdata/test001.txt";
            Checker = new Parser(ProgramPath);
        }

        //[TearDown]
        //public void cleanup()
        //{
        //}


        [Test]
        public void IsRelOpTest()
        {

            for (Token t = Token.UNKNOWN; t <= Token.IDENTIFIER; t++)
            {
                //using (var p = new Parser(ProgramPath)) 
                //{
                    Checker.Tok = t;
                    var expected = ((t >= Token.EQUAL) && (t <= Token.GREATER_EQ));
                    Assert.AreEqual(expected, Checker.IsRelOp());
                //}
            }
        }


        [Test]
        public void ParserDestructorTest()
        {
            Checker = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            using (Checker = new Parser(ProgramPath))
            {
                Assert.AreEqual(1, Checker.Scanner.LineNo);
                
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }


        [Test]
        public void GetExpectedThrowsTest()
        {
            var ex =Assert.Throws<ParserException>(() => Checker.GetExpected(Token.UNKNOWN));
           
        }





        
    }







}
