package compiler.parser.ast;

import compiler.lexer.*;
import org.jetbrains.annotations.Contract;
import org.jetbrains.annotations.Nullable;


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

    /**
     * Creates an AST using the given tokenizer.
     *
     * @param tok
     * @throws Exception
     */
    public AST(Tokenizer tok) throws Exception {
        AST.tokenizer = tok;
        AstType nodeType = AstType.root;
        TokenNode tn = null;
        right = null;
        mid = null;
        left = computation();

    }

    /**
     * Creates an AST node with node and token information
     *
     * @param new_type
     * @param tokenNode
     */
    public AST(AstType new_type, TokenNode tokenNode) {
        nodeType = new_type;
        tn = tokenNode;
        left = null;
        right = null;
        mid = null;
    }


    /**
     * Creates an AST node with AST information only
     *
     * @param new_type
     */
    public AST(AstType new_type) {
        this.nodeType = new_type;
        tn = null;
        left = null;
        right = null;
        mid = null;
    }


    /**
     * Advances the tokenizer to get the next token from the file.
     */
    private void getNextToken() {
        currToken = nextToken;
        do {
            nextToken = tokenizer.getNextToken();
        } while (nextToken != null && nextToken.getT() == Token.COMMENT);
    }

    /**
     * Creates a computation node
     *
     * @return An AST with with the computation node as the root.
     * @throws Exception
     */
    private AST computation() throws Exception {

        while (currToken == null) {
            getNextToken();
        }
        errorCheck(Token.MAIN);

        AST compNode = new AST(AstType.computation, currToken);
        compNode.left = declarations();
        compNode.right = body();

        if (compNode.right == null)
            throw new Exception("Parse Error: Main cannot have an empty function body.");

        getNextToken();
        errorCheck(Token.EOF);

        return compNode;
    }

    /**
     * Creates an AST node for a function body
     *
     * @return An AST with the function body as the root
     * @throws Exception
     */
    private AST body() throws Exception {
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

    /**
     * creates a super node for all types of declarations (allowed to be null)
     *
     * @return An AST, or null
     * @throws Exception
     */
    private AST declarations() throws Exception {

        AST dec = new AST(AstType.declarations);

        dec.left = varDecl(false);
        dec.right = funcDel();

        if (!dec.hasChildren())
            dec = null;

        return dec;
    }

    //todo: finish function declarations
    @Nullable
    private AST funcDel() {
        AST funDec = new AST(AstType.funcDecl);

        getNextToken();
        if (currToken.getT() != Token.FUNCTION)
            return null;

        //add parameters

        //add declarations

        //add body


        // return null if funDec is empty
        if (!funDec.hasChildren())
            return null;

        return funDec;
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

    @Contract(pure = true)
    private boolean hasChildren() {
        return (left != null || mid != null || right != null);
    }

    /**
     * Creates an Array Declaration node to hande complexites of multidimensitonal arrays
     *
     * @return AST node holding an array declaration
     * @throws Exception
     */
    private AST aryDecl() throws Exception {
        AST aryDecl = new AST(AstType.aryDecl);
        getNextToken();

        // handle ope bracket
        errorCheck(Token.OPEN_BRACKET);

        aryDecl.left = new AST(AstType.terminal, currToken);
        getNextToken();

        // get array size
        aryDecl.mid = number();

        //handle close bracket
        getNextToken();
        errorCheck(Token.CLOSE_BRACKET);
        aryDecl.right = new AST(AstType.terminal, currToken);

        return aryDecl;
    }

    /**
     * TypeDecl node, holds type declarations
     *
     * @param startedAry true if an array is being parsed
     * @return An AST node containing a TypeDecl
     * @throws Exception Parse errors
     */
    @Nullable
    private AST typeDecl(boolean startedAry) throws Exception {
        AST td = new AST(AstType.typeDecl);
        getNextToken();

        // cache the current token
        Token t = currToken.getT();

        // check if we are processing continued array declaratins
        if (startedAry) {
            //complex array declarations are to the left
            td.left = aryDecl();

            // remaining parts of type declaration on the right
            td.right = typeDecl(true);

            return td;
        }

        // handle the 'array' keyword token
        if (t == Token.ARRAY) {

            //insert the keyword
            td.left = new AST(AstType.terminal, currToken);

            // begin processing the array type declaration
            td.right = typeDecl(true);

            // handle the 'var' keyword token
        } else if (t == Token.VAR) {
            td.left = new AST(AstType.terminal, currToken);
        }

        // if there are no children return null
        if (td.hasChildren())
            return td;

        return null;
    }

    @Nullable
    private AST statSequence() throws Exception {
        AST stSeq = new AST(AstType.statSequence);

        stSeq.left = statement();

        //return null if there are no statements left
        if (stSeq.left == null)
            return null;

        // return if there are not other statements
        if (nextToken.getT() != Token.SEMI_COLON)
            return stSeq;

        getNextToken();
        stSeq.mid = new AST(AstType.terminal, currToken);

        //recursively check stat sequences
        stSeq.right = statSequence();

        // statsequence cannot finish on a semicolon
        if (stSeq.right == null)
            throw new Exception("Parse Error: Expected to find end of function body '}' ");
        return stSeq;
    }

    /**
     * Crate an AST for a statement
     *
     * @return An AST node for a statment
     * @throws Exception
     */
    @Nullable
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
        return new AST(AstType.terminal, currToken);
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

        params.mid = paramList();

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
                exp.mid = new AST(AstType.terminal, currToken);
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

    //TODO: implement if statements
    private AST ifStmt() {
        return null;
    }

    @Nullable
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

    @Nullable
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

    /**
     * Checks if the current token has the expected value
     *
     * @param t the Token value to compare currToken with
     * @throws Exception Throws a Parse exception detailing what the parser expected
     */
    private void errorCheck(Token t) throws Exception {
        if (currToken.getT() != t)
            throw new Exception("Parse Error: Expected to find '" + Token.toString(t) + "', instead found " + currToken.getT().toString());
    }


    /**
     * prints out an AST
     */
    public void printTree() {
        print();

        if (left != null)
            left.printTree();
        if (mid != null)
            mid.printTree();
        if (right != null)
            right.printTree();
    }

    public void print() {
        if (tn != null)
            System.out.println(Token.printToken(tn.getT()) + ": " + tn.getS());
    }


    //TODO: Fix methods to print the AST in a visual way
    /**
     * prints the tree one level at a time to help visualization
     * @param depth the maximum depth to print
     */
    public void printTree(int depth) {
        for (int i = 0; i < depth; i++) {
            printTree(0, i);
            System.out.println();
        }
    }

    /**
     * @param currentDepth the current depth in the AST
     * @param targetDepth The target depth to be printed
     */
    private void printTree(int currentDepth, int targetDepth) {
        if (currentDepth == targetDepth)
        {
            print();
        }
        else {
            printChild(left, currentDepth, targetDepth);
            printChild(mid, currentDepth, targetDepth);
            printChild(right, currentDepth, targetDepth);
        }
    }

    /**
     * Checks if the child is null before calling printTree on it
     * @param child the child to pass to print tree
     * @param currentDepth the depth of the parent node
     * @param targetDepth The target depth for printTree
     */
    private void printChild(AST child, int currentDepth, int targetDepth) {
        if(child != null)
            child.printTree(currentDepth +1, targetDepth);
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
