using System;
using System.Diagnostics;
using ConsoleWrapper.Settings;

namespace ConsoleWrapper
{
    public class CWrapper
    {
        private Process _wrappedProcess;
        private string _pipeHash;
        private WrapperSettings _settings;

        public CWrapper(string executableLocation)
        {
            Initialise(executableLocation, null, new WrapperSettings());
        }

        public CWrapper(string executableLocation, string startArgs)
        {
            Initialise(executableLocation, startArgs, new WrapperSettings());
        }

        public CWrapper(string executableLocation, string startArgs, WrapperSettings settings)
        {
            Initialise(executableLocation, startArgs, settings);
        }

        private void Initialise(string location, string args, WrapperSettings settings)
        {
            _pipeHash = Guid.NewGuid().ToString();
            string arguments = args + "-pipeHash " + _pipeHash;

            _wrappedProcess = new Process();

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = location,
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardError = _settings.RedirectStandardError,
                RedirectStandardInput = _settings.RedirectStandardInput,
                RedirectStandardOutput = _settings.RedirectStandardOutput
            };
        }
    }
}
