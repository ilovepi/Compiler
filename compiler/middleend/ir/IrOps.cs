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
        WriteNl
    }


//TODO: move to own file in backend
    public enum OpCodes
    {
        Add = 0,
        Sub = 1,
        Mul = 2,
        Div = 3,
        Mod = 4,
        Cmp = 5,
        Or = 8,
        And = 9,
        Bic = 10,
        Xor = 11,
        Lsh = 12,
        Ash = 13,
        Chk = 14,

        Addi = 16,
        Subi = 17,
        Muli = 18,
        Divi = 19,
        Modi = 20,
        Cmpi = 21,
        Ori = 24,
        Andi = 25,
        Bici = 26,
        Xori = 27,
        Lshi = 28,
        Ashi = 29,
        Chki = 30,

        Ldw = 32,
        Ldx = 33,
        Pop = 34,
        Stw = 36,
        Stx = 37,
        Psh = 38,


        Beq = 40,
        Bne = 41,
        Blt = 42,
        Bge = 43,
        Ble = 44,
        Bgt = 45,
        Bsr = 46,
        Jsr = 48,
        Ret = 49,
        Rdd = 50,
        Wrd = 51,
        Wrh = 52,
        Wrl = 53
    }
}