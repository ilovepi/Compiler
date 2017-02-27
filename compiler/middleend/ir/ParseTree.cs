namespace compiler.middleend.ir
{
	public class ParseTree
	{
		public ParseTree()
		{
			ControlFlowGraph = new Cfg();
			DominatorTree = new DomTree();
		}

		public ParseTree(Cfg pCfg, DomTree pDom)
		{
			ControlFlowGraph = pCfg;
			DominatorTree = pDom;
		}

		public Cfg ControlFlowGraph { get; set; }
		public DomTree DominatorTree { get; set; }
	}
}
