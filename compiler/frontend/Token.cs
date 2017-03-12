#region Basic header

// MIT License
// 
// Copyright (c) 2016 Paul Kirth
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

namespace compiler.frontend
{
    public enum Token
    {
        UNKNOWN,
        //Arithmetic operators
        PLUS,
        MINUS,
        TIMES,
        DIVIDE,

        //Relational Operators: ==, !=, <, <= , >, >=
        EQUAL,
        NOT_EQUAL,
        LESS,
        LESS_EQ,
        GREATER,
        GREATER_EQ,


        //Misc operators "<-" , ";" , ","
        ASSIGN,
        SEMI_COLON,
        COMMA,

        //Blocks
        OPEN_PAREN,
        CLOSE_PAREN, // "(" ")"
        OPEN_BRACKET,
        CLOSE_BRACKET, // "[", "]"
        OPEN_CURL,
        CLOSE_CURL, // "{", "}"


        //KEYWORDS
        LET,
        CALL,
        IF,
        THEN,
        ELSE,
        FI,
        WHILE,
        DO,
        OD,
        RETURN,
        MAIN,
        VAR,
        ARRAY,

        FUNCTION,
        PROCEDURE,

        // parsing tokens
        EOF,
        COMMENT,


        NUMBER,
        IDENTIFIER
    }

    public class TokenHelper
    {
        /// <summary>
        ///     A to String function for printing Token values
        /// </summary>
        /// <param name="t">The token to read</param>
        /// <returns>String representation of the token</returns>
        public static string ToString(Token t)
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
                case Token.VAR:
                    return "var";
                case Token.ARRAY:
                    return "array";
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


        /// <summary>
        ///     A to String function for printing Token values
        /// </summary>
        /// <param name="t">The token to read</param>
        /// <returns>String representation of the token</returns>
        public static string PrintToken(Token t)
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
                case Token.VAR:
                    return "VAR";
                case Token.ARRAY:
                    return "ARRAY";
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