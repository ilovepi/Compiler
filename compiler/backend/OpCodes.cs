namespace compiler.backend
{
    public enum OpCodes
    {
        ADD = 0,
        SUB = 1,
        MUL = 2,
        DIV = 3,
        MOD = 4,
        CMP = 5,
        OR  = 8,
        AND = 9,
        BIC = 10,
        XOR = 11,
        LSH = 12,
        ASH = 13,
        CHK = 14,

        ADDI = 16,
        SUBI = 17,
        MULI = 18,
        DIVI = 19,
        MODI = 20,
        CMPI = 21,
        ORI  = 24,
        ANDI = 25,
        BICI = 26,
        XORI = 27,
        LSHI = 28,
        ASHI = 29,
        CHKI = 30,

        LDW = 32,
        LDX = 33,
        POP = 34,
        STW = 36,
        STX = 37,
        PSH = 38,


        BEQ = 40,
        BNE = 41,
        BLT = 42,
        BGE = 43,
        BLE = 44,
        BGT = 45,
        BSR = 46,
        JSR = 48,
        RET = 49,
        RDD = 50,
        WRD = 51,
        WRH = 52,
        WRL = 53
    }
}