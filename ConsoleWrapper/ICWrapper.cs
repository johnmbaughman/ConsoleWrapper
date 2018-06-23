using ConsoleWrapper.Settings;
using System;
using System.Diagnostics;

namespace ConsoleWrapper
{
    public interface ICWrapper : IDisposable
    {
        event EventHandler<DataReceivedEventArgs> OutputDataReceived;
        event EventHandler<DataReceivedEventArgs> ErrorDataReceived;
        event EventHandler<DateTime> Exited;
        event EventHandler Killed;

        bool Disposed { get; }
        bool Executing { get; }
        string ExecutableLocation { get; }
        WrapperSettings Settings { get; }
        BufferHandler BufferHandler { get; }

        void Execute(string startArgs);
        void Kill();
        bool TryKill();
        void WriteToConsole(string data);
    }
}
