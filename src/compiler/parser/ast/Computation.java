package compiler.parser.ast;

import compiler.lexer.Tokenizer;

/**
 * Created by paul on 12/18/16.
 */
public class Computation implements ASTNode{
    String label;
    VarDecls vars;
    FuncDecls funcs;
    Body body;

    Computation(Tokenizer tok)
    {
        tokenizer = tok;

       // find main

        // handle varDecl

        // handle funDecl

        //hand body

        // handle EOF

    }

    @Override
    public void visit() {

    }

    @Override
    public void print() {

    }
}
