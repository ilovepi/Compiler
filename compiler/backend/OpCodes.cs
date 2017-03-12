#region Basic header

// MIT License
// 
// Copyright (c) 2016 Paul Kirth
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

#endregion

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
        OR = 8,
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
        ORI = 24,
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