package compiler.parser.ast;

import compiler.lexer.Token;
import compiler.lexer.TokenNode;

/**
 * Created by paul on 12/18/16.
 */
public class Identifier implements ASTNode{
    TokenNode tn;

    private Identifier() {
    }

    private Identifier(TokenNode tokenNode) {
        tn = tokenNode;
    }

    public String toString() {
        return tn.getT().toString() + " : " + tn.getS();
    }

    public Identifier createIdentifier(TokenNode tn) {
        return tn.getT() != Token.IDENTIFIER ? null : new Identifier(tn);
    }



}
