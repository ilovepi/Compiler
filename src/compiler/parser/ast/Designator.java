package compiler.parser.ast;

import compiler.lexer.ParserException;
import compiler.lexer.Token;

import static compiler.parser.ast.Identifier.createIdentifier;

/**
 * Created by paul on 12/18/16.
 */
public class Designator extends ASTNode{
    Identifier ident;
    ArrayOperator arrayOperator;


    public Designator(Identifier id)
    {
        ident = id;
        arrayOperator = null;
    }

     public Designator(Identifier id, ArrayOperator aryOp)
    {
        ident = id;
        arrayOperator = aryOp;
    }

    public static Designator createDesignator() throws ParserException {
        getNextToken();
        checkError(Token.IDENTIFIER);

        Identifier id = createIdentifier(currToken);

        Designator des = new Designator(id);
        des.arrayOperator = ArrayOperator.createArrayOperator();

        return des;
    }


    @Override
    public void visit() {

    }

    @Override
    public void print() {

    }
}
