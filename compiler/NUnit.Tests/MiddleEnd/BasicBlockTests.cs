using NUnit.Framework;
using compiler.middleend.ir;
using compiler.middleend.optimization;

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class BasicBlockTests
    {

        public BasicBlock Block { get; set; }

        [SetUp]
        public void Init()
        {
            Block = new BasicBlock();
        }

        [Test]
        public void ConstructorTest()
        {
            Assert.Null(Block.Name);
            Block = new BasicBlock("Test Block");
            Assert.AreEqual("Test Block", Block.Name);
        }




        [Test]
        public void SearchTest()
        {
            Cfg graphCfg = new Cfg();
            
            graphCfg.Root = new Node(new BasicBlock("MyBlock"));
            var inst1 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 10),
                new Operand(Operand.OpType.Identifier, 10));

            var inst2 = new Instruction(IrOps.Add, new Operand(Operand.OpType.Constant, 05),
                new Operand(inst1));

            graphCfg.Root.Bb.AddInstruction(inst1);
            graphCfg.Root.Bb.AddInstruction(inst1);
            graphCfg.Root.Bb.AddInstruction(inst2);


            graphCfg.Root.Insert(new Node(new BasicBlock("Child Block")));
            graphCfg.Root.Child.Bb.AddInstruction(inst2);
            CsElimination.Eliminate(graphCfg.Root);


            //graphCfg.Root.Insert(new Node(new BasicBlock("Child Block 2")));
            //graphCfg.Root.Insert(new Node(new BasicBlock("Child Block 2")));

            Assert.NotNull(graphCfg.Root.Leaf().AnchorSearch(inst1));
        }

    }
}
