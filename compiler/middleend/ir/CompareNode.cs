﻿using System;
using System.Collections.Generic;

namespace compiler.middleend.ir
{
    public class CompareNode : Node
    {
        public CompareNode(BasicBlock pBb) : base(pBb, NodeTypes.CompareB)
        {
            Colorname = "cornflowerblue";
            FalseNode = null;
        }


        public Node FalseNode { get; set; }
		public JoinNode Join { get; set; }


        public void Insert(Node other, bool trueChild)
        {
            if (trueChild)
            {
                Child = other;
                other.UpdateParent(this);
            }
            else
            {
                FalseNode = other;
                other.UpdateParent(this);
            }
        }

      
        public void InsertFalse(Node other)
        {
            Insert(other, false);
        }

        public void InsertTrue(Node other)
        {
            Insert(other, true);
        }

//        public override List<Node> GetAllChildren()
//        {
//            List<Node> ret = base.GetAllChildren();
//            ret.Add(FalseNode);
//            return ret;
//        }

        public override void CheckEnqueue(Cfg cfg)
        {
            cfg.BfsCheckEnqueue(this, Child);
            cfg.BfsCheckEnqueue(this, FalseNode);
        }

        public override void Consolidate()
        {
            base.CircularRef(Child);
            base.CircularRef(FalseNode);

            // consolidate children who exist
            Child?.Consolidate();
            FalseNode?.Consolidate();
        }
    }
}