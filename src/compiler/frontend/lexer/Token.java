package compiler.frontend.lexer;

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
    MAIN, VAR,

    IDENTIFIER, NUMBER, FUNCTION, PROCEDURE,

    // parsing tokens
    EOF,
    COMMENT,

    UNKNOWN;


    /**
     * Classify Tokens based on the Grammar
     * TODO: consider using a hash table instead of enum + function... could be easier to maintian ???
     * @param str The token(string) to classify
     * @return The correct token of the string
     */

    public static Token classifyToken(String str)
    {
        // Choose the  correct Token Value
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

            // Relational Operators
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
            case ">=":
                return GREATER_EQ;

            // Misc operators
            case "<-":
                return ASSIGN;
            case ";":
                return SEMI_COLON;
            case ",":
                return COMMA;

            // Blocks
            case "(":
                return OPEN_PAREN;
            case ")":
                return CLOSE_PAREN;
            case "[":
                return OPEN_BRACKET;
            case "]":
                return CLOSE_BRACKET;
            case "{":
                return OPEN_CURL;
            case "}":
                return CLOSE_CURL;

            //keywords
            case "let":
                return LET;
            case "call":
                return CALL;
            case "var":
                return VAR;

            case "if":
                return IF;
            case "then":
                return THEN;
            case "else":
                return ELSE;
            case "fi":
                return FI;
            case "while":
                return WHILE;
            case "do":
                return DO;
            case "od":
                return OD;

            case "return":
                return RETURN;

            case "main":
                return MAIN;
            case "function":
                return FUNCTION;
            case "procedure":
                return PROCEDURE;
            case ".":
                return EOF;

            default:
                if(str.matches("[0-9]+"))
                {
                    return NUMBER;
                }
                else if (str.matches("[A-Za-z][A-Za-z0-9]*"))
                {
                    return IDENTIFIER;
                }
                else if (str.matches("//.*")) {
                    return COMMENT;
                }
                else
                {
                    return UNKNOWN;
                }
        }

    }

    /**
     * A to String function for printing Token values
     * @param t The token to read
     * @return String representation of the token
     */
    public static String toString(Token t)
    {
        // choose the correct string based on the Token
        switch (t)
        {
            case PLUS:
                return "+";
            case MINUS:
                return "-";
            case TIMES:
                return "*";
            case DIVIDE:
                return "/";

            // Relational Operators
            case EQUAL:
                return "==";
            case NOT_EQUAL:
                return "!=";
            case LESS:
                return "<";
            case LESS_EQ:
                return "<=";
            case GREATER:
                return ">";
            case GREATER_EQ:
                return ">=";

            // Misc operators
            case ASSIGN:
                return "<-";
            case SEMI_COLON:
                return ";";
            case COMMA:
                return ",";

            // Blocks
            case OPEN_PAREN:
                return "(";
            case CLOSE_PAREN:
                return ")";
            case OPEN_BRACKET:
                return "[";
            case CLOSE_BRACKET:
                return "]";
            case OPEN_CURL:
                return "{";
            case CLOSE_CURL:
                return "}";

            //keywords
            case LET:
                return "let";
            case CALL:
                return "call";

            case IF:
                return "if";
            case THEN:
                return "then";
            case ELSE:
                return "else";
            case FI:
                return "fi";
            case WHILE:
                return "while";
            case DO:
                return "do";
            case OD:
                return "od";

            case RETURN:
                return "return";

            case MAIN:
                return "main";
            case FUNCTION:
                return "function";
            case PROCEDURE:
                return "procedure";
            case EOF:
                return "EOF";
            case NUMBER:
                return "number";
            case IDENTIFIER:
                return "identifier";
            case UNKNOWN:
                return "unknown";
            case COMMENT:
                return "comment";
            default:
                return "ERROR";
        }

    }

    /**
     * A to String function for printing Token values
     * @param t The token to read
     * @return String representation of the token
     */
    public static String printToken(Token t)
    {
        //
        switch (t)
        {
            case PLUS:
                return "PLUS";
            case MINUS:
                return "MINUS";
            case TIMES:
                return "TIMES";
            case DIVIDE:
                return "DIVIDE";

            // Relational Operators
            case EQUAL:
                return "EQUAL";
            case NOT_EQUAL:
                return "NOT_EQUAL";
            case LESS:
                return "LESS";
            case LESS_EQ:
                return "LESS_EQ";
            case GREATER:
                return "GREATER";
            case GREATER_EQ:
                return "GREATER_EQ";

            // Misc operators
            case ASSIGN:
                return "ASSIGN";
            case SEMI_COLON:
                return "SEMI_COLON";
            case COMMA:
                return "COMMA";

            // Blocks
            case OPEN_PAREN:
                return "OPEN_PAREN";
            case CLOSE_PAREN:
                return "CLOSE_PAREN";
            case OPEN_BRACKET:
                return "OPEN_BRACKET";
            case CLOSE_BRACKET:
                return "CLOSE_BRACKET";
            case OPEN_CURL:
                return "OPEN_CURL";
            case CLOSE_CURL:
                return "CLOSE_CURL";

            //keywords
            case LET:
                return "LET";
            case CALL:
                return "CALL";
            case VAR:
                return "VAR";

            case IF:
                return "IF";
            case THEN:
                return "THEN";
            case ELSE:
                return "ELSE";
            case FI:
                return "FI";
            case WHILE:
                return "WHILE";
            case DO:
                return "DO";
            case OD:
                return "OD";

            case RETURN:
                return "RETURN";

            case MAIN:
                return "MAIN";
            case FUNCTION:
                return "FUNCTION";
            case PROCEDURE:
                return "PROCEDURE";
            case EOF:
                return "EOF";
            case NUMBER:
                return "NUMBER";
            case IDENTIFIER:
               return "IDENTIFIER";
            case UNKNOWN:
                return "UNKNOWN";
            case COMMENT:
                return "COMMENT";
            default:
                return "ERROR";
        }

    }


}
