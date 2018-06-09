using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleWrapper.Settings
{
    public class EncodingSettings : SettingsBase
    {
        private Encoding _stdErrEncoding;
        private Encoding _stdInEncoding;
        private Encoding _stdOutEncoding;

        /// <summary>
        /// The encoding to use for the standard error stream
        /// </summary>
        public Encoding StandardErrorEncoding
        {
            get => _stdErrEncoding;
            set => SetValue(value, ref _stdErrEncoding);
        }

        /// <summary>
        /// The encoding to use for the standard input stream
        /// </summary>
        public Encoding StandardInputEncoding
        {
            get => _stdInEncoding;
            set => SetValue(value, ref _stdInEncoding);
        }

        /// <summary>
        /// The encoding to use for the standard output stream
        /// </summary>
        public Encoding StandardOutputEncoding
        {
            get => _stdOutEncoding;
            set => SetValue(value, ref _stdOutEncoding);
        }

        public EncodingSettings()
        {
            StandardErrorEncoding = Console.OutputEncoding;
            StandardInputEncoding = Console.InputEncoding;
            StandardOutputEncoding = Console.OutputEncoding;
        }

        public EncodingSettings(Encoding standardErrorEncoding, Encoding standardInputEncoding, Encoding standardOutputEncoding)
        {
            StandardErrorEncoding = standardErrorEncoding;
            StandardInputEncoding = standardInputEncoding;
            StandardOutputEncoding = standardOutputEncoding;
        }
    }
}
