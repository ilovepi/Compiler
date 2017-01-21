using System;
using System.Data.Common;
using System.IO;

namespace compiler.frontend
{
    // TODO: write unit test for ident v. number storage
    public class Lexer
    {
        /// <summary>
        /// A StreamReader to read chars from file
        /// </summary>
        public StreamReader Sr { get; set; }

        /// <summary>
        /// The current character from the file
        /// </summary>
        public char C { get; set; }

        /// <summary>
        /// Table of all program symbols
        /// </summary>
        public SymbolTable SymbolTble { get; }

        /// <summary>
        /// The current Symbol
        /// </summary>
        public int Sym { get; set; }

        /// <summary>
        /// The last Numeric Value
        /// </summary>
        public int Val { get; set; }


        /// <summary>
        /// The last identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The current line number in the source text
        /// </summary>
        public int LineNo              // line number in file
        { set; get; }

        /// <summary>
        /// The current position in the current line
        /// </summary>
        public int Position                 // position in line
        { set; get; }

        /// <summary>
        /// Constructor for the Lexer
        /// </summary>
        /// <param name="filename">The name of the source file to begin tokenizing</param>
        public Lexer(string filename)
        {
            try
            {
                Sr = new StreamReader(filename);
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
                throw;
            }

            SymbolTble = new SymbolTable();
            next();
            LineNo = 1;

        }

        ~Lexer()
        {
            //TODO: Need unit test to verify that destructior releases files
            if (Sr != null)
            {
                Sr.Close();
                Sr = null;
            }
        }

        public char next()
        {
            if (Sr.Peek() == -1)
            {
                throw new Exception("Error: Lexer cannot read beyond the end of the file");
            }
            C = (char)Sr.Read();
            return C;
        }


        public Token getNextToken()
        {
            findNextToken();

            if (char.IsDigit(C))
            {
                return number();
            }
            else if (char.IsLetter(C))
            {
                return symbol();
            }
            else if (!char.IsWhiteSpace(C))
            {
                return punctuation();
            }

            throw new Exception("Error: unable to parse next token");
        }

        public Token number()
        {
            string s = string.Empty;

            while (char.IsDigit(C))
            {
                s += C;
                next();
            }

            Val = int.Parse(s);
            return Token.NUMBER;
        }

        public Token punctuation()
        {
            //TODO: test coverage for this function is weak, add more path coverage
            switch (C)
            {
                case '=':
                    next();
                    if (C != '=')
                    {
                        throw new Exception("Error: '=' is not a valid token, " +
                                            "must be one of: '==', '>=', '<=', '!='");
                    }
                    next();
                    return Token.EQUAL;

                case '!':
                    next();
                    if (C != '=')
                    {
                        throw new Exception("Error: '!' is not a valid token, " +
                                            "must be one of: '==', '>=', '<=', '!='");
                    }
                    next();
                    return Token.NOT_EQUAL;

                case '<':
                    next();
                    if (C == '=')
                    {
                        next();
                        return Token.LESS_EQ;
                    }
                    else if (C == '-')
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
                    if (C == '=')
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
                    if (C == '/')
                    {
                        while (C != '\n')
                        {
                            next();
                        }
                        return Token.COMMENT;
                    }
                    return Token.DIVIDE;
                case '#':
                    next();
                    while (C != '\n')
                    {
                        next();
                    }
                    return Token.COMMENT;

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
            s += C;
            next();

            while (char.IsLetterOrDigit(C))
            {
                s += C;
                next();
            }

            //ret.kind = (int)kind.variable;

            if (SymbolTble.lookup(s))
            {
                Id = SymbolTble.val(s);
            }
            else
            {
                SymbolTble.insert(s);
                Id = SymbolTble.val(s);
            }

            if (SymbolTble.isId(s))
                return Token.IDENTIFIER;
            return (Token)Id;
        }

        /// <summary>
        /// Finds the next token. Scans forward through whitespace.
        /// </summary>
        public void findNextToken()
        {
            while (char.IsWhiteSpace(C))
            {
                next();
            }
        }
    }
}
