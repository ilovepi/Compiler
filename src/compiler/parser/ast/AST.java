package compiler.parser.ast;

/**
 * Created by paul on 12/14/16.
 * <p>
 * Abstract Syntax Tree Class
 */
public class AST {

    private ASTNode root;


    public AST() {
        root = new ASTNode(ASTNode.NodeType.program);
    }

    public void createAST(ASTNode new_root) {
        root = new_root;
    }

    public boolean insert(ASTNode node, ASTNode root, ASTNode.NodeType parent, boolean left) throws Exception {
        if (root.m_type == parent) {
            if (left) {
                if (root.left == null) {
                    root.left = node;
                    return true;
                }
            } else if (root.right == null) {
                root.right = node;
                return true;
            } else {
                throw new Exception("Could not insert node: " + node.m_value + " into AST");
            }

        }

    }

    public void search(ASTNode node, ASTNode.NodeType t) {
        //if(node)

    }


}
