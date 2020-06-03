using ConsoleWrapper;
using System;

namespace TestConsole
{
    public static class Program
    {
        public static void Main()
        {
            CWrapper wrapper = new CWrapper("C:\\Windows\\System32\\cmd.exe");
            wrapper.OutputDataReceived += (s, e) => Console.WriteLine(e.Data);
            wrapper.Execute("/C ping www.google.co.nz");
            wrapper.ExitedMRE.Wait();
        }
    }
}
