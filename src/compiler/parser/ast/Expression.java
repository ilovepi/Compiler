package compiler.parser.ast;

/**
 * Created by paul on 12/18/16.
 */
public class Expression implements ASTNode {
    Term term;
    AddSub operator;
    Expression nextExpression;


    public String toString()
    {
        String str = term + operator;
        if(nextExpression != null)
            str += nextExpression;

        return str;
    }

    public static Expression createExpression()
    {
        return null;
    }

}
