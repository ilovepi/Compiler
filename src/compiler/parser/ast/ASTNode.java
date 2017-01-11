package compiler.parser.ast;

import compiler.lexer.ParserException;
import compiler.lexer.Token;
import compiler.lexer.TokenNode;
import compiler.lexer.Tokenizer;

import java.text.ParseException;

/**
 * Created by paul on 12/18/16.
 */
abstract class ASTNode {
    public static Tokenizer tokenizer;
    public static TokenNode currToken;
    public static TokenNode nextToken;

    abstract public void visit();

    abstract public void print();

    public static void getNextToken()
    {
        currToken = nextToken;
        do {
            nextToken = tokenizer.getNextToken();
        }while (nextToken != null && nextToken.getT() != Token.EOF);
    }


    public static void getNextToken(Token t) throws ParserException {
        getNextToken();
        checkError(t);
    }

    public static void checkError(Token t) throws ParserException {
        if(currToken.getT() != t){
            throw new ParserException(currToken, t);
        }
    }

}
