using System;

namespace compiler.frontend
{
    public class ParserException : Exception
    {
        public ParserException()
        {
            
        }

        public ParserException(string message)
            : base(message)
        {
            
        }

        public ParserException(string message, Exception inner)
            : base(message, inner)
        {

        }


        public static ParserException CreateParserException(Token expected, Token found, int line, int pos, string file)
        {
            string message = "Error in file: " + file + " at line " + line + ", pos " + pos +
                             "\n\tFound: " + TokenHelper.ToString(found) + " but Expected: " +
                             TokenHelper.ToString(expected);
            return new ParserException(message);
        }
    }
}