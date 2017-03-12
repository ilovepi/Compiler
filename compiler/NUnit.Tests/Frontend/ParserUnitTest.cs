#region Basic header

// MIT License
// 
// Copyright (c) 2016 Paul Kirth
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using compiler.frontend;
using compiler.middleend.ir;
using NUnit.Framework;

#endregion

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
            Checker = new Parser(ProgramPath, true);
        }

        public string ProgramPath { get; private set; }
        public string ProgramDir { get; set; }

        public Parser Checker { get; set; }

        [Test]
        public void DesignatorArrayTest()
        {
            //TODO: add more cases to the test file, and hit them all
            using (Checker = new Parser(ProgramDir + @"/Frontend/parserdata/Array.txt", true))
            {
                var v = new SortedDictionary<int, SsaVariable>();
                Checker.Next();
                Checker.VarDecl(v);
                Checker.Designator(v);
                Checker.Designator(v);
                Checker.Designator(v);
            }
        }

        [Test]
        public void DesignatorBadExpressionTest()
        {
            //TODO: add more cases to the test file, and hit them all
            using (Checker = new Parser(ProgramDir + @"/Frontend/parserdata/BadExpression.txt", true))
            {
                Checker.Next();
                var v = new SortedDictionary<int, SsaVariable>();
                Checker.VarDecl(v);
                Assert.Throws<ParserException>(() => Checker.Designator(v));
            }
        }


        [Test]
        public void DesignatorIdOnlyTest()
        {
            //TODO: add more cases to the test file, and hit them all
            using (Checker = new Parser(ProgramDir + @"/Frontend/parserdata/Identifier.txt", true))
            {
                var v = new SortedDictionary<int, SsaVariable>();
                Checker.Next();
                Checker.Designator(v);
            }
        }


        [Test]
        public void DesignatorMultiDimensionalArrayTest()
        {
            //TODO: add more cases to the test file, and hit them all
            using (Checker = new Parser(ProgramDir + @"/Frontend/parserdata/MultiDimArray.txt", true))
            {
                var v = new SortedDictionary<int, SsaVariable>();
                Checker.Next();
                Checker.VarDecl(v);
                Checker.Designator(v);
                Checker.Designator(v);
                Checker.Designator(v);
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

            using (Checker = new Parser(ProgramPath, true))
            {
                Assert.AreEqual(1, Checker.Scanner.LineNo);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}