package compiler.parser;

import compiler.lexer.Token;
import compiler.lexer.TokenNode;
import compiler.lexer.Tokenizer;

import static compiler.lexer.Token.*;
import static compiler.lexer.Token.MAIN;
import static compiler.parser.ProgramState.*;

/**
 * Created by paul on 12/14/16.
 */
public class Parser {



    public Tokenizer tokenizer;
    public AST ast;

    public ProgramState state;


    public  Parser()
    {
        tokenizer = null;
        ast = null;

    }


    public void parse()
    {
        AST ast = new AST();

        TokenNode tn;
        do{
            tn = tokenizer.getNextToken();

            processToken(tn);





        } while ( tn.getT() != Token.EOF);
    }


    public boolean processToken(TokenNode tn)
    {
        boolean ret = false;
        switch(state)
        {
            case program:
                Token t = tn.getT();
                if(t == COMMENT)
                {
                    return true;
                }
                else if(t == MAIN)
                {
                    state = computation;

                }
        }




        return ret;
    }
}
