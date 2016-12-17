package compiler.test;

import compiler.lexer.Token;
import compiler.lexer.TokenNode;
import junit.framework.TestCase;

/**
 * Created by paul on 12/16/16.
 */
public class TokenNodeTest extends TestCase {
    public void testGetT() throws Exception {

        TokenNode tested = new TokenNode(Token.ARRAY, "array");
        TokenNode expected = new TokenNode(Token.ARRAY, "array");
        assertEquals(tested.getT(), expected.getT());
    }

    public void testGetS() throws Exception {

        TokenNode tested = new TokenNode(Token.ARRAY, "array");
        TokenNode expected = new TokenNode(Token.ARRAY, "array");
        assertEquals(tested.getS(), expected.getS());
    }

    public void testGetVal() throws Exception {

        TokenNode tested = new TokenNode(Token.ARRAY, "array");
        TokenNode expected = new TokenNode(Token.ARRAY, "array");
        assertEquals(tested.getVal(), expected.getVal());
    }

}