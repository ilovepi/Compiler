package compiler.lexer;

/**
 * Created by paul on 10/14/16.
 */
public class TokenNode {

    private Token t;
    private String s;
    private int val;

    public TokenNode(Token new_t, String new_s)
    {
        t = new_t;
        s = new_s;
    }


    public TokenNode(Token new_t, double new_val)
    {
        t = new_t;
        val = (int)new_val;
    }


}
