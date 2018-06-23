using System.IO;

namespace ConsoleWrapper
{
    public class BufferHandler
    {
        #region Privates

        private readonly MemoryStream _outputDataStream;
        private readonly MemoryStream _errorDataStream;

        #endregion

        #region Publics

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
    }
}
