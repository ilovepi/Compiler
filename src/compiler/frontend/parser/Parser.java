package compiler.frontend.parser;


import compiler.frontend.lexer.Token;
import compiler.frontend.lexer.TokenNode;
import compiler.frontend.lexer.Tokenizer;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;

/**
 * Created by paul on 1/10/17.
 */
public class Parser {
    FileInputStream is;

    Tokenizer tokenizer;
    TokenNode tn;

    public Parser() {
        is = null;
        tokenizer = null;
        tn = null;

    }


    public void parse(String filename) {
        tokenizer = new Tokenizer(filename);
        next();
        comp();
    }


    void next() {

        do {
        tn = tokenizer.getNextToken();
         if (tn == null) {
            error();
        }
      }while(tn.getT() == Token.COMMENT);


    }

    void comp() {
        //“main” { varDecl } { funcDecl } “{” statSequence “}” “.” .
        checkToken(Token.MAIN);
        int ret = 0;

        while (ret != -1) {
            ret = varDecl();
        }

        ret = 0;

        while (ret != -1) {
            ret = funcDecl();
        }

        checkToken(Token.OPEN_CURL);
        statSequence();
        checkToken(Token.CLOSE_CURL);
    }

    void checkToken(Token t) {
        if (t != tn.getT())
            error();
      do {
          next();
      }while(tn.getT() == Token.COMMENT);
    }
    /*
    void find_word(String word) {
        for (char c : word.toCharArray()) {
            if (in == c) {
                next();
            } else {
                error();
            }
        }
    }
    */

    public void error() {
        System.out.println("Parse error at position " + tokenizer.getPos()  + " of line " + tokenizer.getLineno());
        System.exit(-1);
    }

    int varDecl() {
        //varDecl = typeDecl indent { “,” ident } “;” .
        int ret = 0;

        ret = typeDecl();
        if(ret == -1)
            return ret;

        ident();
        while (tn.getT() == Token.COMMA) {
            next();
            ident();
        }

        checkToken(Token.SEMI_COLON);
        return 0;
    }

    int typeDecl() {
        //typeDecl = “var” | “array” “[“ number “]” { “[“ number “]” }
        if (tn.getT() == Token.VAR) {
            next();
            return 0;
        } else if(tn.getT() == Token.ARRAY){
            next();
            do {
                checkToken(Token.OPEN_BRACKET);
                number();
                checkToken(Token.CLOSE_BRACKET);
            } while (tn.getT() == Token.OPEN_BRACKET);

            return 0;
        }

        return -1;
    }

    int number() {

        int ret = 0;
        if (tn.getT() == Token.NUMBER) {
            ret = tn.getVal();
            next();
            return ret;
        } else {
            error();
        }

        //unreachable either will return a value or error
        return ret;
    }


    String ident() {

        String ret = "";
        if (tn.getT() == Token.IDENTIFIER) {
            ret = tn.getS();
            next();
            return ret;
        } else {
            error();
        }

        //unreachable either will return a value or error
        return ret;
    }


    int funcDecl() {
        //funcDecl = (“function” | “procedure”) ident [formalParam] “;” funcBody “;” .

        if (tn.getT() == Token.FUNCTION) {
            next();
        } else if (tn.getT() == Token.PROCEDURE) {
            next();
        } else {
            return -1;
        }

        ident();
        formalParam();
        checkToken(Token.SEMI_COLON);
        funcBody();
        checkToken(Token.SEMI_COLON);

        return 0;
    }

    void funcBody() {
        //funcBody = { varDecl } “{” [ statSequence ] “}”.

        int ret = 0;
        do {
            ret = varDecl();
        } while (ret != -1);

        checkToken(Token.OPEN_CURL);
        if (tn.getT() != Token.CLOSE_CURL) {
            statSequence();
        }
        checkToken(Token.CLOSE_CURL);
    }

    void statSequence() {
        //statSequence = statement { “;” statement }.
        statement();
        while (tn.getT() == Token.SEMI_COLON) {
            next();
            statement();
        }
    }

    void statement() {
        //statement = assignment | funcCall | ifStatement | whileStatement | returnStatement.
        Token t = tn.getT();
        switch (t) {
            case LET:
                assignment();
                break;
            case CALL:
                funcCall();
                break;
            case IF:
                ifStmt();
                break;
            case WHILE:
                whileStmt();
                break;
            case RETURN:
                returnStmt();
                break;

            // may nee to remove the error statement
            default:
                error();
        }
    }

    void relOp() {
        Token t = tn.getT();
        switch (t) {
            case EQUAL:
            case NOT_EQUAL:
            case GREATER:
            case GREATER_EQ:
            case LESS:
            case LESS_EQ:
                next();


            default:
                error();
        }
    }

    void designator() {
        ident();
        while (tn.getT() == Token.OPEN_BRACKET) {
            next();
            expr();
            checkToken(Token.CLOSE_BRACKET);
        }
    }

    void expr() {
        term();
        while (tn.getT() == Token.PLUS || tn.getT() == Token.MINUS) {
            next();
            term();
        }
    }

    void term() {
        factor();
        while (tn.getT() == Token.TIMES || tn.getT() == Token.DIVIDE) {
            next();
            factor();
        }

    }

    void factor() {

        Token t = tn.getT();
        switch (t) {
            case OPEN_PAREN:
                next();
                expr();
                checkToken(Token.CLOSE_PAREN);
                next();
                break;
            case NUMBER:
                next();
                break;
            case IDENTIFIER:
                designator();
                break;
            case CALL:
                funcCall();
                break;
            default:
                error();
        }
    }

    void relation() {
        expr();
        relOp();
        expr();
    }

    void assignment() {
        checkToken(Token.LET);
        designator();
        checkToken(Token.ASSIGN);
        expr();
    }

    void funcCall() {
        checkToken(Token.CALL);
        ident();
        if (tn.getT() == Token.OPEN_PAREN) {
            next();
            if (tn.getT() != Token.CLOSE_PAREN) {
                expr();
                while (tn.getT() == Token.COMMA) {
                    expr();
                }
            }
            checkToken(Token.CLOSE_PAREN);
        }
    }

    void ifStmt() {
        checkToken(Token.IF);
        relation();
        checkToken(Token.THEN);
        statSequence();
        if (tn.getT() == Token.ELSE) {
            next();
            statSequence();
        }
        checkToken(Token.FI);
    }

    void whileStmt() {
        checkToken(Token.WHILE);
        relation();
        checkToken(Token.DO);
        statSequence();
        checkToken(Token.OD);
    }

    void returnStmt() {
        checkToken(Token.RETURN);

        expr();
    }


    void formalParam() {
        if (tn.getT() == Token.SEMI_COLON)
            return;
        if (tn.getT() == Token.OPEN_PAREN) {
            next();

            if (tn.getT() == Token.CLOSE_PAREN) {
                next();
                return;
            }

            ident();
            while (tn.getT() == Token.COMMA) {
                next();
                ident();
            }

            checkToken(Token.CLOSE_PAREN);
        }
    }

}
