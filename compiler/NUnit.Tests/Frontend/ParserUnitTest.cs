using System;
using System.Collections.Generic;
using compiler.frontend;
using compiler.middleend.ir;
using NUnit.Framework;

namespace NUnit.Tests.Frontend
{
    [TestFixture]
    public class ParserUnitTest
    {
        [SetUp]
        public void Init()
        {
            ProgramDir = TestContext.CurrentContext.TestDirectory;
            ProgramPath = ProgramDir + @"/Frontend/testdata/test001.txt";
            Checker = new Parser(ProgramPath);
        }

        public string ProgramPath { get; private set; }
        public string ProgramDir { get; set; }

        public Parser Checker { get; set; }

        [Test]
        public void DesignatorArrayTest()
        {
            //TODO: add more cases to the test file, and hit them all
            using (Checker = new Parser(ProgramDir + @"/Frontend/parserdata/Array.txt"))
            {
                var v = new SortedDictionary<int, SsaVariable>();
                Checker.Next();
                Checker.Designator(ref v);
                Checker.Designator(ref v);
                Checker.Designator(ref v);
            }
        }

        [Test]
        public void DesignatorBadExpressionTest()
        {
            //TODO: add more cases to the test file, and hit them all
            using (Checker = new Parser(ProgramDir + @"/Frontend/parserdata/BadExpression.txt"))
            {
                Checker.Next();
                var v = new SortedDictionary<int, SsaVariable>();
                Assert.Throws<ParserException>(() => Checker.Designator(ref v));
            }
        }


        [Test]
        public void DesignatorIdOnlyTest()
        {
            //TODO: add more cases to the test file, and hit them all
            using (Checker = new Parser(ProgramDir + @"/Frontend/parserdata/Identifier.txt"))
            {
                var v = new SortedDictionary<int, SsaVariable>();
                Checker.Next();
                Checker.Designator(ref v);
            }
        }


        [Test]
        public void DesignatorMultiDimensionalArrayTest()
        {
            //TODO: add more cases to the test file, and hit them all
            using (Checker = new Parser(ProgramDir + @"/Frontend/parserdata/MultiDimArray.txt"))
            {
                var v = new SortedDictionary<int, SsaVariable>();
                Checker.Next();
                Checker.Designator(ref v);
                Checker.Designator(ref v);
                Checker.Designator(ref v);
            }
        }


        [Test]
        public void GetExpectedAdvanceTest()
        {
            Assert.AreEqual(1, Checker.Pos);
            Assert.AreEqual(1, Checker.LineNo);
            Checker.GetExpected(Token.UNKNOWN);
            Assert.AreEqual(Token.MAIN, Checker.Tok);
            Assert.AreEqual(6, Checker.Pos);
            Assert.AreEqual(2, Checker.LineNo);

            Checker.GetExpected(Token.MAIN);
            Assert.AreEqual(Token.VAR, Checker.Tok);
            Assert.AreEqual(5, Checker.Pos);
            Assert.AreEqual(3, Checker.LineNo);
        }


        [Test]
        public void GetExpectedThrowsTest()
        {
            Assert.Throws<ParserException>(() => Checker.GetExpected(Token.EOF));
        }

        //[TearDown]
        //public void cleanup()
        //{
        //}


        [Test]
        public void IsRelOpTest()
        {
            for (var t = Token.UNKNOWN; t <= Token.IDENTIFIER; t++)
            {
                //using (var p = new Parser(ProgramPath)) 
                //{
                Checker.Tok = t;
                bool expected = (t >= Token.EQUAL) && (t <= Token.GREATER_EQ);
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
    }
}