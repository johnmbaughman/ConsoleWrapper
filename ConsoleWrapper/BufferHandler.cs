using System;
using System.IO;

namespace ConsoleWrapper
{
    public class BufferHandler : IDisposable
    {
        private readonly MemoryStream _outputDataStream;
        private readonly MemoryStream _errorDataStream;

        private bool _isDisposed;

        internal StreamWriter OutputDataWriter;
        internal StreamWriter ErrorDataWriter;

        /// <summary>
        /// Reads data from the output stream
        /// </summary>
        public StreamReader OutputDataReader { get; protected set; }

        /// <summary>
        /// Reads data from the error stream
        /// </summary>
        public StreamReader ErrorDataReader { get; protected set; }

        /// <summary>
        /// Stores data obtained from a wrapper process
        /// </summary>
        public BufferHandler()
        {
            _outputDataStream = new MemoryStream();
            _errorDataStream = new MemoryStream();

            OutputDataWriter = new StreamWriter(_outputDataStream);
            ErrorDataWriter = new StreamWriter(_errorDataStream);

            OutputDataReader = new StreamReader(_outputDataStream);
            ErrorDataReader = new StreamReader(_errorDataStream);
        }

        /// <summary>
        /// Release all resources used by this instance of the <see cref="BufferHandler"/> class
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    OutputDataReader.Dispose();
                    ErrorDataReader.Dispose();
                    OutputDataWriter.Dispose();
                    ErrorDataWriter.Dispose();
                    _outputDataStream.Dispose();
                    _errorDataStream.Dispose();
                }

                _isDisposed = true;
            }
        }
    }
}
