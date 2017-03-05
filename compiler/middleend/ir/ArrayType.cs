using System;
using System.Collections.Generic;

namespace compiler.middleend.ir
{
    public class ArrayType : VariableType
    {
        public List<int> Dimensions { get; set; }

        public ArrayType() : base(string.Empty, 0, 0, 1, true)
        {
            
        }

        public ArrayType(List<int> dims) : base(string.Empty, 0, 0, 1, true)
        {
            Dimensions = new List<int>(dims);

            foreach (int dimension in Dimensions)
            {
                Size *= dimension;
            }
        }

        //constructor for arrays
        public ArrayType(int pId, int pOffset, List<int> dims) : base(String.Empty, pId, pOffset, 1, true)
        {
            Dimensions = new List<int>(dims);

            foreach (int dimension in Dimensions)
            {
                Size *= dimension;
            }

        }


        //constructor for arrays
        public ArrayType(string pName, int pId, int pOffset, List<int> dims ): base(pName, pId, pOffset, 1, true)
        {
            Dimensions = new List<int>(dims);

            foreach (int dimension in Dimensions)
            {
                Size *= dimension;
            }

        }

        public ArrayType(string pName, int pId, int pOffset, List<int> dims , bool pIsAray) : base(pName, pId, pOffset, 1, pIsAray)
        {
            Dimensions = new List<int>(dims);

            foreach (int dimension in Dimensions)
            {
                Size *= dimension;
            }
        }
    }
}