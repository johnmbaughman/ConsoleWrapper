using System;
using System.Diagnostics;
using ConsoleWrapper.Settings;

namespace ConsoleWrapper
{
    public class CWrapper
    {
        private readonly Process _wrappedProcess;

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

            _wrappedProcess = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = ExecutableLocation,
                Arguments = startArgs,
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
