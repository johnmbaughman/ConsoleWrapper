﻿using System;
using System.IO;

namespace ConsoleWrapper
{
    public class BufferHandler : IDisposable
    {
        #region Fields

        private readonly MemoryStream _outputDataStream;
        private readonly MemoryStream _errorDataStream;

        private bool _isDisposed;

        #endregion

        #region Properties

        /// <summary>
        /// Reads data from the output stream
        /// </summary>
        public StreamReader OutputDataReader;

        /// <summary>
        /// Reads data from the error stream
        /// </summary>
        public StreamReader ErrorDataReader;

        #endregion

        /// <summary>
        /// Stores data obtained from a wrapper process
        /// </summary>
        /// <param name="outputDataWriter">Used to write to the output data stream</param>
        /// <param name="errorDataWriter">Used to write to the error data stream</param>
        public BufferHandler(out StreamWriter outputDataWriter, out StreamWriter errorDataWriter)
        {
            _outputDataStream = new MemoryStream();
            _errorDataStream = new MemoryStream();

            outputDataWriter = new StreamWriter(_outputDataStream);
            errorDataWriter = new StreamWriter(_errorDataStream);

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
                    _outputDataStream.Dispose();
                    _errorDataStream.Dispose();
                }

                _isDisposed = true;
            }
        }
    }
}
