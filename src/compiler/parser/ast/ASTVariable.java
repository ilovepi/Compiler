package compiler.parser.ast;

/**
 * Created by paul on 12/16/16.
 */
public class ASTVariable {
    public String name;
    public int value;

    ASTVariable(){
        name = "";
        value = 0;
    }

    ASTVariable(String pName)
    {
        this.name = pName;
    }
}
