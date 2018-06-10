using ConsoleWrapper.Settings;
using System;
using System.Diagnostics;

namespace ConsoleWrapper
{
    public interface ICWrapper : IDisposable
    {
        event DataReceivedEventHandler OutputDataReceived;
        event DataReceivedEventHandler ErrorDataReceived;
        event EventHandler ConsoleAppExited;

        bool Disposed { get; }
        bool Executing { get; }
        string ExecutableLocation { get; }
        WrapperSettings Settings { get; }

        void Execute();
        void Kill();
        void WriteToConsole(string data);
    }
}
