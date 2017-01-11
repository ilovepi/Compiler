package compiler;

import compiler.frontend.lexer.Tokenizer;
import compiler.frontend.lexer.Token;
import compiler.frontend.lexer.TokenNode;

/**
 * Created by Paul Kirth on 10/13/16.
 */
public class Main {


    public static void main(String[] args)
    {
        Tokenizer s = new Tokenizer("src/compiler/test002.txt");

        TokenNode tn;

        String str;

        do{
            tn = s.getNextToken();
            if(tn == null)
                break;

            if(tn.getS() != null)
                str = tn.getS();
            else
                str = Integer.toString(tn.getVal());

            if(str == null)
            {
                str = "Error: TokenNode was NULL";
            }

            System.out.println(Token.printToken(tn.getT()) + ": " + str );
        }
        while (tn != null);


    }
}
