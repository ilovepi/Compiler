using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using compiler.middleend.ir;

namespace NUnit.Tests.Middle_End
{
    [TestFixture]
    public class InstructionTests
    {
        [Test]
        public void ToStringTest()
        {
            var inst1 = new Instruction(IrOps.add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            Assert.AreEqual( inst2.Num.ToString() + " add #5 (" + inst1.Num.ToString() + ")",inst2.ToString());


            

        }
    }
}
