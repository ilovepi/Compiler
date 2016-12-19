package compiler.parser.ast;

import compiler.lexer.ParserException;
import compiler.lexer.Token;
import compiler.lexer.TokenNode;

/**
 * Created by paul on 12/18/16.
 */
public class Num extends ASTNode{
    int value;
    TokenNode tokenNode;

    private Num(int val, TokenNode tn)
    {
        value = val;
        tokenNode = tn;
    }

    public static Num createNum() throws ParserException {
        getNextToken(Token.NUMBER);
        return new Num(Integer.parseInt(currToken.getS()), currToken);
    }

    @Override
    public void visit() {

    }

    @Override
    public void print() {

    }

    public String toString()
    {
        return tokenNode.toString();
    }
}
