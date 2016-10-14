package compiler.lexer;

/**
 * Created by Paul Kirth on 10/13/16.
 */




public enum  Token {

    //Arithmetic operators
    PLUS, MINUS, TIMES, DIVIDE,

    //Relational Operators: ==, !=, <, <= , >, >=
    EQUAL, NOT_EQUAL, LESS, LESS_EQ, GREATER, GREATER_EQ,


    //Misc operators "<-" , ";" , ","
    ASSIGN, SEMI_COLON, COMMA,

    //Blocks
    OPEN_PAREN, CLOSE_PAREN,        // "(" ")"
    OPEN_BRACKET, CLOSE_BRACKET,    // "[", "]"
    OPEN_CURL, CLOSE_CURL,          // "{", "}"


    //KEYWORDS
    LET, CALL, IF, THEN, ELSE, FI,WHILE, DO, OD, RETURN,
    MAIN,

    IDENTIFIER, NUMBER,

    // parsing tokens
    EOF,

    UNKNOWN;


    public static Token classifyToken(String str)
    {
        switch (str)
        {
            case "+":
                return PLUS;
            case "-":
                return MINUS;
            case "*":
                return TIMES;
            case "/":
                return DIVIDE;
            case "==":
                return EQUAL;
            case "!=":
                return NOT_EQUAL;
            case "<":
                return LESS;

            case "<=":
                return LESS_EQ;
            case ">":
                return GREATER;
            case "<=":
                return GREATER_EQ;
            case "/<-":
                return ASSIGN;
            case ";":
                return SEMI_COLON;
            case ",":
                return COMMA;
            case "(":
                return OPEN_PAREN;

            case ")":
                return CLOSE_PAREN;
            case "-":
                return MINUS;
            case "*":
                return TIMES;
            case "/":
                return DIVIDE;
            case "==":
                return EQUAL;
            case "!=":
                return NOT_EQUAL;
            case "<":
                return LESS;

            case "+":
                return PLUS;
            case "-":
                return MINUS;
            case "*":
                return TIMES;
            case "/":
                return DIVIDE;
            case "==":
                return EQUAL;
            case "!=":
                return NOT_EQUAL;
            case "<":
                return LESS;
        }

    }





}
