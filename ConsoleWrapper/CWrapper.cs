using System;
using System.Diagnostics;
using ConsoleWrapper.Settings;

namespace ConsoleWrapper
{
    public class CWrapper
    {
        private readonly Process _wrappedProcess;
        private readonly string _pipeHash;

        public string ExecutableLocation { get; protected set; }
        public WrapperSettings Settings { get; protected set; }

        public CWrapper(string executableLocation)
            : this (executableLocation, String.Empty) { }

        public CWrapper(string executableLocation, string startArgs)
            : this(executableLocation, startArgs, new WrapperSettings()) { }

        public CWrapper(string executableLocation, string startArgs, WrapperSettings settings)
        {
            ExecutableLocation = executableLocation;
            Settings = settings;
            Settings.Lock();

            _pipeHash = Guid.NewGuid().ToString();
            string args = startArgs + "-pipeHash " + _pipeHash;

            _wrappedProcess = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = ExecutableLocation,
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = Settings.CreateNoWindow,
                RedirectStandardError = Settings.RedirectStandardError,
                RedirectStandardInput = Settings.RedirectStandardInput,
                RedirectStandardOutput = Settings.RedirectStandardOutput,
                WorkingDirectory = Settings.WorkingDirectory,
                StandardErrorEncoding = Settings.EncodingSettings.StandardErrorEncoding,
                StandardOutputEncoding = Settings.EncodingSettings.StandardOutputEncoding
            };
        }
    }
}
