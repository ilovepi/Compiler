namespace compiler.middleend.ir
{
    public enum IrOps
    {
        Neg,
        Add,
        Sub,
        Mul,
        Div,
        Cmp,
        Adda,
        Load,
        Store,
        Move,
        Phi,
        End,
        Bra,
        Bne,
        Beq,
        Ble,
        Blt,
        Bge,
        Bgt,
        Read,
        Write,
        WriteNl,
        Kill,
        Ssa
    }


//TODO: move to own file in backend
}