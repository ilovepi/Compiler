
package dlx;


public class DLXsimulator {

    public static void main(String[] args)
    {
        try{
            String input = null;
            boolean outTime = false;
            for(int i = 0; i<args.length; i++) {
                switch(args[i]) {
                    case "-time":
                        outTime = true;
                        break;
                    default:
                        // default is intput file name, will only execute
                        // a single file, so last one read will win.
                        input = args[i];
                        break;
                }
            }
            if(input == null) {
                System.out.println("No input file provided");
                System.exit(1);
            }

            int[] code = DLXutil.readCode(input);
            DLX.load(code);
            int time = DLX.execute();
            if(outTime) {
                System.err.print(time);
            }
            System.exit(0);

        }
        catch (Exception e)
        {
            System.out.println("Error in simulated program");
            System.exit(-1);
        }

    }
}
