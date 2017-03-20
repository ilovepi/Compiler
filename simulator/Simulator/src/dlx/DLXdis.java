package dlx;

import java.io.FileWriter;

public class DLXdis {
    public static void main(String[] args) {
        try {
            String input = null, output = "out.s";
            for(int i = 0; i<args.length; i++) {
                switch(args[i]) {
                    case "-o":
                        // next item will be the filename of the disassembler
                        // output
                        i++;
                        output = args[i];
                        break;
                    default:
                        // default is intput file name
                        input = args[i];
                        break;
                }
            }
            if(input == null) {
                System.out.println("No input file provided");
                System.exit(1);
            }

            int[] code = DLXutil.readCode(input);
            FileWriter w = new FileWriter(output);
            for(int i : code) {
                w.write(DLX.disassemble(i));
            }
            w.close();

            System.exit(0);
        } catch (Exception e) {
            System.out.println("An error occured");
            System.exit(-1);
        }
    }
}