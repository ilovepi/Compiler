﻿namespace compiler.middleend.ir
{
    public class WhileNode : CompareNode
    {
        //public Node FalseNode { get; set; }


        public Node LoopParent { get; set; }

        //todo: rightnow we insert on the false node, but we need to fix that
        public WhileNode(BasicBlock pBB) : base(pBB)
        {
            base.NodeType = NodeTypes.WhileB;
            FalseNode = null;
            LoopParent = null;
        }

    }
}