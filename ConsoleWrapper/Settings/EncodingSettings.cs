using System;
using System.Text;

namespace ConsoleWrapper.Settings
{
    public class EncodingSettings : SettingsBase
    {
        private Encoding _stdErrEncoding;
        private Encoding _stdOutEncoding;

        /// <summary>
        /// The encoding to use for the standard error stream
        /// </summary>
        public Encoding StandardErrorEncoding
        {
            get => _stdErrEncoding;
            set => SetProperty(ref _stdErrEncoding, value);
        }

        /// <summary>
        /// The encoding to use for the standard output stream
        /// </summary>
        public Encoding StandardOutputEncoding
        {
            get => _stdOutEncoding;
            set => SetProperty(ref _stdOutEncoding, value);
        }

        public EncodingSettings()
            : this(Console.OutputEncoding, Console.OutputEncoding)
        {
        }

        public EncodingSettings(Encoding standardErrorEncoding, Encoding standardOutputEncoding)
        {
            StandardErrorEncoding = standardErrorEncoding;
            StandardOutputEncoding = standardOutputEncoding;
        }
    }
}
