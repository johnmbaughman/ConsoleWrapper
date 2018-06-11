using System;

namespace EchoConsole
{
    public static class Program
    {
        public const string EXCEPTION_KEY = "throwException";
        public const string EXIT_KEY = "exitOnStart";

        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case EXCEPTION_KEY:
                        throw new Exception("This exception was requested by the caller");
                    case EXIT_KEY:
                        return;
                }
            }

            while (true)
                Console.WriteLine(Console.ReadLine());
        }
    }
}
