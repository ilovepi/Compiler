using System.Collections.Generic;
using compiler.middleend.ir;
namespace compiler
{
	public class Node
	{
		public BasicBlock BB { get; set; }

        /// <summary>
        /// Parent from a true branch(left)
        /// </summary>
		public Node TrueParent { get; set; }

        /// <summary>
        /// Parent from a false branch (right)
        /// </summary>
		public Node FalseParent { get; set; }

        /// <summary>
        /// Child on the true branch (left)
        /// </summary>
		public Node TrueChild { get; set; }


        /// <summary>
        /// Child on the false branch (right)
        /// </summary>
		public Node FalseChild { get; set; }



		public Node(BasicBlock pBB)
		{
			BB = pBB;
			TrueChild = null;
			FalseChild = null;
			TrueParent = null;
			FalseParent = null;
		}


	    public bool IsRoot()
	    {
	        return (TrueParent == null) && (FalseParent == null);
	    }


	    public void Insert(Node other, bool truePath)
	    {
	        if (truePath)
	        {
	            if (TrueChild == null)
	            {
	                TrueChild = other;
	                other.TrueParent = this;
	            }
	            else
	            {
	                TrueChild.Insert(other,true);
	            }
	        }
	        else
	        {
                if (FalseChild == null)
                {
                    FalseChild = other;
                    other.FalseParent = this;
                }
                else
                {
                    FalseChild.Insert(FalseChild , false);
                }
            }


        }




	}
}
