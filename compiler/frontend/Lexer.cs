using System;
using System.IO;

namespace compiler.frontend
{
    public class Lexer
    {
        StreamReader sr;// file reader
        public char c; // current char
        SymbolTable symbolTble; // symbol table
        int sym; // current token
        int val; // numberic value
        int id; // identifier

        public Lexer(string filename)
        {
            try
            {
                sr = new StreamReader(filename);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }

            symbolTble = new SymbolTable();
            next();
        }

        ~Lexer()
        {
            if (sr != null)
            {
                sr.Close();
                sr = null;
            }
        }

        public char next()
        {
            if (sr.Peek() == -1)
            {
                throw new Exception("Error: Lexer cannot read beyond the end of the file");
            }
            c = (char)sr.Read();
            return c;
        }


        public Token getNextToken()
        {
            findNextToken();

            if (char.IsDigit(c))
            {
                return number();
            }
            else if (char.IsLetter(c))
            {
                return symbol();
            }
            else if (!char.IsWhiteSpace(c))
            {                
                return punctuation();
            }          

            throw new Exception("Error: unable to parse next token");

        }

        public Token number()
        {

            string s = string.Empty;

            while (char.IsDigit(c))
            {
                s += c;
                next();
            }

            val = int.Parse(s);
            return Token.NUMBER;

        }

        public Token punctuation()
        {
            switch (c)
            {
                case '=':
                    next();
                    if (c != '=')
                    {
                        throw new Exception("Error: '=' is not a valid token, " +
                                            "must be one of: '==', '>=', '<=', '!='");
                    }
                    next();
                    return Token.EQUAL;

                case '!':
                    next();
                    if (c != '=')
                    {
                        throw new Exception("Error: '!' is not a valid token, " +
                                            "must be one of: '==', '>=', '<=', '!='");
                    }
                    next();
                    return Token.NOT_EQUAL;

                case '<':
                    next();
                    if (c == '=')
                    {
                        next();
                        return Token.LESS_EQ;
                    }
                    else if (c == '-')
                    {
                        next();
                        return Token.ASSIGN;
                    }
                    else
                    {
                        return Token.LESS;
                    }

                case '>':
                    next();
                    if (c == '=')
                    {
                        next();
                        return Token.GREATER_EQ;
                    }
                    else
                    {
                        return Token.GREATER;
                    }


                case '+':
                    next();
                    return Token.PLUS;
                case '-':
                    next();
                    return Token.MINUS;
                case '*':
                    next();
                    return Token.TIMES;
                case '/':
                    next();
                    if (c == '/')
                    {
                        while(c != '\n')
                        {
                            next();
                        }
                        return Token.COMMENT;
                    }
                    return Token.DIVIDE;


                case ',':
                    next();
                    return Token.COMMA;
                case ';':
                    next();
                    return Token.SEMI_COLON;
                case '.':                    
                    return Token.EOF;

                case '(':
                    next();
                    return Token.OPEN_PAREN;

                case ')':
                    next();
                    return Token.CLOSE_PAREN;


                case '[':
                    next();
                    return Token.OPEN_BRACKET;
                case ']':
                    next();
                    return Token.CLOSE_BRACKET;

                case '{':
                    next();
                    return Token.OPEN_CURL;
                case '}':
                    next();
                    return Token.CLOSE_CURL;

            
                default:
                    return Token.UNKNOWN;                    
            }
        }





        public Token symbol()
        {
            //Result ret = new Result();

            string s = string.Empty;
            s += c;
            next();

            while (char.IsLetterOrDigit(c))
            {
                s += c;
                next();
            }

            //ret.kind = (int)kind.variable;

            if (symbolTble.lookup(s))
            {
                id = symbolTble.val(s);
            }
            else
            {
                symbolTble.insert(s);
                id = symbolTble.val(s);
            }

            if(symbolTble.isId(s))
                return Token.IDENTIFIER;
            return (Token)id;
        }

        /// <summary>
        /// Finds the next token. Scans forward through whitespace.
        /// </summary>
        public void findNextToken()
        {
            while (char.IsWhiteSpace(c))
            {
                next();
            }
        }



    }
}