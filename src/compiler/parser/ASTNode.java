package compiler.parser;

import compiler.lexer.Token;

import java.util.concurrent.Callable;

/**
 * Created by paul on 12/14/16.
 *
 */
public class ASTNode<T> {

    /*
        designator = ident{ "[" expression "]" }.
        factor = designator | number | “(“ expression “)” | funcCall .
        term = factor { (“*” | “/”) factor}.
        expression = term {(“+” | “-”) term}.
        relation = expression relOp expression .
        assignment = “let” designator “<-” expression.
        funcCall = “call” ident [ “(“ [expression { “,” expression } ] “)” ].
        ifStatement = “if” relation “then” statSequence [ “else” statSequence ] “fi”.
        whileStatement = “while” relation “do” StatSequence “od”.
        returnStatement = “return” [ expression ] .
        statement = assignment | funcCall | ifStatement | whileStatement | returnStatement.
        statSequence = statement { “;” statement }.
        typeDecl = “var” | “array” “[“ number “]” { “[“ number “]” }.
        varDecl = typeDecl indent { “,” ident } “;” .
        funcDecl = (“function” | “procedure”) ident [formalParam] “;” funcBody “;” .
        formalParam = “(“ [ident { “,” ident }] “)” .
        funcBody = { varDecl } “{” [ statSequence ] “}”.
        computation = “main” { varDecl } { funcDecl } “{” statSequence “}” “.” .

     */


    public enum NodeType
    {
        expression,
        designator,
        factor,
        term,
        relation,
        statement,
        statSeq,
        assignment,
        funcCall,
        ifStatement,
        whileStatement,
        returnStatement,
        typeDecl,
        varDecl,
        funcDecl,
        formalParam,
        funcBody,
        computation
    }

    public ASTNode left;
    public ASTNode right;

    public Token m_token;
    public T m_value;


    public ASTNode(Token t, T val)
    {
        m_token = t;
        m_value = val;
        left = null;
        right = null;
    }





    //public NodeType nodeType;

}
