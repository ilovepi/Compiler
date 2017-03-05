using compiler;

namespace Program
{
    internal class Program
    {
        //TODO: adjust main to use the parser when it is complete
        private static void Main(string[] args)
        {
            //TODO: Be sure to address options parsing
            Compiler.DefaultRun(@"../../testdata/test008.txt");
        }
    }
}