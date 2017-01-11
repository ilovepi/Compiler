package compiler.frontend.lexer;

import java.io.*;

/**
 * Created by paul on 10/14/16.
 */
public class Tokenizer {

    private char currChar;
    private String line;
    private int pos;

    public int getPos() {
        return pos;
    }

    public int getLineno() {
        return lineno;
    }

    private int lineno;


    private FileReader fr;
    private BufferedReader reader;



    //TODO: Replace the StreamTokenizer with my own tokenizer class

    public Tokenizer(String filename) {
        try {
            fr = new FileReader(filename);
            reader = new BufferedReader(fr);

            line = reader.readLine();

            pos = 0;
            lineno =1;


        } catch (FileNotFoundException e) {
            e.printStackTrace();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    public TokenNode getNextToken() {

        if(line == null)
            return null;
        if(pos >= line.length())
            return null;
        //read character
        char c = line.charAt(pos);
        String token;

        int end = findEndOfToken(line, pos);
        if(end == pos && end < line.length())
            end++;

        token = line.substring(pos, end);

        pos = end;
        if (end == line.length())
        {
            try {
                line = reader.readLine();
                pos = 0;
                lineno++;
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        return new TokenNode(Token.classifyToken(token), token);

    }

    int findEndOfToken(String str, int position)
    {
        boolean started = false;
        char[] word = str.toCharArray();

        for (int i = position; i < str.length(); ++i)
        {
            char currentChar = word[i];
            char nextChar;
            if( i < (str.length()-1 ))
                nextChar = word[i+1];
            else
                nextChar='\n';


            //skip leading whitespace
            if(!started)
            {
                if(Character.isWhitespace(currentChar)) {
                    pos++;
                    continue;
                }
                else
                    started= true;
            }

           switch (currentChar)
           {
               case ' ':
               case '\t':
               case '\n':
               case ';':
               case ',':
               case '+':
               case '-':
               case '*':
               case '{':
               case '}':
               case '(':
               case ')':
                   return i;
               case '<':
               {

                   if(nextChar == '-' || nextChar == '=')
                   {
                       return i+2;
                   }
                   return i;
               }

               case '>':
               case '=':
               case '!':
                   if(nextChar == '=')
                       return i+1;
                   else
                       return i;


               case '/':
                   if(nextChar == '/')
                   {
                       return str.length();
                   }
                   else
                       return i;
               default:
                   if( (currentChar >= 'a' && currentChar <='z' ) ||
                           (currentChar >= 'A' && currentChar <='Z' ) ||
                           (currentChar >= '0' && currentChar <= '9') )
                   {
                       continue;
                   }
                   else {
                       return i;
                   }
           }
        }

        return str.length();

    }












/*
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
/*
                    return new TokenNode(Token.classifyToken(tokenizer.sval), tokenizer.sval);
            }

        } catch (IOException e) {
            e.printStackTrace();
        }

        return new TokenNode( Token.UNKNOWN, tokenizer.sval);
    }

*/

}
