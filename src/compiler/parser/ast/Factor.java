package compiler.parser.ast;

/**
 * Created by paul on 12/18/16.
 */
public class Factor extends ASTNode{
    Designator designator;
    Num number;
    ParenExpression parenExpression;
    FuncCall funcCall;


    public static Factor createFactor(){
        return null;
    }


    @Override
    public void visit() {

    }

    @Override
    public void print() {

    }
}
