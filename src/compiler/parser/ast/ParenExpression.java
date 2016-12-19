package compiler.parser.ast;

import compiler.lexer.ParserException;
import compiler.lexer.Token;
import compiler.lexer.TokenNode;

/**
 * Created by paul on 12/18/16.
 */
public class ParenExpression extends ASTNode {
    private TokenNode openParen;
    private TokenNode closeParen;
    private Expression expression;
    private ParenExpression next;

    private ParenExpression(TokenNode open, Expression exp, TokenNode close) {
        openParen = open;
        expression = exp;
        closeParen = close;
    }

    public static ParenExpression createParenExpression() throws ParserException {

        // do stuff to make an array operator node
        // read open brace
        if (nextToken.getT() != Token.OPEN_PAREN)
            return null;

        getNextToken();

        checkError(Token.OPEN_PAREN);

        TokenNode open = currToken;

        getNextToken();

        //read expression
        Expression exp = Expression.createExpression();

        //read close brace
        checkError(Token.CLOSE_PAREN);

        TokenNode close = currToken;

        // create an ArrayOperator node
        ParenExpression paren = new ParenExpression(open, exp, close);

        // link it to other array operator nodes
        paren.next = createParenExpression();

        return paren;
    }

    @Override
    public String toString() {
        return openParen + expression.toString() + closeParen;
    }

    @Override
    public void print() {
        System.out.println(this.toString());
    }

    @Override
    public void visit() {
        //openParen.;

        expression.visit();
        //closeParen.
    }

}
