package compiler.frontend.parser;

import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.IOException;

/**
 * Created by paul on 1/10/17.
 */
public class Parser {
    FileInputStream is;
    char in;
    int lineno;
    int pos;

    Parser() {
        is = null;
        lineno = 0;
        pos = 0;

    }


    void parse(String filename) {
        try {
            is = new FileInputStream(filename);
            next();
            comp();

        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } finally {
            if (is != null) {
                try {
                    is.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
        }


    }


    void next() {

        try {
            int res = is.read();
            if (res != -1)
                in = (char) res;
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    void comp() {
        //“main” { varDecl } { funcDecl } “{” statSequence “}” “.” .
        find_word("main");
        int ret = 0;

        while (ret != -1) {
            ret = varDecl();
        }

        ret = 0;

        while (ret != -1) {
            ret = funcDecl();
        }

        find_word("{");
        statSequence();
        find_word("}.");
    }


    void find_word(String word) {
        for (char c : word.toCharArray()) {
            if (in == c) {
                next();
            } else {
                error();
            }
        }
    }

    void error() {
        System.out.println("Parse error at position " + pos + "of line" + lineno);
    }

    int varDecl() {
        //varDecl = typeDecl indent { “,” ident } “;” .
        int ret = 0;

        typeDecl();
        ident();
        while (in == ',') {
            ident();
        }

        find_word(";");
        return ret;
    }

    void typeDecl() {
        //typeDecl = “var” | “array” “[“ number “]” { “[“ number “]” }
        if (in == 'v') {
            find_word("var");
        } else {
            find_word("array");
            do {
                find_word("[");
                number();
                find_word("]");
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
            find_word("function");
        } else if (in == 'p') {
            find_word("procedure");
        } else {
            return -1;
        }

        ident();
        formalParam();
        find_word(";");
        funcBody();
        find_word(";");

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
            find_word("]");
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
                find_word(")");
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
        find_word("let");
        designator();
        find_word("<-");
        expr();
    }

    void funcCall() {
        find_word("call");
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
            find_word(")");
        }
    }

    void ifStmt() {
        find_word("if");
        relation();
        find_word("then");
        statSequence();
        if(in == 'e')
        {
            find_word("else");
            statSequence();
        }
        find_word("fi");
    }

    void whileStmt() {
        find_word("while");
        relation();
        find_word("do");
        statSequence();
        find_word("od");
    }

    void returnStmt() {
        find_word("return");

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

            find_word(")");
        }
    }

}
