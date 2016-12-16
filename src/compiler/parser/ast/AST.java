package compiler.parser.ast;
import compiler.lexer.*;


/**
 * Created by paul on 12/16/16.
 */
public class AST {



    private Tokenizer tonkenizer;

    AST computation()
    {
        TokenNode tn;
        do{
            tn = tonkenizer.getNextToken();
        }while (tn.getT() == Token.COMMENT);

        if(tn.getT() != Token.MAIN)

    }



    AST createAST()



}
