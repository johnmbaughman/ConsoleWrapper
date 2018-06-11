﻿using ConsoleWrapper.Settings;
using System;
using System.Diagnostics;

namespace ConsoleWrapper
{
    public interface ICWrapper : IDisposable
    {
        event DataReceivedEventHandler OutputDataReceived;
        event DataReceivedEventHandler ErrorDataReceived;
        event EventHandler Exited;
        event EventHandler Killed;

        bool Disposed { get; }
        bool Executing { get; }
        string ExecutableLocation { get; }
        WrapperSettings Settings { get; }

        void Execute();
        void Kill();
        void TryKill();
        void WriteToConsole(string data);
    }
}
