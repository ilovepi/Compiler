package compiler.parser;

/**
 * Created by paul on 12/18/16.
 */
public class ParserException extends Exception {
    public ParserException(String msg) {
        super(msg);
    }

    public ParserException(TokenNode currToken, Token expected) {
        super("Parser Exception: Syntax Error at line " + currToken.getLineNumber() +
                ": Expected to find '" + Token.toString(expected) +
                "', instead found '" + currToken.getS() + "'.");
    }


    public ParserException(TokenNode currToken, Token expected, String filename) {
        super("Parser Exception: Syntax Error at line " + currToken.getLineNumber() +
                " of file '" + filename + "': Expected to find '" + Token.toString(expected) +
                "', instead found '" + currToken.getS() + "'.");
    }

    public ParserException(TokenNode currToken, Token expected, String filename, String function) {
        super("Parser Exception: Syntax Error in function '" + function + "' at line " + currToken.getLineNumber() +
                " of file '" + filename + "': Expected to find '" + Token.toString(expected) +
                "', instead found '" + currToken.getS() + "'.");
    }

}
