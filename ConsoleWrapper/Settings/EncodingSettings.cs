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
            set => SetValue(value, ref _stdErrEncoding);
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
