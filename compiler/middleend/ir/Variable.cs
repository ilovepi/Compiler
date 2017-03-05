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
// 
// 
// 
// Created on:  03 04, 2017
#endregion

using System;

namespace compiler.middleend.ir
{
    public class Variable
    {
        public const int Dword = 4;

        public string Name { get; set; }

        public int Id { get; set; }

        public readonly bool IsArray;

        public int Size { get; set; }

        public int Offset { get; set; }

        public int Address { get; set; }

        // constructor for normal variables
        public Variable(string pName, int pId, int pOffset)
        {
            Name = pName;
            Id = pId;
            Offset = pOffset;
            IsArray = false;
            Size = 1;
            Address = 0;
        }

        //constructor for arrays
        public Variable(string pName, int pId, int pOffset, int size)
        {
            Name = pName;
            Id = pId;
            Offset = pOffset;
            IsArray = true;
            Size = size;
            Address = 0;
        }

        public Variable(string pName, int pId, int pOffset, int size, bool pIsAray)
        {
            Name = pName;
            Id = pId;
            Offset = pOffset;
            IsArray = pIsAray;
            Size = size;
            Address = 0;
        }



        public void Allocate(int baseAddr)
        {
            Address = baseAddr + (Offset * Dword);
        }

    }
}