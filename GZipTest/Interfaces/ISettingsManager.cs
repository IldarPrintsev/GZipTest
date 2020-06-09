namespace GZipTest.Interfaces
{
    /// <summary>
    ///     Gets system info and application settings.
    /// </summary>
    public interface ISettingsManager
    {
        /// <summary>
        ///     Gets the count of threads.
        /// </summary>
        int ThreadCount { get; }

        /// <summary>
        ///     Gets the buffer size of a stream.
        /// </summary>
        int BufferSize { get; }

        /// <summary>
        ///     Gets the file block size.
        /// </summary>
        int BlockSize { get; }

        /// <summary>
        ///     Gets the count of blocks.
        /// </summary>
        int MaxBlockCount { get; }
    }
}
