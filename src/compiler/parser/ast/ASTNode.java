package compiler.parser.ast;

import compiler.lexer.Token;
import compiler.lexer.TokenNode;
import compiler.lexer.Tokenizer;

import java.text.ParseException;

/**
 * Created by paul on 12/18/16.
 */
abstract class ASTNode {
    public static Tokenizer tokenizer;
    public TokenNode currToken;
    public TokenNode nextToken;

    abstract public void visit();

    abstract public void print();

    public void getNextToken()
    {
        currToken = nextToken;
        do {
            nextToken = tokenizer.getNextToken();
        }while (nextToken != null && nextToken.getT() != Token.EOF);
    }

    public void getNextToken(Token t)
    {
        getNextToken();
        if(currToken.getT() != t)
        {
            throw new Exception("Syntax Error: expected '" + t.)
        }
    }

    public void checkError(Token t){
        if(currToken.getT() != t){
            throw new "")
        }
    }

}
