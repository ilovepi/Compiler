using System;
using System.Net.Configuration;
using compiler;
using compiler.frontend;

namespace Program
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                //TODO: Be sure to address options parsing
                Compiler.DefaultRun(@"../../testdata/test002.txt");
            }
            catch (ParserException e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}