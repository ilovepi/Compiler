using compiler.middleend.ir;
using NUnit.Framework;

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class InstructionTests
    {
        [Test]
        public void ToStringTest()
        {
            var inst1 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            Assert.AreEqual( inst2.Num.ToString() + " Add #5 (" + inst1.Num.ToString() + ")",inst2.ToString());


            

        }
    }
}
