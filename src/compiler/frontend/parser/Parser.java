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
    //char in;
    int lineno;
    int pos;

    Tokenizer tokenizer;
    TokenNode tn;

    Parser() {
        is = null;
        lineno = 0;
        pos = 0;
        tokenizer = null;
        tn = null;

    }


    void parse(String filename) {
        tokenizer = new Tokenizer(filename);






    }


    void next() {

        tn = tokenizer.getNextToken();

       if(tn == null)
       {
           error();
       }
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
        if(t != tn.getT())
            error();
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

    void error() {
        System.out.println("Parse error at position " + pos + "of line" + lineno);
    }

    int varDecl() {
        //varDecl = typeDecl indent { “,” ident } “;” .
        int ret = 0;

        typeDecl();
        ident();
        while (tn.getT() == Token.COMMA) {
            next();
            ident();
        }

        checkToken(Token.SEMI_COLON);
        next();
        return ret;
    }

    void typeDecl() {
        //typeDecl = “var” | “array” “[“ number “]” { “[“ number “]” }
        if (tn.getT() == Token.VAR) {
            checkToken(Token.VAR);
        } else {
            checkToken(Token.ARRAY);
            do {
                checkToken("[");
                number();
                checkToken("]");
            } while (in == '[');
        }
    }

    int number() {
        StringBuilder sb = new StringBuilder();

        if (Character.isDigit(in)) {
            while (Character.isDigit(in)) {
                sb.append(in);
                next();
            }
        } else {
            error();
        }

        return Integer.parseInt(sb.toString());
    }

    void letter() {
        if (Character.isLowerCase(in)) {

            next();
        } else {
            error();
        }

    }

    String ident() {
        StringBuilder sb = new StringBuilder();
        if (Character.isLowerCase(in)) {
            sb.append(in);
            next();
        } else {
            error();
        }

        while (Character.isLowerCase(in) || Character.isDigit(in)) {
            sb.append(in);
            next();
        }

        return sb.toString();
    }


    int funcDecl() {
        //funcDecl = (“function” | “procedure”) ident [formalParam] “;” funcBody “;” .

        if (in == 'f') {
            checkToken("function");
        } else if (in == 'p') {
            checkToken("procedure");
        } else {
            return -1;
        }

        ident();
        formalParam();
        checkToken(";");
        funcBody();
        checkToken(";");

        return 0;
    }

    void funcBody() {

    }

    void statSequence() {
        //statSequence = statement { “;” statement }.
        statement();
        while (in == ';') {
            next();
            statement();
        }
    }

    void statement() {
        //statement = assignment | funcCall | ifStatement | whileStatement | returnStatement.
        switch (in) {
            case 'l':
                assignment();
                break;
            case 'c':
                funcCall();
                break;
            case 'i':
                ifStmt();
                break;
            case 'w':
                whileStmt();
                break;
            case 'r':
                returnStmt();
                break;
        }
    }

    void relOp() {
        switch (in) {
            case '=':
            case '!':
            case '<':
            case '>':
                next();
                if (in == '=')
                    next();
                break;

            default:
                error();
        }
    }

    void designator() {
        ident();
        while (in == '[') {
            next();
            expr();
            checkToken("]");
        }
    }

    void expr() {
        term();
        while (in == '+' || in == '-') {
            next();
            term();
        }
    }

    void term() {
        factor();
        while (in == '*' || in == '/') {
            next();
            factor();
        }

    }

    void factor() {


            if(in == '(') {
                next();
                expr();
                checkToken(")");
            }

            if(Character.isDigit(in))
            {
                number();

            }

            if(in == 'c') {
                next();
                if (in == 'a'){
                    next();
                    if(in == 'l'){
                        next();
                        if(in == 'l')
                        {
                            next();
                            if(Character.isWhitespace(in))
                            {
                                next();


                            }
                        }
                    }
                }
            }

    }

    void relation() {
        expr();
        relOp();
        expr();
    }

    void assignment() {
        checkToken("let");
        designator();
        checkToken("<-");
        expr();
    }

    void funcCall() {
        checkToken("call");
        ident();
        if(in == '(')
        {
            next();
            if(in != ')')
            {
                expr();
                while(in == ',')
                {
                    expr();
                }
            }
            checkToken(")");
        }
    }

    void ifStmt() {
        checkToken("if");
        relation();
        checkToken("then");
        statSequence();
        if(in == 'e')
        {
            checkToken("else");
            statSequence();
        }
        checkToken("fi");
    }

    void whileStmt() {
        checkToken("while");
        relation();
        checkToken("do");
        statSequence();
        checkToken("od");
    }

    void returnStmt() {
        checkToken("return");

        expr();
    }


    void formalParam() {
        if (in == ';')
            return;
        if (in == '(') {
            next();

            if (in == ')') {
                next();
                return;
            }

            ident();
            while (in == ',') {
                next();
                ident();
            }

            checkToken(")");
        }
    }

}
