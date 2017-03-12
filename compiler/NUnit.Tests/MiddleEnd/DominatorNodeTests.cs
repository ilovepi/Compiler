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

using System.Linq;
using compiler.middleend.ir;
using NUnit.Framework;

#endregion

namespace NUnit.Tests.MiddleEnd
{
    [TestFixture]
    public class DominatorNodeTests
    {
        [SetUp]
        public void Init()
        {
            _d = new DominatorNode(new BasicBlock("poo"));
        }

        private DominatorNode _d;

        [Test]
        public void TestEquals()
        {
            Assert.NotNull(_d);
            Assert.True(_d.Equals(_d));
            Assert.False(_d.Equals(null));
            var e = new DominatorNode(_d.Bb);
            Assert.True(_d.Equals(e));
            _d.Children.Add(e);
            Assert.False(_d.Equals(e));

            _d.RemoveChild(e);
            e.Bb = null;
            Assert.False(_d.Equals(e));
            _d.InsertChild(e);
            e.Bb = new BasicBlock("This");
            Assert.False(_d.Equals(e));

            _d = new DominatorNode(new BasicBlock("poo"));
            e = new DominatorNode(_d.Bb);
            _d.InsertChild(new DominatorNode(new BasicBlock("that")));
            e.Children.Add(_d.Children.First());
            Assert.True(_d.Equals(e));

            e.Children.Clear();
            e.InsertChild(new DominatorNode(new BasicBlock("the other thing")));
            Assert.False(_d.Equals(e));
        }


        [Test]
        public void TestIsRoot()
        {
            Assert.True(_d.IsRoot());
            var e = new DominatorNode(new BasicBlock("poo"));
            e.InsertChild(_d);
            Assert.False(_d.IsRoot());
        }
    }
}