using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.frontend
{
    enum Token
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

    class TokenHelper
    {

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
                case Token.PLUS:
                    return "+";
                case Token.MINUS:
                    return "-";
                case Token.TIMES:
                    return "*";
                case Token.DIVIDE:
                    return "/";

                // Relational Operators
                case Token.EQUAL:
                    return "==";
                case Token.NOT_EQUAL:
                    return "!=";
                case Token.LESS:
                    return "<";
                case Token.LESS_EQ:
                    return "<=";
                case Token.GREATER:
                    return ">";
                case Token.GREATER_EQ:
                    return ">=";

                // Misc operators
                case Token.ASSIGN:
                    return "<-";
                case Token.SEMI_COLON:
                    return ";";
                case Token.COMMA:
                    return ",";

                // Blocks
                case Token.OPEN_PAREN:
                    return "(";
                case Token.CLOSE_PAREN:
                    return ")";
                case Token.OPEN_BRACKET:
                    return "[";
                case Token.CLOSE_BRACKET:
                    return "]";
                case Token.OPEN_CURL:
                    return "{";
                case Token.CLOSE_CURL:
                    return "}";

                //keywords
                case Token.LET:
                    return "let";
                case Token.CALL:
                    return "call";

                case Token.IF:
                    return "if";
                case Token.THEN:
                    return "then";
                case Token.ELSE:
                    return "else";
                case Token.FI:
                    return "fi";
                case Token.WHILE:
                    return "while";
                case Token.DO:
                    return "do";
                case Token.OD:
                    return "od";

                case Token.RETURN:
                    return "return";

                case Token.MAIN:
                    return "main";
                case Token.FUNCTION:
                    return "function";
                case Token.PROCEDURE:
                    return "procedure";
                case Token.EOF:
                    return "EOF";
                case Token.NUMBER:
                    return "number";
                case Token.IDENTIFIER:
                    return "identifier";
                case Token.UNKNOWN:
                    return "unknown";
                case Token.COMMENT:
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
                case Token.PLUS:
                    return "PLUS";
                case Token.MINUS:
                    return "MINUS";
                case Token.TIMES:
                    return "TIMES";
                case Token.DIVIDE:
                    return "DIVIDE";

                // Relational Operators
                case Token.EQUAL:
                    return "EQUAL";
                case Token.NOT_EQUAL:
                    return "NOT_EQUAL";
                case Token.LESS:
                    return "LESS";
                case Token.LESS_EQ:
                    return "LESS_EQ";
                case Token.GREATER:
                    return "GREATER";
                case Token.GREATER_EQ:
                    return "GREATER_EQ";

                // Misc operators
                case Token.ASSIGN:
                    return "ASSIGN";
                case Token.SEMI_COLON:
                    return "SEMI_COLON";
                case Token.COMMA:
                    return "COMMA";

                // Blocks
                case Token.OPEN_PAREN:
                    return "OPEN_PAREN";
                case Token.CLOSE_PAREN:
                    return "CLOSE_PAREN";
                case Token.OPEN_BRACKET:
                    return "OPEN_BRACKET";
                case Token.CLOSE_BRACKET:
                    return "CLOSE_BRACKET";
                case Token.OPEN_CURL:
                    return "OPEN_CURL";
                case Token.CLOSE_CURL:
                    return "CLOSE_CURL";

                //keywords
                case Token.LET:
                    return "LET";
                case Token.CALL:
                    return "CALL";
                case Token.VAR:
                    return "VAR";

                case Token.IF:
                    return "IF";
                case Token.THEN:
                    return "THEN";
                case Token.ELSE:
                    return "ELSE";
                case Token.FI:
                    return "FI";
                case Token.WHILE:
                    return "WHILE";
                case Token.DO:
                    return "DO";
                case Token.OD:
                    return "OD";

                case Token.RETURN:
                    return "RETURN";

                case Token.MAIN:
                    return "MAIN";
                case Token.FUNCTION:
                    return "FUNCTION";
                case Token.PROCEDURE:
                    return "PROCEDURE";
                case Token.EOF:
                    return "EOF";
                case Token.NUMBER:
                    return "NUMBER";
                case Token.IDENTIFIER:
                    return "IDENTIFIER";
                case Token.UNKNOWN:
                    return "UNKNOWN";
                case Token.COMMENT:
                    return "COMMENT";
                default:
                    return "ERROR";
            }

        }

    }




}
