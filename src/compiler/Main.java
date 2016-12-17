package compiler;

import compiler.lexer.Tokenizer;
import compiler.lexer.Token;
import compiler.lexer.TokenNode;
import compiler.parser.parser;

/**
 * Created by Paul Kirth on 10/13/16.
 */
public class Main {


    public static void main(String[] args) {
        String filename = "src/compiler/test001.txt";
        //Tokenizer s = new Tokenizer(filename);

        //TokenNode tn;

        //String str;

//        do{
//            tn = s.getNextToken();
//            if(tn == null)
//                continue;
//
//            if(tn.getS() != null)
//                str = tn.getS();
//            else
//                str = Integer.toString(tn.getVal());
//
//            if(str == null)
//            {
//                str = "Error: TokenNode was NULL";
//            }
//
//            System.out.println(Token.printToken(tn.getT()) + ": " + str );
//        }
//        while ( tn.getT() != Token.EOF);


        parser p = new parser(filename);

        try {
            p.run();

            p.print();
        } catch (Exception e) {
            e.printStackTrace();
        }


    }
}
