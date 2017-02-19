namespace compiler.middleend.ir
{
    public class DomTree
    {
        public DomTree()
        {
            Root = null;
        }

        public DominatorNode Root { get; set; }


        public void RecursiveDfs(DominatorNode curNode)
        {
            // TODO: include display or print function here
            foreach (DominatorNode child in curNode.Children)
            {
                if (child != null)
                {
                    RecursiveDfs(child);
                }
            }
        }

        public void DfsTraversal()
        {
            if (Root != null)
            {
                RecursiveDfs(Root);
            }
        }
    }
}