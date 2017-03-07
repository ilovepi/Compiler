using System;

namespace compiler.frontend
{
    [Serializable]
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        {
        }

        public static ParserException CreateParserException(Token expected, Token found, int line, int pos, string file)
        {
            string message = "Error in file: " + file + " at line " + line + ", pos " + pos +
                             "\n\tFound: " + TokenHelper.ToString(found) + " but Expected: " +
                             TokenHelper.ToString(expected);
            return new ParserException(message);
        }

        public static ParserException CreateParserException(Token found, int line, int pos, string file)
        {
            string message = "Error in file: " + file + " at line " + line + ", pos " + pos +
                             "\n\tFound: " + TokenHelper.ToString(found);
            return new ParserException(message);
        }

        public static ParserException CreateParserException(string msg, int line, int pos, string file)
        {
            string message = "Error in file: " + file + " at line " + line + ", pos " + pos + "\n" + msg;

            return new ParserException(message);
        }

    }
}