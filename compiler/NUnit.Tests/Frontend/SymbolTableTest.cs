using System.Data;
using compiler.frontend;
using NUnit.Framework;

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

        public SymbolTable Table { get; set; }


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