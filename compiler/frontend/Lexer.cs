using System;
using System.IO;

namespace compiler.frontend
{
    public class Lexer : IDisposable
    {
        /// <summary>
        ///     Constructor for the Lexer
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

        /// <summary>
        ///     A StreamReader to read chars from file
        /// </summary>
        public StreamReader Sr { get; set; }

        /// <summary>
        ///     The current character from the file
        /// </summary>
        public char C { get; set; }

        /// <summary>
        ///     Table of all program symbols
        /// </summary>
        public SymbolTable SymbolTble { get; }

        /// <summary>
        ///     The current Symbol
        /// </summary>
        public int Sym { get; set; }

        /// <summary>
        ///     The last Numeric Value
        /// </summary>
        public int Val { get; set; }


        /// <summary>
        ///     The last identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        ///     The current line number in the source text
        /// </summary>
        public int LineNo { set; get; }

        /// <summary>
        ///     The current position in the current line
        /// </summary>
        public int Position { set; get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Lexer()
        {
            Dispose(false);
        }

        public char Next()
        {
            if (Sr.Peek() == -1)
            {
                C = '.';
                throw new IOException("Error: Lexer cannot read beyond the end of the file");
            }
            C = (char) Sr.Read();

            // if(C== '\r')
            // {
            //     return Next();
            // }
            // else

            if (C == '\n')
            {
                Position = 1;
                LineNo++;
            }
            else
            {
                Position++;
            }

            return C;
        }


        public Token GetNextToken()
        {
            Token token = NextToken();
            Sym = (int) token;
            return token;
        }

        private Token NextToken()
        {
            FindNextToken();

            if (char.IsDigit(C))
            {
                return Number();
            }
            if (char.IsLetter(C))
            {
                return Symbol();
            }
            if (!char.IsWhiteSpace(C))
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
            switch (C)
            {
                case '=':
                    Next();
                    if (C != '=')
                    {
                        return Token.UNKNOWN;
                    }
                    Next();
                    return Token.EQUAL;

                case '!':
                    Next();
                    if (C != '=')
                    {
                        return Token.UNKNOWN;
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
                    if (C == '-')
                    {
                        Next();
                        return Token.ASSIGN;
                    }
                    return Token.LESS;

                case '>':
                    Next();
                    if (C == '=')
                    {
                        Next();
                        return Token.GREATER_EQ;
                    }
                    return Token.GREATER;


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
                Id = SymbolTble.Values[s];
            }
            else
            {
                SymbolTble.Insert(s);
                Id = SymbolTble.Values[s];
            }

            if (SymbolTble.IsId(s))
            {
                return Token.IDENTIFIER;
            }
            return (Token) Id;
        }

        /// <summary>
        ///     Finds the next token. Scans forward through whitespace.
        /// </summary>
        private void FindNextToken()
        {
            while (char.IsWhiteSpace(C))
            {
                Next();
            }
        }

        private void ReleaseUnmanagedResources()
        {
            
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                Sr?.Dispose();
            }
        }
    }
}