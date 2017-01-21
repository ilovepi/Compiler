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
            Next();
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

        public char Next()
        {
            if (Sr.Peek() == -1)
            {
                throw new Exception("Error: Lexer cannot read beyond the end of the file");
            }
            C = (char)Sr.Read();
            return C;
        }


        public Token GetNextToken()
        {
            FindNextToken();

            if (char.IsDigit(C))
            {
                return Number();
            }
            else if (char.IsLetter(C))
            {
                return Symbol();
            }
            else if (!char.IsWhiteSpace(C))
            {
                return Punctuation();
            }

            throw new Exception("Error: unable to parse next token");
        }

        public Token Number()
        {
            string s = string.Empty;

            while (char.IsDigit(C))
            {
                s += C;
                Next();
            }

            Val = int.Parse(s);
            return Token.NUMBER;
        }

        public Token Punctuation()
        {
            //TODO: test coverage for this function is weak, add more path coverage
            switch (C)
            {
                case '=':
                    Next();
                    if (C != '=')
                    {
                        throw new Exception("Error: '=' is not a valid token, " +
                                            "must be one of: '==', '>=', '<=', '!='");
                    }
                    Next();
                    return Token.EQUAL;

                case '!':
                    Next();
                    if (C != '=')
                    {
                        throw new Exception("Error: '!' is not a valid token, " +
                                            "must be one of: '==', '>=', '<=', '!='");
                    }
                    Next();
                    return Token.NOT_EQUAL;

                case '<':
                    Next();
                    if (C == '=')
                    {
                        Next();
                        return Token.LESS_EQ;
                    }
                    else if (C == '-')
                    {
                        Next();
                        return Token.ASSIGN;
                    }
                    else
                    {
                        return Token.LESS;
                    }

                case '>':
                    Next();
                    if (C == '=')
                    {
                        Next();
                        return Token.GREATER_EQ;
                    }
                    else
                    {
                        return Token.GREATER;
                    }


                case '+':
                    Next();
                    return Token.PLUS;
                case '-':
                    Next();
                    return Token.MINUS;
                case '*':
                    Next();
                    return Token.TIMES;
                case '/':
                    Next();
                    if (C == '/')
                    {
                        while (C != '\n')
                        {
                            Next();
                        }
                        return Token.COMMENT;
                    }
                    return Token.DIVIDE;
                case '#':
                    Next();
                    while (C != '\n')
                    {
                        Next();
                    }
                    return Token.COMMENT;

                case ',':
                    Next();
                    return Token.COMMA;
                case ';':
                    Next();
                    return Token.SEMI_COLON;
                case '.':
                    return Token.EOF;

                case '(':
                    Next();
                    return Token.OPEN_PAREN;

                case ')':
                    Next();
                    return Token.CLOSE_PAREN;


                case '[':
                    Next();
                    return Token.OPEN_BRACKET;
                case ']':
                    Next();
                    return Token.CLOSE_BRACKET;

                case '{':
                    Next();
                    return Token.OPEN_CURL;
                case '}':
                    Next();
                    return Token.CLOSE_CURL;


                default:
                    return Token.UNKNOWN;
            }
        }


        public Token Symbol()
        {
            //Result ret = new Result();

            string s = string.Empty;
            s += C;
            Next();

            while (char.IsLetterOrDigit(C))
            {
                s += C;
                Next();
            }

            //ret.kind = (int)kind.variable;

            if (SymbolTble.Lookup(s))
            {
                Id = SymbolTble.Val(s);
            }
            else
            {
                SymbolTble.Insert(s);
                Id = SymbolTble.Val(s);
            }

            if (SymbolTble.IsId(s))
                return Token.IDENTIFIER;
            return (Token)Id;
        }

        /// <summary>
        /// Finds the next token. Scans forward through whitespace.
        /// </summary>
        public void FindNextToken()
        {
            while (char.IsWhiteSpace(C))
            {
                Next();
            }
        }
    }
}
