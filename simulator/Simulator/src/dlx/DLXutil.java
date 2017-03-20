package dlx;

import java.io.FileInputStream;
import java.util.ArrayList;

public class DLXutil {
    public static int[] readCode(String filename) throws Exception {
        FileInputStream in = new FileInputStream(filename);
        ArrayList<Integer> r = new ArrayList<Integer>();

        int b;
        int count = 0;
        int current = 0;
        while((b = in.read()) != -1) {
            current = current | (b << ((count % 4) * 8));
            if(count % 4 == 3) {
                r.add(current);
                current = 0;
            }
            count++;
        }
        in.close();

        int[] rval = new int[r.size()];
        for(int i = 0; i < r.size(); i++) {
            rval[i] = r.get(i);
        }
        return rval;
    }
}