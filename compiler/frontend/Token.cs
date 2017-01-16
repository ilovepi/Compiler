using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.frontend
{
    enum TokenType
    {
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
        LET, CALL, IF, THEN, ELSE, FI, WHILE, DO, OD, RETURN,
        MAIN, VAR, ARRAY,

        IDENTIFIER, NUMBER, FUNCTION, PROCEDURE,

        // parsing tokens
        EOF,
        COMMENT,

        UNKNOWN

    }

    class Token
    {

        /**
        * A to String function for printing Token values
        * @param t The token to read
        * @return String representation of the token
        */
        public static String toString(TokenType t)
        {
            // choose the correct string based on the Token
            switch (t)
            {
                case TokenType.PLUS:
                    return "+";
                case TokenType.MINUS:
                    return "-";
                case TokenType.TIMES:
                    return "*";
                case TokenType.DIVIDE:
                    return "/";

                // Relational Operators
                case TokenType.EQUAL:
                    return "==";
                case TokenType.NOT_EQUAL:
                    return "!=";
                case TokenType.LESS:
                    return "<";
                case TokenType.LESS_EQ:
                    return "<=";
                case TokenType.GREATER:
                    return ">";
                case TokenType.GREATER_EQ:
                    return ">=";

                // Misc operators
                case TokenType.ASSIGN:
                    return "<-";
                case TokenType.SEMI_COLON:
                    return ";";
                case TokenType.COMMA:
                    return ",";

                // Blocks
                case TokenType.OPEN_PAREN:
                    return "(";
                case TokenType.CLOSE_PAREN:
                    return ")";
                case TokenType.OPEN_BRACKET:
                    return "[";
                case TokenType.CLOSE_BRACKET:
                    return "]";
                case TokenType.OPEN_CURL:
                    return "{";
                case TokenType.CLOSE_CURL:
                    return "}";

                //keywords
                case TokenType.LET:
                    return "let";
                case TokenType.CALL:
                    return "call";

                case TokenType.IF:
                    return "if";
                case TokenType.THEN:
                    return "then";
                case TokenType.ELSE:
                    return "else";
                case TokenType.FI:
                    return "fi";
                case TokenType.WHILE:
                    return "while";
                case TokenType.DO:
                    return "do";
                case TokenType.OD:
                    return "od";

                case TokenType.RETURN:
                    return "return";

                case TokenType.MAIN:
                    return "main";
                case TokenType.FUNCTION:
                    return "function";
                case TokenType.PROCEDURE:
                    return "procedure";
                case TokenType.EOF:
                    return "EOF";
                case TokenType.NUMBER:
                    return "number";
                case TokenType.IDENTIFIER:
                    return "identifier";
                case TokenType.UNKNOWN:
                    return "unknown";
                case TokenType.COMMENT:
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
        public static String printToken(TokenType t)
        {
            //
            switch (t)
            {
                case TokenType.PLUS:
                    return "PLUS";
                case TokenType.MINUS:
                    return "MINUS";
                case TokenType.TIMES:
                    return "TIMES";
                case TokenType.DIVIDE:
                    return "DIVIDE";

                // Relational Operators
                case TokenType.EQUAL:
                    return "EQUAL";
                case TokenType.NOT_EQUAL:
                    return "NOT_EQUAL";
                case TokenType.LESS:
                    return "LESS";
                case TokenType.LESS_EQ:
                    return "LESS_EQ";
                case TokenType.GREATER:
                    return "GREATER";
                case TokenType.GREATER_EQ:
                    return "GREATER_EQ";

                // Misc operators
                case TokenType.ASSIGN:
                    return "ASSIGN";
                case TokenType.SEMI_COLON:
                    return "SEMI_COLON";
                case TokenType.COMMA:
                    return "COMMA";

                // Blocks
                case TokenType.OPEN_PAREN:
                    return "OPEN_PAREN";
                case TokenType.CLOSE_PAREN:
                    return "CLOSE_PAREN";
                case TokenType.OPEN_BRACKET:
                    return "OPEN_BRACKET";
                case TokenType.CLOSE_BRACKET:
                    return "CLOSE_BRACKET";
                case TokenType.OPEN_CURL:
                    return "OPEN_CURL";
                case TokenType.CLOSE_CURL:
                    return "CLOSE_CURL";

                //keywords
                case TokenType.LET:
                    return "LET";
                case TokenType.CALL:
                    return "CALL";
                case TokenType.VAR:
                    return "VAR";

                case TokenType.IF:
                    return "IF";
                case TokenType.THEN:
                    return "THEN";
                case TokenType.ELSE:
                    return "ELSE";
                case TokenType.FI:
                    return "FI";
                case TokenType.WHILE:
                    return "WHILE";
                case TokenType.DO:
                    return "DO";
                case TokenType.OD:
                    return "OD";

                case TokenType.RETURN:
                    return "RETURN";

                case TokenType.MAIN:
                    return "MAIN";
                case TokenType.FUNCTION:
                    return "FUNCTION";
                case TokenType.PROCEDURE:
                    return "PROCEDURE";
                case TokenType.EOF:
                    return "EOF";
                case TokenType.NUMBER:
                    return "NUMBER";
                case TokenType.IDENTIFIER:
                    return "IDENTIFIER";
                case TokenType.UNKNOWN:
                    return "UNKNOWN";
                case TokenType.COMMENT:
                    return "COMMENT";
                default:
                    return "ERROR";
            }

        }

    }




}
