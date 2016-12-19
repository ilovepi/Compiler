package compiler.parser.ast;

import compiler.lexer.ParserException;
import compiler.lexer.Token;
import compiler.lexer.TokenNode;

/**
 * Created by paul on 12/18/16.
 */
public class ArrayOperator extends ASTNode {

    private TokenNode openBrace;
    private TokenNode closeBrace;
    private Expression expression;
    private ArrayOperator next;

    private ArrayOperator(TokenNode open, Expression exp, TokenNode close) {
        openBrace = open;
        expression = exp;
        closeBrace = close;
    }

    public static ArrayOperator createArrayOperator() throws ParserException {

        // do stuff to make an array operator node
        // read open brace
        if(nextToken.getT() != Token.OPEN_BRACKET)
            return null;

        getNextToken();

        checkError(Token.OPEN_BRACKET);

        TokenNode open = currToken;

        getNextToken();

        //read expression
        Expression exp = Expression.createExpression();

        //read close brace
        checkError(Token.CLOSE_BRACKET);

        TokenNode close = currToken;

        // create an ArrayOperator node
        ArrayOperator aryOp = new ArrayOperator(open,exp, close);

        // link it to other array operator nodes
        aryOp.next = createArrayOperator();

        return aryOp;
    }

    @Override
    public String toString() {
        return openBrace + expression.toString() + closeBrace;
    }

    @Override
    public void print() {
        System.out.println(this.toString());
    }

    @Override
    public void visit() {
        //openBrace.;

        expression.visit();
        //closeBrace.
    }


}
