using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ConsoleWrapper.Settings;

namespace ConsoleWrapper
{
    public class CWrapper : ICWrapper
    {
        #region Fields

        private readonly Process _wrappedProcess;
        private readonly StreamWriter _outputDataWriter;
        private readonly StreamWriter _errorDataWriter;

        #endregion

        #region Properties

        /// <summary>
        /// Indicates whether this <see cref="CWrapper"/> is disposed
        /// </summary>
        public bool Disposed { get; protected set; }

        /// <summary>
        /// Indicates whether the executable is running
        /// </summary>
        public bool Executing { get; protected set; }

        /// <summary>
        /// The location of the executable
        /// </summary>
        public string ExecutableLocation { get; protected set; }

        public WrapperSettings Settings { get; protected set; }

        /// <summary>
        /// Stores output and error data from the wrapper process. Depending on the value of <see cref="WrapperSettings.UseBufferHandler"/>, this property may return null
        /// </summary>
        public BufferHandler BufferHandler { get; protected set; }

        #endregion

        #region Events

        /// <summary>
        /// Invoked when output data is received from the executable
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> OutputDataReceived;

        /// <summary>
        /// Set when data is received from the console application
        /// </summary>
        public ManualResetEventSlim OutputDataMRE = new ManualResetEventSlim(false);

        /// <summary>
        /// Invoked when error data is received from the executable
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> ErrorDataReceived;

        /// <summary>
        /// Set when error data is received from the console application
        /// </summary>
        public ManualResetEventSlim ErrorDataMRE = new ManualResetEventSlim(false);

        /// <summary>
        /// Invoked when the executable exits
        /// </summary>
        public event EventHandler<DateTime> Exited;

        /// <summary>
        /// Set when the executable exits
        /// </summary>
        public ManualResetEventSlim ExitedMRE = new ManualResetEventSlim(false);

        /// <summary>
        /// Invoked when the executable is killed by the <see cref="CWrapper"/>
        /// </summary>
        public event EventHandler Killed;

        /// <summary>
        /// Set when the executable is killed by the <see cref="CWrapper"/>
        /// </summary>
        public ManualResetEventSlim KilledMRE = new ManualResetEventSlim(false);

        #endregion

        #region Ctors

        public CWrapper(string executableLocation)
            : this (executableLocation, new WrapperSettings()) { }

        public CWrapper(string executableLocation, WrapperSettings settings)
        {
            if (!File.Exists(executableLocation))
                throw new ArgumentException("No executable exists at the specified location", nameof(executableLocation));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));

            ExecutableLocation = executableLocation;
            Settings.Lock();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = ExecutableLocation,
                UseShellExecute = false,
                CreateNoWindow = Settings.ShowWindow,
                RedirectStandardError = Settings.RedirectStandardError,
                RedirectStandardInput = Settings.RedirectStandardInput,
                RedirectStandardOutput = Settings.RedirectStandardOutput,
                StandardOutputEncoding = Settings.EncodingSettings.StandardOutputEncoding,
                StandardErrorEncoding = Settings.EncodingSettings.StandardErrorEncoding,
                WorkingDirectory = Settings.WorkingDirectory
            };

            if (Settings.UseBufferHandler)
                BufferHandler = new BufferHandler(out _outputDataWriter, out _errorDataWriter);

            _wrappedProcess = new Process
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            _wrappedProcess.OutputDataReceived += (s, e) =>
            {
                OutputDataReceived?.Invoke(s, e);
                if (Settings.UseBufferHandler)
                    _outputDataWriter.WriteLine(e.Data);
                OutputDataMRE.Set();
            };
            _wrappedProcess.ErrorDataReceived += (s, e) =>
            {
                ErrorDataReceived?.Invoke(s, e);
                if (Settings.UseBufferHandler)
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
                throw new InvalidOperationException("This CWrapper instance is already executing");

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
        /// Immediately kills the running executable. Only use this if you are certain the executable is in a state where it can safely shutdown
        /// </summary>
        public void Kill()
        {
            CheckDisposed();
            if (!Executing)
                throw new InvalidOperationException("This CWrapper instance is not executing");

            Executing = false;
            _wrappedProcess.CancelErrorRead();
            _wrappedProcess.CancelOutputRead();
            _wrappedProcess.Kill();
            _wrappedProcess.WaitForExit();

            KilledMRE.Set();
            Killed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Writes data to the standard input stream of the executable, provided <see cref="WrapperSettings.RedirectStandardInput"/> is true
        /// </summary>
        /// <param name="data">The data to write to the console</param>
        public void WriteToConsole(string data)
        {
            CheckDisposed();
            if (!Executing)
                throw new InvalidOperationException("This CWrapper instance is not executing!");
            if (!Settings.RedirectStandardInput)
                throw new InvalidOperationException("Cannot write to standard input. You have not enabled redirection in the " + nameof(WrapperSettings));

            _wrappedProcess.StandardInput.WriteLine(data);
        }

        /// <summary>
        /// Releases all resources and kills the executable
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    if (Executing)
                        Kill();
                    _wrappedProcess.Dispose();

                    OutputDataMRE.Dispose();
                    ErrorDataMRE.Dispose();
                    ExitedMRE.Dispose();
                    KilledMRE.Dispose();

                    _outputDataWriter?.Dispose();
                    _errorDataWriter?.Dispose();
                    BufferHandler?.Dispose();
                }

                Disposed = true;
            }
        }

        private void CheckDisposed()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(CWrapper));
        }
    }
}
