namespace compiler.middlend.ir
{


    public enum IrOps
    {
        neg,
        add,
        sub,
        mul,
        div,
        cmp,
        adda,
        load,
        store,
        move,
        phi,
        end,
        bra,
        bne,
        beq,
        ble,
        blt,
        bge,
        bgt,
        read,
        write,
        writeNL
    };




    public enum OpCodes
    {
        add = 0,
        sub = 1,
        mul = 2,
        div = 3,
        mod = 4,
        cmp = 5,
        or  = 8,
        and = 9,
        bic = 10,
        xor = 11,
        lsh = 12,
        ash = 13,
        chk = 14,

        addi = 16,
        subi = 17,
        muli = 18,
        divi = 19,
        modi = 20,
        cmpi = 21,
        ori  = 24,
        andi = 25,
        bici = 26,
        xori = 27,
        lshi = 28,
        ashi = 29,
        chki = 30,

        ldw = 32,
        ldx = 33,
        pop = 34,
        stw = 36,
        stx = 37,
        psh = 38,


        beq = 40,
        bne = 41,
        blt = 42,
        bge = 43,
        ble = 44,
        bgt = 45,
        bsr = 46,
        jsr = 48,
        ret = 49,
        rdd = 50,
        wrd = 51,
        wrh = 52,
        wrl = 53

    };
}