package compiler.parser.ast;

import compiler.lexer.Token;
import compiler.lexer.TokenNode;
import compiler.lexer.Tokenizer;

/**
 * Created by paul on 12/18/16.
 */
public class ASTNode {
    public static Tokenizer tokenizer;
    TokenNode currToken;
    TokenNode nextToken;

    public void visit(){}

    public void print(){}

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

}
