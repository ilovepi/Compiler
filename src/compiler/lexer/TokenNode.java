package compiler.lexer;

/**
 * Created by paul on 10/14/16.
 */
public class TokenNode {

    private Token t;
    private String s;
    private int val;
    private int lineNumber;

    public TokenNode(Token new_t, String new_s, int line)
    {
        t = new_t;
        s = new_s;
        lineNumber = line;
    }


    public TokenNode(Token new_t, double new_val)
    {
        t = new_t;
        s = null;
        val = (int)new_val;
    }

    public Token getT() {
        return t;
    }

    public String getS() {
        return s;
    }

    public int getVal() {
        return val;
    }

    public int getLineNumber() {
        return lineNumber;
    }
}
