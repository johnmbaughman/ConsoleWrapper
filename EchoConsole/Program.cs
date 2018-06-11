using System;

namespace EchoConsole
{
    public static class Program
    {
        public const string EXCEPTION_KEY = "throwException";

        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == EXCEPTION_KEY)
                throw new Exception("This exception was requested by the caller");

            while (true)
                Console.WriteLine(Console.ReadLine());
        }
    }
}
