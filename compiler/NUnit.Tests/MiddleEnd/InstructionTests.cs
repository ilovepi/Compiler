using compiler.middleend.ir;
using NUnit.Framework;

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class InstructionTests
    {
        public Instruction Inst1 { get; set; }

        public Instruction Inst2 { get; set; }

        [SetUp]
        public void Init()
        {
            Inst1 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 10),
               new Operand(Operand.OpType.Identifier, 10));

            Inst2 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 05),
                new Operand(Inst1));
        }


        [Test]
        public void ToStringTest()
        {
           

            Assert.AreEqual( Inst2.Num.ToString() + ": Add #5 (" + Inst1.Num.ToString() + ")",Inst2.ToString());
        }

        [Test]
        public void EqualsNotEqualTest()
        {

            Assert.AreNotEqual(Inst2, Inst1);
            Assert.AreNotEqual(Inst2, null);
            Assert.AreNotEqual(Inst1, null);

            Assert.AreNotEqual(Inst1, new List());
        }


        [Test]
        public void EqualsTest()
        {
            Assert.AreEqual(Inst2, Inst2);

            Assert.AreEqual(Inst1, new Instruction(Inst1));

            
        }
    }
}
