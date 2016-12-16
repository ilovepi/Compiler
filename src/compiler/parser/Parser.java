package compiler.parser;

import compiler.lexer.Token;
import compiler.lexer.TokenNode;
import compiler.lexer.Tokenizer;
import compiler.parser.ast.AST;
import compiler.parser.ast.ASTNode;
import compiler.parser.ast.VarDeclNode;

import static compiler.lexer.Token.*;
import static compiler.parser.ProgramState.*;

/**
 * Created by paul on 12/14/16.
 */
public class Parser {


    public Tokenizer tokenizer;
    public AST ast;

    public ProgramState state;

    TokenNode current_token;
    TokenNode next_token;


    public Parser() {
        tokenizer = null;
        ast = null;

    }


    public void parse() {
        AST ast = new AST();

        TokenNode tn;

    }


    public void processToken(TokenNode tn) throws Exception {
        // boolean run = true;
        // while(run)

        Token t = tn.getT();
        switch (state) {
            case program:
                if (t == COMMENT) {
                    return;
                } else if (t == MAIN) {
                    state = computation;
                    return;
                } else {
                    throw new Exception("Parse error: Expected to find 'main'");
                }
            case computation:


                break;
            case varDecl:
                break;
            case funDecl:
                break;
            case typeDecl:
                break;
            case statSeq:
                break;
            case statement:
                break;
            case assignment:
                break;
            case funcCall:
                break;
            case ifStatement:
                break;
            case whileStatement:
                break;
            case returnStatement:
                break;
            case formalParams:
                break;
            case funcBody:
                break;
            case designator:
                break;
            case factor:
                break;
            case term:
                break;
            case expression:
                break;
            case relation:
                break;
            default:
                throw new Exception("Unknown Parse error");

        }
        return;
    }




    private boolean varDecl(Token t) {
        if (t == Token.VAR) {
            createDeclNode()

        } else if (t == Token.ARRAY) {

        }

        return false;
    }

    public void processMain() throws Exception {
        while(current_token.getT() == COMMENT)
        {
            getNextToken();
        }

        if(current_token.getT() != MAIN)
            throw new Exception("Parse error: Expected to find 'main'");


        processVarDecl()


    }

    void processVarDecl()
    {
        Token t;
        do {
            getNextToken();
            t = current_token.getT();
            if (t == VAR) {
               VarDeclNode var = new VarDeclNode(current_token);


            } else if (t == ARRAY) {

            }
        }while(t == VAR || t == ARRAY);

    }

    public void getNextToken() {
        current_token = next_token;
        next_token = tokenizer.getNextToken();
    }

    public TokenNode peekNextToken() {
        return next_token;
    }

    ASTNode createDeclNode()
    {

    }


}
