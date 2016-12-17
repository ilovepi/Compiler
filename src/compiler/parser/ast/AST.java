package compiler.parser.ast;

import compiler.lexer.*;


/**
 * Created by paul on 12/16/16.
 */
public class AST {

    private static Tokenizer tokenizer;
    private static TokenNode currToken;
    private static TokenNode nextToken;
    private AST left;
    private AST mid;
    private AST right;
    private TokenNode tn;
    private AstType nodeType;

    public AST() {
        this(null, null);
    }


    public AST(Tokenizer tok) throws Exception {
        AST.tokenizer = tok;
        AstType nodeType = AstType.root;
        TokenNode tn = null;
        right = null;
        mid = null;
        left = computation();

    }

    public AST(AstType new_type, TokenNode tokenNode) {
        nodeType = new_type;
        tn = tokenNode;
        left = null;
        right = null;
        mid = null;
    }


    public AST(AstType new_type) {
        this.nodeType = new_type;
        tn = null;
        left = null;
        right = null;
        mid = null;
    }

    private void getNextToken() {
        currToken = nextToken;
        do {
            nextToken = tokenizer.getNextToken();
        } while (nextToken != null && nextToken.getT() == Token.COMMENT);
    }

    private AST computation() throws Exception {

        while (currToken == null) {
            getNextToken();
        }
        errorCheck(Token.MAIN);

        AST compNode = new AST(AstType.computation, currToken);
        compNode.left = declarations();
        compNode.right = body();

        getNextToken();
        errorCheck(Token.EOF);

        return compNode;
    }

    public AST body() throws Exception {
        //getNextToken();
        errorCheck(Token.OPEN_CURL);

        AST bodyNode = new AST(AstType.body);

        bodyNode.left = new AST(AstType.terminal, currToken);

        bodyNode.mid = statSequence();

        getNextToken();
        errorCheck(Token.CLOSE_CURL);

        bodyNode.right = new AST(AstType.terminal, currToken);


        return bodyNode;

    }

    public AST declarations() throws Exception {

        AST dec = new AST(AstType.declarations);

        dec.left = varDecl(false);
        dec.right = funcDel();

        if (!dec.hasChildren())
            dec = null;

        return dec;
    }

    private AST funcDel() {
        AST funDec = new AST(AstType.funcDecl);

        getNextToken();
        if (currToken.getT() != Token.FUNCTION)
            return null;

        return null;
    }

    private AST varDecl(boolean isStarted) throws Exception {
        AST vd = new AST(AstType.varDecl);

        if (!isStarted) {
            vd.left = typeDecl(false);
        } else {
            getNextToken();
            vd.left = new AST(AstType.terminal, currToken);
        }

        vd.mid = ident();

        if (nextToken.getT() == Token.COMMA) {
            vd.right = varDecl(true);
        } else if (nextToken.getT() == Token.SEMI_COLON) {

            getNextToken();
            vd.right = new AST(AstType.terminal, currToken);
        }

        if (!vd.hasChildren())
            vd = null;


        return vd;
    }

    private boolean hasChildren() {
        return (left != null || mid != null || right != null);
    }

    private AST aryDecl() throws Exception {
        AST aryDecl = new AST(AstType.aryDecl);
        getNextToken();

        errorCheck(Token.OPEN_BRACKET);

        aryDecl.left = new AST(AstType.terminal, currToken);
        getNextToken();

        aryDecl.mid = number();

        getNextToken();
        errorCheck(Token.CLOSE_BRACKET);
        aryDecl.right = new AST(AstType.terminal, currToken);


        return aryDecl;
    }

    private AST typeDecl(boolean startedAry) throws Exception {
        AST td = new AST(AstType.typeDecl);
        getNextToken();

        Token t = currToken.getT();

        if (startedAry) {
            td.left = aryDecl();

            td.right = typeDecl(true);

            return td;
        }

        if (t == Token.ARRAY) {

            td.left = new AST(AstType.terminal, currToken);

            td.right = typeDecl(true);

        } else if (t == Token.VAR) {
            td.left = new AST(AstType.terminal, currToken);
        }

        if (td.hasChildren())
            return td;

        return null;
    }

    public AST statSequence() throws Exception {
        AST stSeq = new AST(AstType.statSequence);

        stSeq.left = statement();
        if (stSeq.left == null)
            return null;

        if (nextToken.getT() != Token.SEMI_COLON)
            return stSeq;

        getNextToken();
        stSeq.mid = new AST(AstType.terminal, currToken);

        stSeq.right = statSequence();

        //if (stSeq.right == null)
        //  throw new Exception("Parse Error: Expected to find end of function body '}' ");
        return stSeq;
    }

    private AST statement() throws Exception {

        Token t = nextToken.getT();
        switch (t) {
            case LET:
                return asgnStmt();
            case CALL:
                return callStmt();
            case IF:
                return ifStmt();
            case WHILE:
                return whileStmt();
            case RETURN:
                return returnStmt();
        }

        return null;
    }

