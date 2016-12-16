package compiler.parser.ast;

/**
 * Created by paul on 12/14/16.
 *
 * Abstract Syntax Tree Class
 */
public class AST {

    private ASTNode root;


    public AST(){
        root = new ASTNode(ASTNode.NodeType.program);
    }

    public void createAST(ASTNode new_root)
    {
        root = new_root;
    }


    public void search(ASTNode node,ASTNode.NodeType t)
    {
        //if(node)

    }


}
