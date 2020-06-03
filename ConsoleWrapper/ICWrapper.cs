using ConsoleWrapper.Settings;
using System;
using System.Diagnostics;

namespace ConsoleWrapper
{
    public interface ICWrapper : IDisposable
    {
        #region Properties

        /// <summary>
        /// Indicates whether or not this <see cref="ICWrapper"/> is disposed
        /// </summary>
        bool Disposed { get; }

        /// <summary>
        /// Indicates whether the executable is running
        /// </summary>
        bool Executing { get; }

        /// <summary>
        /// The location of the executable
        /// </summary>
        string Executable { get; }

        WrapperSettings Settings { get; }

        #endregion

        #region Events

        /// <summary>
        /// Invoked when output data is received from the executable
        /// </summary>
        event EventHandler<DataReceivedEventArgs> OutputDataReceived;

        /// <summary>
        /// Invoked when error data is received from the executable
        /// </summary>
        event EventHandler<DataReceivedEventArgs> ErrorDataReceived;

        /// <summary>
        /// Invoked when the executable exits
        /// </summary>
        event EventHandler<DateTime> Exited;

        /// <summary>
        /// Invoked when the executable is killed by the <see cref="ICWrapper"/>
        /// </summary>
        event EventHandler Killed;

        #endregion

        /// <summary>
        /// Executes the console application
        /// </summary>
        /// <param name="startArgs">The arguments passed when starting the console application</param>
        void Execute(string startArgs);

        /// <summary>
        /// Immediately kills the running executable. Only use this if you are certain the executable is in a state where it can safely shutdown
        /// </summary>
        void Kill();

        /// <summary>
        /// Writes data to the standard input stream of the executable, provided <see cref="WrapperSettings.RedirectStandardInput"/> is true
        /// </summary>
        /// <param name="data">The data to write to the console</param>
        void WriteToConsole(string data);
    }
}
