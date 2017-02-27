using compiler;

namespace Program
{
    internal class Program
    {
        //TODO: adjust main to use the parser when it is complete
        private static void Main(string[] args)
        {
            Compiler.DefaultRun(@"../../testdata/test002.txt");
        }
    }
}