using GZipTest.Interfaces;
using System;

namespace GZipTest.Managers
{
    /// <summary>
    ///     Gets system info and application settings.
    /// </summary>
    public class SettingsManager : ISettingsManager
    {
        private readonly int _threadCount;
        private readonly int _bufferSize;
        private readonly int _blockSize;
        private readonly int _maxBlockCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SettingsManager"/> class.
        /// </summary>
        public SettingsManager()
        {
            this._threadCount = Environment.ProcessorCount;
            this._bufferSize = 80 * 1024;
            this._blockSize = 1024 * 1024;
            this._maxBlockCount = _threadCount * 10;
        }

        /// <summary>
        ///     Gets the count of threads.
        /// </summary>
        public int ThreadCount => this._threadCount;

        /// <summary>
        ///     Gets the buffer size of a stream.
        /// </summary>
        public int BufferSize => this._bufferSize;

        /// <summary>
        ///     Gets the file block size.
        /// </summary>
        public int BlockSize => this._blockSize;

        /// <summary>
        ///     Gets the count of blocks.
        /// </summary>
        public int MaxBlockCount => this._maxBlockCount;
    }
}
