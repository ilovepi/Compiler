using System;
using System.Collections.Generic;
using compiler.middleend.ir;
namespace compiler
{
	public class Node
	{
	    public enum NodeTypes
	    {
	        BB,
	        CompareB,
	        JoinB,
	        WhileB
	    };

		public BasicBlock BB { get; set; }

	    public NodeTypes NodeType;

        /// <summary>
        /// Parent Node
        /// </summary>
		public Node Parent { get; set; }

       

        /// <summary>
        /// Successor Node
        /// </summary>
		public Node Child { get; set; }

	    public int BlockNumber;



        public Node(BasicBlock pBB)
		{
			BB = pBB;
		    Parent = null;
		    Child = null;
		    NodeType = NodeTypes.BB;

		}

	    public Node(BasicBlock pBB, NodeTypes n)
	    {
	        BB = pBB;
	        Parent = null;
	        Child = null;
	        NodeType = n;

	    }



	    public bool IsRoot()
	    {
	        return (Parent == null);
	    }


	    public virtual void Insert(Node other)
	    {
	        if (Child == null)
	        {
	            Child = other;
	            other.UpdateParent(this);
	        }
	        else
	        {
	            Child.Insert(other);
	        }
        }

	    public virtual void InsertTrue(JoinNode other)
	    {
	        Child = other;
	        other.Parent = this;

	    }

        public virtual void InsertFalse(JoinNode other)
        {
            other.FalseParent = this;
            Child = other;
        }

        //TODO: does this really work? maybe find a better design
        virtual public void UpdateParent(Node other)
	    {
	        Parent = other;
	    }

	    public static Node Leaf(Node root)
	    {
	        if (root == null)
	        {
	            return null;
            }
	        if (root.Child == null)
	        {
	            return root;
            }
	        else
	        {
	            return Leaf(root.Child);
	        }
	    }



        public static Node Leaf(WhileNode root)
        {
            if (root == null)
            {
                return null;
            }
            if (root.FalseNode == null)
            {
                return root;
            }
            else
            {
                return Leaf(root.FalseNode);
            }
        }

	    public virtual List<Node> GetAllChildren()
	    {
            var ret = new List<Node>(){Child};
	        return ret;
	    }

	    public static void consolodate(Node root)
	    {
	        if (root == null || root.Child == null)
	            return;

            if (object.ReferenceEquals(root, root.Child))
                throw new Exception("Circular reference in basic block!!");



            if (root.Child.GetType() == typeof(Node))
	        {
                root.BB.Instructions.AddRange(root.Child.BB.Instructions);

	            var temp = root.Child;
	            root.Child = temp.Child;

                consolodate(root);
	        }
           

	    }


    }
}
