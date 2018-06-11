﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
        /// Set when data is received from the console application
        /// </summary>
        public ManualResetEventSlim OutputDataMRE = new ManualResetEventSlim(false);

        /// <summary>
        /// Invoked when error data is received from the console application
        /// </summary>
        public event DataReceivedEventHandler ErrorDataReceived;

        /// <summary>
        /// Set when error data is received from the console application
        /// </summary>
        public ManualResetEventSlim ErrorDataMRE = new ManualResetEventSlim(false);

        /// <summary>
        /// Invoked when the console application is exited
        /// </summary>
        public event EventHandler Exited;

        /// <summary>
        /// Set when the console application exits
        /// </summary>
        public ManualResetEventSlim ExitedMRE = new ManualResetEventSlim(false);

        /// <summary>
        /// Invoked when this CWrapper instance kills the child console app
        /// </summary>
        public event EventHandler Killed;

        /// <summary>
        /// Set when the console application is killed
        /// </summary>
        public ManualResetEventSlim KilledMRE = new ManualResetEventSlim(false);

        #endregion

        #region Privates

        private readonly Process _wrappedProcess;

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
            : this (executableLocation, new WrapperSettings()) { }

        public CWrapper(string executableLocation, WrapperSettings settings)
        {
            if (!File.Exists(executableLocation))
                throw new ArgumentException("No executable exists at the specified location!", nameof(executableLocation));

            ExecutableLocation = executableLocation;
            Settings = settings ?? new WrapperSettings();
            Settings.Lock();

            Executing = false;
            Disposed = false;

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = ExecutableLocation,
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

            _wrappedProcess.OutputDataReceived += (s, e) =>
            {
                OutputDataMRE.Set();
                OutputDataReceived?.Invoke(s, e);
            };
            _wrappedProcess.ErrorDataReceived += (s, e) =>
            {
                ErrorDataMRE.Set();
                ErrorDataReceived?.Invoke(s, e);
            };
            _wrappedProcess.Exited += (s, e) =>
            {
                Executing = false;
                ErrorDataMRE.Set();
                Exited?.Invoke(s, e);
            };
        }
        #endregion

        /// <summary>
        /// Executes the console application
        /// </summary>
        /// <param name="startArgs">The arguments passed when starting the console application</param>
        public void Execute(string startArgs = null)
        {
            CheckDisposed();
            if (Executing)
                throw new InvalidOperationException("This CWrapper instance is already executing!");

            _wrappedProcess.StartInfo.Arguments = startArgs;
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

            KilledMRE.Set();
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
        /// Disposes of this CWrapper instance. Note that this will also kill the child process
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
