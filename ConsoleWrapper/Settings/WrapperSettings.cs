﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleWrapper.Settings
{
    public class WrapperSettings : SettingsBase
    {
        private bool _redirectStandardError;
        private bool _redirectStandardInput;
        private bool _redirectStandardOutput;
        private string _workingDirectory;
        private EncodingSettings _encodingSettings;

        /// <summary>
        /// Defines whether the standard input stream of the console app should be redirected
        /// </summary>
        public bool RedirectStandardError
        {
            get => _redirectStandardError;
            set => SetValue(value, ref _redirectStandardError);
        }

        /// <summary>
        /// Defines whether the standard input stream of the console app should be redirected
        /// </summary>
        public bool RedirectStandardInput
        {
            get => _redirectStandardInput;
            set => SetValue(value, ref _redirectStandardInput);
        }

        /// <summary>
        /// Defines whether the standard output stream of the console app should be redirected
        /// </summary>
        public bool RedirectStandardOutput
        {
            get => _redirectStandardOutput;
            set => SetValue(value, ref _redirectStandardOutput);
        }

        /// <summary>
        /// Gets or sets the working directory of the console app to be started
        /// </summary>
        public string WorkingDirectory
        {
            get => _workingDirectory;
            set => SetValue(value, ref _workingDirectory);
        }

        /// <summary>
        /// Gets or sets the encoding settings for the console app to be started
        /// </summary>
        public EncodingSettings EncodingSettings
        {
            get => _encodingSettings;
            set => SetValue(value, ref _encodingSettings);
        }

        /// <summary>
        /// Settings for the console wrapper
        /// </summary>
        public WrapperSettings()
        {
            RedirectStandardError = true;
            RedirectStandardInput = true;
            RedirectStandardOutput = true;
            WorkingDirectory = Environment.CurrentDirectory;
            EncodingSettings = new EncodingSettings();
        }
    }
}