    private AST returnStmt() throws Exception {
        getNextToken();
        errorCheck(Token.RETURN);
        AST ret = new AST(AstType.returnStatement, currToken);

        ret.mid = expression();

        return ret;
    }

    private AST expression() throws Exception {
        AST exp = new AST(AstType.expression);

        exp.left = term();

        if (nextToken.getT() == Token.PLUS || nextToken.getT() == Token.MINUS) {
            getNextToken();
            exp.mid = new AST(AstType.terminal, currToken);
            getNextToken();
            exp.right = factor();
        }

        if (exp.hasChildren())
            return exp;

        return null;
    }

    private AST term() throws Exception {

        AST term = new AST(AstType.term);

        term.left = factor();
        if (nextToken.getT() == Token.TIMES || nextToken.getT() == Token.DIVIDE) {
            getNextToken();
            term.mid = new AST(AstType.terminal, currToken);
            //getNextToken();
            term.right = factor();
        }

        return term;
    }

    private AST factor() throws Exception {
        Token t = nextToken.getT();
        switch (t) {
            case NUMBER:
                return number();
            case CALL:
                return funcCall();
            case IDENTIFIER:
                return designator();
            case OPEN_PAREN:
                getNextToken();
                AST exp = new AST(AstType.factor);
                exp.left = new AST(AstType.terminal, currToken);

                exp.mid = expression();

                getNextToken();
                exp.right = new AST(AstType.terminal, currToken);

                return exp;
        }
        return null;
    }

    private AST ident() throws Exception {
        getNextToken();
        errorCheck(Token.IDENTIFIER);
        AST id = new AST(AstType.terminal, currToken);

        return id;
    }

    private AST number() throws Exception {
        getNextToken();
        errorCheck(Token.NUMBER);

        return new AST(AstType.terminal, currToken);
    }

    private AST funcCall() throws Exception {
        getNextToken();

        errorCheck(Token.CALL);

        AST func = new AST(AstType.funcCall);
        func.left = new AST(AstType.terminal, currToken);

        func.mid = ident();

        func.left = params();

        return func;
    }

    private AST params() throws Exception {
        if (nextToken.getT() != Token.OPEN_PAREN)
            return null;

        getNextToken();

        AST params = new AST(AstType.funcCall);

        params.left = new AST(AstType.terminal, currToken);

        //getNextToken();

        mid = paramList();

        getNextToken();

        params.right = new AST(AstType.terminal, currToken);

        errorCheck(Token.CLOSE_PAREN);

        return params;
    }

    private AST paramList() throws Exception {

        AST exp = expression();

        if (exp != null) {
            if (nextToken.getT() == Token.COMMA) {
                getNextToken();
                exp.right = paramList();
            }
        }
        return exp;
    }

    private AST designator() throws Exception {

        AST desig = ident();
        if (nextToken.getT() == Token.OPEN_BRACKET) {
            desig.right = aryIndex();
        }

        return desig;
    }

    private AST aryIndex() throws Exception {
        getNextToken();
        AST ary = new AST(AstType.aryIndex);

        errorCheck(Token.OPEN_BRACKET);

        ary.left = new AST(AstType.terminal, currToken);
        getNextToken();


        ary.mid = expression();

        getNextToken();

        errorCheck(Token.CLOSE_BRACKET);
        ary.right = new AST(AstType.terminal, currToken);

        return ary;
    }


    //todo: complete while and if statements
    private AST whileStmt() {
        return null;
    }

    private AST ifStmt() {
        return null;
    }

    private AST callStmt() throws Exception {
        getNextToken();
        errorCheck(Token.CALL);
        AST call = new AST(AstType.funcCall);
        call.left = new AST(AstType.terminal, currToken);

        call.mid = ident();
//        getNextToken();
        call.right = params();

        if (!call.hasChildren())
            return null;

        return call;
    }

    private AST asgnStmt() throws Exception {
        getNextToken();
        errorCheck(Token.LET);
        AST asgn = new AST(AstType.assignment);
        asgn.left = new AST(AstType.terminal, currToken);

        asgn.mid = designator();
        getNextToken();
        errorCheck(Token.ASSIGN);
        asgn.tn = currToken;
        //getNextToken();
        asgn.right = expression();

        if (!asgn.hasChildren())
            return null;

        return asgn;
    }

    void errorCheck(Token t) throws Exception {
        if (currToken.getT() != t)
            throw new Exception("Parse Error: Expected to find '" + Token.toString(t) + "', instead found " + currToken.getT().toString());

    }


    public void print() {
        if (tn != null)
            System.out.println(Token.printToken(tn.getT()) + ": " + tn.getS());

        if (left != null)
            left.print();
        if (mid != null)
            mid.print();
        if (right != null)
            right.print();
    }

    enum AstType {
        root,
        computation,
        declarations,
        body,
        varDecl,
        typeDecl,
        funcDecl,
        variable,
        aryDecl,
        array,
        aryIndex,
        statement,
        statSequence,
        assignment,
        funcCall,
        ifStatement,
        whileStatement,
        returnStatement,
        designator,
        factor,
        term,
        expression,
        relation,

        //terminals
        terminal

    }


}
