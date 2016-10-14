package compiler;

import compiler.lexer.Scanner;
import compiler.lexer.Token;
import compiler.lexer.TokenNode;

/**
 * Created by Paul Kirth on 10/13/16.
 */
public class Main {


    public static void main(String[] args)
    {
        Scanner s = new Scanner("test001.txt");

        TokenNode tn;

        String str;

        do{
            tn = s.getNextToken();

            if(tn.getS() != null)
                str = tn.getS();
            else
                str = Integer.toString(tn.getVal());

            if(str == null)
            {
                str = "Error: TokenNode was NULL";
            }

            System.out.println(Token.printToken(tn.getT()) + ": " + tn.getVal() );
        }
        while (tn.getT() != Token.EOF);


    }
}
