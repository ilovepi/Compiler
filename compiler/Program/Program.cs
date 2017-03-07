using System;
using System.Net.Configuration;
using compiler;
using compiler.frontend;

namespace Program
{
    internal class Program
    {
        //TODO: adjust main to use the parser when it is complete
        private static void Main(string[] args)
        {
            try
            {
                //TODO: Be sure to address options parsing
                Compiler.DefaultRun(@"../../testdata/test003.txt");
            }
            catch (ParserException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}