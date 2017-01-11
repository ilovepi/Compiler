package compiler.parser.ast;

import compiler.lexer.ParserException;
import compiler.lexer.Token;

/**
 * Created by paul on 12/18/16.
 */
public class Factor extends ASTNode {
    Designator designator;
    Num number;
    ParenExpression parenExpression;
    FuncCall funcCall;

    Factor next;

    private Factor() {
        designator = null;
        number = null;
        parenExpression = null;
        funcCall = null;
        next = null;
    }


    public static Factor createFactor() throws ParserException {

        Factor f = new Factor();

        switch (nextToken.getT()) {
            case ARRAY:
            case VAR:
                f.designator = Designator.createDesignator();
                break;
            case CALL:
                f.funcCall = FuncCall.createFuncCall();
                break;
            case NUMBER:
                f.number = Num.createNum();
                break;
            case OPEN_PAREN:
                f.parenExpression = ParenExpression.createParenExpression();
                break;
        }

        if (f.hasChildren()) {
            if(currToken.getT() == Token.PLUS ||
                    currToken.getT() == Token.MINUS ) {

                f.next =
            }
            return f;
        }

        return null;
    }

    public boolean hasChildren() {
        return (getChild() != null);
    }

    public ASTNode getChild() {
        if (designator != null)
            return designator;

        if (number != null)
            return number;

        if (parenExpression != null)
            return parenExpression;

        if (funcCall != null)
            return funcCall;
        return null;
    }


    @Override
    public void visit() {

    }

    public String toString()
    {
        return "Factor: " + getChild();
    }

    @Override
    public void print() {
        System.out.println(toString());

    }
}
