package compiler.parser.ast;

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

    public static ArrayOperator createArrayOperator(){

        // do stuff to make an array operator node
        // read open brace
        che

        if(super.currToken.getT() != Token.OPEN_BRACKET)


        //read expression

        //read close brace

        // create an ArrayOperator node

        // link it to other array operator nodes

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
