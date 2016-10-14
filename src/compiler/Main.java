package compiler;

import compiler.lexer.Token;

/**
 * Created by Paul Kirth on 10/13/16.
 */
public class Main {

    public static void main(String[] args)
    {
        Token c;
        c = Token.classifyToken("<-");

        System.out.println(Token.toString(c));


    }
}
