package compiler.parser.ast;

import compiler.lexer.TokenNode;

/**
 * Created by paul on 12/15/16.
 */
public class VarDeclNode extends ASTNode<String>{

    public boolean isArray;
    public int arraySize;


    public VarDeclNode(TokenNode tn)
    {
        super(tn.getT(), tn.getS(), NodeType.varDecl);

    }
}


