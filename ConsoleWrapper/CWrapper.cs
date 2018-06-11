using System;
using System.Diagnostics;
using System.IO;
using ConsoleWrapper.Settings;

namespace ConsoleWrapper
{
    public class CWrapper : ICWrapper
    {
        #region Events

        /// <summary>
        /// Invoked when output data is received from the console application
        /// </summary>
        public event DataReceivedEventHandler OutputDataReceived;

        /// <summary>
        /// Invoked when error data is received from the console application
        /// </summary>
        public event DataReceivedEventHandler ErrorDataReceived;

        /// <summary>
        /// Invoked when the console application is exited
        /// </summary>
        public event EventHandler Exited;

        public event EventHandler Killed;

        #endregion

        #region Privates

        private readonly Process _wrappedProcess;
        private readonly string _startArgs;

        #endregion

        #region Publics

        /// <summary>
        /// Indicates whether or not this CWrapper instance is disposed
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// Indicates whether or not this CWrapper instance is executing
        /// </summary>
        public bool Executing { get; protected set; }

        /// <summary>
        /// The location of the executable that this CWrapper instance is using
        /// </summary>
        public string ExecutableLocation { get; protected set; }

        /// <summary>
        /// The settings used by this CWrapper instance
        /// </summary>
        public WrapperSettings Settings { get; protected set; }

        /*/// <summary>
        /// The standard error stream for the console application
        /// </summary>
        public StreamReader StandardError
        {
            get => _wrappedProcess.StandardError;
        }

        /// <summary>
        /// The standard input stream for the console application
        /// </summary>
        public StreamWriter StandardInput
        {
            get => _wrappedProcess.StandardInput;
        }

        /// <summary>
        /// The standard output stream for the console application
        /// </summary>
        public StreamReader StandardOutput
        {
            get => _wrappedProcess.StandardOutput;
        }*/

        #endregion

        #region Ctors

        public CWrapper(string executableLocation)
            : this (executableLocation, String.Empty) { }

        public CWrapper(string executableLocation, string startArgs)
            : this(executableLocation, startArgs, new WrapperSettings()) { }

        public CWrapper(string executableLocation, string startArgs, WrapperSettings settings)
        {
            if (!File.Exists(executableLocation))
                throw new ArgumentException("No executable exists at the specified location!", nameof(executableLocation));

            ExecutableLocation = executableLocation;
            Settings = settings ?? new WrapperSettings();
            Settings.Lock();
            _startArgs = startArgs;

            Executing = false;
            Disposed = false;

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = ExecutableLocation,
                Arguments = _startArgs,
                UseShellExecute = false,
                CreateNoWindow = Settings.CreateNoWindow,
                RedirectStandardError = Settings.RedirectStandardError,
                RedirectStandardInput = Settings.RedirectStandardInput,
                RedirectStandardOutput = Settings.RedirectStandardOutput,
                WorkingDirectory = Settings.WorkingDirectory,
                StandardErrorEncoding = Settings.EncodingSettings.StandardErrorEncoding,
                StandardOutputEncoding = Settings.EncodingSettings.StandardOutputEncoding
            };

            _wrappedProcess = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            _wrappedProcess.OutputDataReceived += (s, e) => OutputDataReceived?.Invoke(s, e);
            _wrappedProcess.ErrorDataReceived += (s, e) => ErrorDataReceived?.Invoke(s, e);
            _wrappedProcess.Exited += (s, e) =>
            {
                Executing = false;
                Exited?.Invoke(s, e);
            };
        }
        #endregion

        /// <summary>
        /// Executes the console application
        /// </summary>
        public void Execute()
        {
            CheckDisposed();
            if (Executing)
                throw new InvalidOperationException("This CWrapper instance is already executing!");

            _wrappedProcess.Start();

            _wrappedProcess.BeginErrorReadLine();
            _wrappedProcess.BeginOutputReadLine();

            AppDomain.CurrentDomain.DomainUnload += (s, e) =>
            {
                if (Executing)
                {
                    _wrappedProcess.Kill();
                    _wrappedProcess.WaitForExit();
                }
            };
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                if (Executing)
                {
                    _wrappedProcess.Kill();
                    _wrappedProcess.WaitForExit();
                }
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (Executing)
                {
                    _wrappedProcess.Kill();
                    _wrappedProcess.WaitForExit();
                }
            };

            Executing = true;
        }

        /// <summary>
        /// Kills the console application that this CWrapper instance is executing
        /// </summary>
        public void Kill()
        {
            CheckDisposed();
            if (!Executing)
                throw new InvalidOperationException("This CWrapper instance is not executing!");

            Executing = false;
            _wrappedProcess.CancelErrorRead();
            _wrappedProcess.CancelOutputRead();
            _wrappedProcess.Kill();
            _wrappedProcess.WaitForExit();

            Killed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Writes data to the console application that this CWrapper instance is executing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data to write to the console</param>
        public void WriteToConsole(string data)
        {
            CheckDisposed();
            if (!Executing)
                throw new InvalidOperationException("This CWrapper instance is not executing!");

            byte[] byteData = Settings.EncodingSettings.StandardInputEncoding.GetBytes(data);
            //_wrappedProcess.StandardInput.Write(data);
            _wrappedProcess.StandardInput.BaseStream.Write(byteData, 0, byteData.Length);
        }

        /// <summary>
        /// Tries to kill this CWrapper instance
        /// </summary>
        /// <returns>True if the process was killed</returns>
        public bool TryKill()
        {
            if (!Disposed && Executing)
            {
                Executing = false;
                _wrappedProcess.Kill();
                _wrappedProcess.WaitForExit();
                return true;
            } else
            {
                return false;
            }
        }

        /// <summary>
        /// Disposes of this CWrapper instance
        /// </summary>
        /// <param name="killChild">If true, will kill the console application that this CWrapper instance is executing</param>
        /*public void Dispose(bool killChild)
        {
            CheckDisposed();
            if (killChild)
                TryKill();
            Dispose();
        }*/

        /// <summary>
        /// Disposes of this CWrapper instance
        /// </summary>
        public void Dispose()
        {
            CheckDisposed();
            TryKill();

            ErrorDataReceived = null;
            Exited = null;
            Killed = null;
            OutputDataReceived = null;

            _wrappedProcess.Dispose();
            Disposed = true;
        }

        private void CheckDisposed()
        {
            if (Disposed)
                throw new InvalidOperationException("This CWrapper instance is disposed!");
        }
    }
}
