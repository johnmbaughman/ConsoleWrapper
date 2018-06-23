using System;
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
        public event EventHandler<DataReceivedEventArgs> OutputDataReceived;

        /// <summary>
        /// Set when data is received from the console application
        /// </summary>
        public ManualResetEventSlim OutputDataMRE = new ManualResetEventSlim(false);

        /// <summary>
        /// Invoked when error data is received from the console application
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> ErrorDataReceived;

        /// <summary>
        /// Set when error data is received from the console application
        /// </summary>
        public ManualResetEventSlim ErrorDataMRE = new ManualResetEventSlim(false);

        /// <summary>
        /// Invoked when the console application is exited
        /// </summary>
        public event EventHandler<DateTime> Exited;

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
        private StreamWriter _outputDataWriter;
        private StreamWriter _errorDataWriter;

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

        /// <summary>
        /// Stores output and error data from the wrapper process
        /// </summary>
        public BufferHandler BufferHandler { get; protected set; }

        /// <summary>
        /// Shortcut to <see cref="BufferHandler.OutputDataReader"/>
        /// </summary>
        public StreamReader OutputBuffer => BufferHandler.OutputDataReader;

        /// <summary>
        /// Shortcut to <see cref="BufferHandler.ErrorDataReader"/>
        /// </summary>
        public StreamReader ErrorBuffer => BufferHandler.ErrorDataReader;

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
                WorkingDirectory = Settings.WorkingDirectory
            };

            if (Settings.RedirectStandardOutput)
                startInfo.StandardOutputEncoding = Settings.EncodingSettings.StandardOutputEncoding;
            if (Settings.RedirectStandardError)
                startInfo.StandardErrorEncoding = Settings.EncodingSettings.StandardErrorEncoding;

            BufferHandler = new BufferHandler(out _outputDataWriter, out _errorDataWriter);

            _wrappedProcess = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            _wrappedProcess.OutputDataReceived += (s, e) =>
            {
                OutputDataReceived?.Invoke(s, e);
                _outputDataWriter.WriteLine(e.Data);
                OutputDataMRE.Set();
            };
            _wrappedProcess.ErrorDataReceived += (s, e) =>
            {
                ErrorDataReceived?.Invoke(s, e);
                _errorDataWriter.WriteLine(e.Data);
                ErrorDataMRE.Set();
            };
            _wrappedProcess.Exited += (s, e) =>
            {
                Executing = false;
                Exited?.Invoke(s, DateTime.Now);
                ExitedMRE.Set();
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

            if (Settings.RedirectStandardError)
                _wrappedProcess.BeginErrorReadLine();
            if (Settings.RedirectStandardOutput)
                _wrappedProcess.BeginOutputReadLine();

            AppDomain.CurrentDomain.DomainUnload += (s, e) => Kill();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Kill();
            AppDomain.CurrentDomain.UnhandledException += (s, e) => Kill();

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
        /// Tries to kill this CWrapper instance
        /// </summary>
        /// <returns>True if the process was killed</returns>
        public bool TryKill()
        {
            if (!Disposed && Executing)
            {
                try
                {
                    Executing = false;
                    _wrappedProcess.CancelErrorRead();
                    _wrappedProcess.CancelOutputRead();
                    _wrappedProcess.Kill();
                    _wrappedProcess.WaitForExit();

                    KilledMRE.Set();
                    Killed?.Invoke(this, EventArgs.Empty);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
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

            //byte[] byteData = Settings.EncodingSettings.StandardInputEncoding.GetBytes(data);
            //_wrappedProcess.StandardInput.BaseStream.Write(byteData, 0, byteData.Length);
            _wrappedProcess.StandardInput.WriteLine(data);
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="CWrapper"/> class
        /// </summary>
        /// <param name="killProcess">If true, the dispose method will try to kill the wrapped process</param>
        /// <param name="killSuccess">True if the wrapper process was killed successfully</param>
        public void Dispose(bool killProcess, out bool killSuccess)
        {
            CheckDisposed();

            if (killProcess)
                killSuccess = TryKill();
            else
                killSuccess = false;

            _wrappedProcess.Dispose();
            OutputDataMRE.Dispose();
            ErrorDataMRE.Dispose();
            ExitedMRE.Dispose();
            KilledMRE.Dispose();

            _outputDataWriter.Dispose();
            _errorDataWriter.Dispose();
            BufferHandler.Dispose();

            Disposed = true;
        }

        /// <summary>
        /// Releases all resources used by the current instance of the <see cref="CWrapper"/> class
        /// </summary>
        /// <remarks>This will kill the wrapped process</remarks>
        public void Dispose() => Dispose(true, out bool success);
        /*{
            CheckDisposed();
            TryKill();

            ErrorDataReceived = null;
            Exited = null;
            Killed = null;
            OutputDataReceived = null;

            _wrappedProcess.Dispose();
            Disposed = true;
        }*/

        private void CheckDisposed()
        {
            if (Disposed)
                throw new InvalidOperationException("This CWrapper instance is disposed!");
        }
    }
}
