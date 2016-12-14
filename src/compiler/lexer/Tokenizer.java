package compiler.lexer;

import com.sun.xml.internal.stream.buffer.stax.StreamReaderBufferProcessor;

import java.io.*;

/**
 * Created by paul on 10/14/16.
 *
 *
 */
public class Tokenizer {

    private char currChar;
    private String line;
    private int pos;


    private FileReader fr;
    private BufferedReader reader;



    //TODO: Replace the StreamTokenizer with my own tokenizer class

    public Tokenizer(String filename) {
        try {
            fr = new FileReader(filename);
            reader = new BufferedReader(fr);

            line = reader.readLine();

            pos = 0;

        } catch (IOException e) {
            e.printStackTrace();
        }
    } // end Tokenizer(filename)

    public TokenNode getNextToken() {

        if(line == null)
            return null;

        if(line.isEmpty())
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
                do {
                    line = reader.readLine();
                } while(line != null && line.isEmpty());

                pos = 0;
            } catch (IOException e) {
                e.printStackTrace();
            }
        }
        return new TokenNode(Token.classifyToken(token), token);

    }

    private int findEndOfToken(String str, int position)
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
               /*
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
                   */
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
                   if( !Character.isLetterOrDigit(currentChar))
                       return i;
           }
        }

        return str.length();

    }



}
