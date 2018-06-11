using System;

namespace EchoConsole
{
    public static class Program
    {
        /// <summary>
        /// Indicates the the console should throw an exception on start
        /// </summary>
        public const string EXCEPTION_KEY = "throwException";

        /// <summary>
        /// Indicates the the console should exit on start
        /// </summary>
        public const string EXIT_KEY = "exitOnStart";

        /// <summary>
        /// Indicates that the console should flood the output stream with the text 'ping'
        /// </summary>
        public const string FLOOD_KEY = "floodOnStart";

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
                    case FLOOD_KEY:
                        while (true)
                            Console.WriteLine("ping");
                    default:
                        while (true)
                        {
                            string data = Console.ReadLine();
                            Console.WriteLine(data);
                        }
                }
            } else
            {
                while (true)
                {
                    string data = Console.ReadLine();
                    Console.WriteLine(data);
                }
            }
        }
    }
}
