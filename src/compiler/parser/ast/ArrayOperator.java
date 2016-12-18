package compiler.parser.ast;

import compiler.lexer.Token;
import compiler.lexer.TokenNode;

/**
 * Created by paul on 12/18/16.
 */
public class ArrayOperator {

    private TokenNode openBrace;
    private TokenNode closeBrace;
    private Expression expression;

    private ArrayOperator(TokenNode open,  Expression exp, TokenNode close)
    {
        openBrace = open;
        expression = exp;
        closeBrace = close;
    }

    public String toString()
    {
        return openBrace + expression + closeBrace;
    }


}
