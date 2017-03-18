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

#region

using compiler;
using compiler.frontend;
using compiler.middleend.optimization;
using NUnit.Framework;

#endregion

namespace NUnit.Tests
{
    [TestFixture]
    public class IntegrationTest
    {




        [Test, Combinatorial]
        public void CompilerIntegrationTest([Values(@"/Frontend/testdata/test001.txt",
                @"/Frontend/testdata/test002.txt",
                @"/Frontend/testdata/test003.txt",
                @"/Frontend/testdata/test004.txt",
                @"/Frontend/testdata/test005.txt",
                @"/Frontend/testdata/test006.txt",
                @"/Frontend/testdata/test007.txt",
                @"/Frontend/testdata/test008.txt",
                @"/Frontend/testdata/test009.txt",
                @"/Frontend/testdata/test010.txt",
                @"/Frontend/testdata/test011.txt",
                @"/Frontend/testdata/test012.txt",
                @"/Frontend/testdata/test013.txt",
                @"/Frontend/testdata/test014.txt",
                @"/Frontend/testdata/test015.txt",
                @"/Frontend/testdata/test016.txt",
                @"/Frontend/testdata/test017.txt",
                @"/Frontend/testdata/test018.txt",
                @"/Frontend/testdata/test019.txt",
                @"/Frontend/testdata/test020.txt",
                @"/Frontend/testdata/test021.txt",
                @"/Frontend/testdata/test022.txt",
                @"/Frontend/testdata/test023.txt",
                @"/Frontend/testdata/test024.txt",
                @"/Frontend/testdata/test025.txt",
                @"/Frontend/testdata/test026.txt",
                @"/Frontend/testdata/test027.txt",
                @"/Frontend/testdata/test028.txt",
                @"/Frontend/testdata/test029.txt",
                @"/Frontend/testdata/test030.txt",
                @"/Frontend/testdata/test031.txt",
                @"/Frontend/testdata/big.txt",
                @"/Frontend/testdata/cell.txt",
                @"/Frontend/testdata/factorial.txt")] string pFilename,
            [Values(true)] bool copyProp, [Values(true)] bool cse, [Values(false)] bool deadCode, [Values(true, false)] bool prune)
        {
            string filename = TestContext.CurrentContext.TestDirectory + pFilename;
            Compiler.TestRun(filename, copyProp, cse,deadCode,prune);
        }

       [Test,Combinatorial]
        public void CompilerIntegrationErrorTest([Values(@"/Frontend/testdata/test004bad.txt", @"/Frontend/testdata/test013bad.txt", @"/Frontend/testdata/test014bad.txt", @"/Frontend/testdata/test028bad.txt", @"/Frontend/testdata/test031bad.txt")] string pFilename,
         [Values(true, false)] bool copyProp, [Values(true, false)] bool cse, [Values(true, false)] bool deadCode, [Values(true, false)] bool prune)
        {
            string filename = TestContext.CurrentContext.TestDirectory + pFilename;
            Assert.Throws<ParserException>(() => Compiler.TestRun(filename, copyProp, cse, deadCode, prune));
        }
    }
}
