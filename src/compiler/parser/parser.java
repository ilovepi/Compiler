package compiler.parser;

import compiler.lexer.Tokenizer;
import compiler.parser.ast.AST;

/**
 * Created by paul on 12/16/16.
 * Recursive decent parser for pl241
 * uses a tokenizer to parse the tokens into an AST
 */
public class parser {
    AST ast;


    Tokenizer tokenizer;


    public parser(String filename)
    {
        tokenizer = new Tokenizer(filename);
    }

    public void run() throws Exception {
        ast = new AST(tokenizer);
    }

    public void print()
    {
         ast.print();
    }

}
