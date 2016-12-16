package compiler.parser.ast.nodes;

import org.w3c.dom.ranges.RangeException;

import static org.w3c.dom.ranges.RangeException.BAD_BOUNDARYPOINTS_ERR;

/**
 * Created by paul on 12/16/16.
 */
public class ArrayDecl extends VarDecl{

    private String name;
    private int size;
    private boolean isMultiDimesional;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public int getSize() {
        return size;
    }

    public void setSize(int size) {
        if( size < 1 )
                throw new RangeException(BAD_BOUNDARYPOINTS_ERR, "Array size must be greater than zero");
        this.size = size;
    }

    public boolean isMultiDimesional() {
        return isMultiDimesional;
    }

    public void setMultiDimensional(boolean multiDimesional) {
        isMultiDimesional = multiDimesional;
    }


    /**
     * @param name Name of the array
     * @param new_size the number of elements in the array
     * @param isMultiD true if the array is multi dimensional
     */
    public ArrayDecl(String name, int new_size, boolean isMultiD)
    {
        super(name);
       setSize(new_size);
       setMultiDimensional(isMultiD);
    }


}
