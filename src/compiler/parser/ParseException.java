package compiler.parser;

/**
 * Created by paul on 12/18/16.
 */
public class ParserException extends Exception{
    private int lineNumber;
    private int offset;

    ParserException(String expected, String found, int line)
    {
        super("");
        String msg = "Parse Error at line " + line +
                " : Expected to find '" + expected +
                "', found '" +found +"' instead.";
        super.

        line
    }

}
