using System;

namespace ConsoleWrapper.Settings
{
    public class WrapperSettings : SettingsBase
    {
        private bool _redirectStandardError;
        private bool _redirectStandardInput;
        private bool _redirectStandardOutput;
        private bool _showWindow;
        private string _workingDirectory;
        private EncodingSettings _encodingSettings;

        /// <summary>
        /// Defines whether the standard error stream of the executable should be redirected to the <see cref="BufferHandler"/>
        /// </summary>
        public bool RedirectStandardError
        {
            get => _redirectStandardError;
            set => SetProperty(ref _redirectStandardError, value);
        }

        /// <summary>
        /// Defines whether the standard input stream of the executable should be redirected to the <see cref="ICWrapper"/>
        /// </summary>
        public bool RedirectStandardInput
        {
            get => _redirectStandardInput;
            set => SetProperty(ref _redirectStandardInput, value);
        }

        /// <summary>
        /// Defines whether the standard output stream of the executable should be redirected to the <see cref="BufferHandler"/>
        /// </summary>
        public bool RedirectStandardOutput
        {
            get => _redirectStandardOutput;
            set => SetProperty(ref _redirectStandardOutput, value);
        }

        /// <summary>
        /// Defines whether the console app should show a window.
        /// </summary>
        public bool ShowWindow
        {
            get => _showWindow;
            set => SetProperty(ref _showWindow, value);
        }

        /// <summary>
        /// Gets or sets the working directory of the executable
        /// </summary>
        public string WorkingDirectory
        {
            get => _workingDirectory;
            set => SetProperty(ref _workingDirectory, value);
        }

        /// <summary>
        /// Gets or sets the encoding settings to be used
        /// </summary>
        public EncodingSettings EncodingSettings
        {
            get => _encodingSettings;
            set => SetProperty(ref _encodingSettings, value);
        }

        /// <summary>
        /// Settings for the console wrapper
        /// </summary>
        public WrapperSettings()
        {
            RedirectStandardError = true;
            RedirectStandardInput = true;
            RedirectStandardOutput = true;
            ShowWindow = false;
            WorkingDirectory = Environment.CurrentDirectory;
            EncodingSettings = new EncodingSettings();
        }
    }
}
