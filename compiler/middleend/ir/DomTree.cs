using System;
using System.Collections.Generic;

namespace compiler
{
    class DomTree
    {
        public DominatorNode Root { get; set; }
        

        public DomTree()
        {
            Root = null;
        }


        public void RecursiveDFS(DominatorNode curNode)
        {
            // TODO: include display or print function here
            foreach (DominatorNode child in curNode.Children)
            {
                if (child != null)
                {
                    RecursiveDFS(child);
                }
            }
        }

        public void DFSTraversal()
        {
            if (Root != null)
            {
                RecursiveDFS(Root);
            }
        }

    }
}
