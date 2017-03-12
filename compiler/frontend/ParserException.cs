﻿#region Basic header

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

#region

using System;

#endregion

namespace compiler.frontend
{
    [Serializable]
    public class ParserException : Exception
    {
        public ParserException(string message)
            : base(message)
        {
        }

        public static ParserException CreateParserException(Token expected, Token found, int line, int pos, string file)
        {
            string message = "Error in file: " + file + " at line " + line + ", pos " + pos +
                             "\n\tFound: " + TokenHelper.ToString(found) + " but Expected: " +
                             TokenHelper.ToString(expected);
            return new ParserException(message);
        }

        public static ParserException CreateParserException(Token found, int line, int pos, string file)
        {
            string message = "Error in file: " + file + " at line " + line + ", pos " + pos +
                             "\n\tFound: " + TokenHelper.ToString(found);
            return new ParserException(message);
        }

        public static ParserException CreateParserException(string msg, int line, int pos, string file)
        {
            string message = "Error in file: " + file + " at line " + line + ", pos " + pos + "\n" + msg;

            return new ParserException(message);
        }
    }
}