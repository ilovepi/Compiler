package compiler.lexer;

import com.sun.xml.internal.stream.buffer.stax.StreamReaderBufferProcessor;

import java.io.*;

/**
 * Created by paul on 10/14/16.
 */
public class Scanner {

    private char currChar;
    private String line;
    private int pos;

    private FileReader reader;
    private InputStream is;

    private InputStreamReader in;

    private StreamTokenizer tokenizer;

    private StreamReaderBufferProcessor srbp;




    public Scanner(String filename)
    {
        try {
            is = new FileInputStream(filename);
        }
        catch (FileNotFoundException e) {
            e.printStackTrace();
        }


        in = new InputStreamReader(is);

        tokenizer = new StreamTokenizer(in);

        tokenizer.wordChars('(', '+');
        tokenizer.wordChars('-', '/');

        tokenizer.wordChars(';', '>');
        tokenizer.wordChars('[', '[');

        tokenizer.wordChars(']', ']');
        tokenizer.wordChars('{', '{');
        tokenizer.wordChars('}', '}');
        tokenizer.wordChars(',', ',');


    }





    public TokenNode getNextToken()
    {
        try {
            int tok_val =tokenizer.nextToken();

            switch (tok_val)
            {
                case StreamTokenizer.TT_EOF:
                    return new TokenNode(Token.EOF, "~EOF~");
                case StreamTokenizer.TT_NUMBER:
                    return new TokenNode(Token.NUMBER, tokenizer.nval);
                case StreamTokenizer.TT_WORD:
                    return new TokenNode(Token.classifyToken(tokenizer.sval), tokenizer.sval);
                //case ',':
                //    return new TokenNode(Token.COMMA, ",");
                default:
                    /*
                    char c = (char)tok_val;
                    String s = Character.toString(c);
                    return new TokenNode(Token.classifyToken(s), s);
                    */
                    return new TokenNode(Token.classifyToken(tokenizer.sval), tokenizer.sval);
            }

        } catch (IOException e) {
            e.printStackTrace();
        }

        return new TokenNode( Token.UNKNOWN, tokenizer.sval);
    }



}
