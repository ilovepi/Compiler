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

using System.Data;
using compiler.frontend;
using NUnit.Framework;

#endregion

namespace NUnit.Tests.Frontend
{
    [TestFixture]
    public class SymbolTableTest
    {
        [SetUp]
        public void Init()
        {
            Table = new SymbolTable();
        }

        [TearDown]
        public void Dispose()
        {
            Table = null;
        }

        private SymbolTable Table { get; set; }


        [Test]
        public void InsertAddressTest()
        {
            Table.Insert("x");
            Table.InsertAddress(Table.Values["x"], 0xfff);
            Assert.AreEqual(0xfff, Table.AddressTble[Table.Values["x"]]);
        }


        [Test]
        public void InsertExceptionThrowsTest()
        {
            var ex = Assert.Throws<DuplicateNameException>(() => Table.Insert(".unknown"));
            Assert.That(ex.Message, Is.EqualTo("Error: Symbol Table cannot contain duplicate symbols: .unknown"));
        }

        [Test]
        public void InsertKeyAddressTest()
        {
            Table.Insert("x", 0xDEAD);
            Assert.AreEqual(0xDEAD, Table.AddressTble[Table.Values["x"]]);
        }

        [Test]
        public void IsIdTest()
        {
            Table.Insert("x");
            Assert.True(Table.IsId("x"));

            // Token.IDENTIFIER should be less than X's value in symbol table
            // could also just check for equality with ".identifier" and Token.IDENTIFIER
            Assert.Less((int) Token.IDENTIFIER, Table.Values["x"]);
        }
    }
}