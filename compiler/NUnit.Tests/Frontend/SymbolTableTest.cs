﻿using NUnit.Framework;
using System.Data;
using compiler.frontend;

namespace NUnit.Tests.Frontend
{
    [TestFixture]
    public class SymbolTableTest
    {
        public SymbolTable Table { get; set; }

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

        
        [Test]
        public void InsertExceptionThrowsTest()
        {
            var ex = Assert.Throws<DuplicateNameException>(() =>Table.Insert(".unknown"));
            Assert.That(ex.Message, Is.EqualTo("Error: Symbol Table cannot contain duplicate symbols: .unknown"));
        }


        [Test]
        public void IsIdTest()
        {
            Table.Insert("x");
            Assert.True(Table.IsId("x"));

            // Token.IDENTIFIER should be less than X's value in symbol table
            // could also just check for equality with ".identifier" and Token.IDENTIFIER
            Assert.Less((int)Token.IDENTIFIER, Table.Values["x"]);
        }


    }
}