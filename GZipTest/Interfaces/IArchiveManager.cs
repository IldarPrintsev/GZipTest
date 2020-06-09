namespace GZipTest.Interfaces
{
    /// <summary>
    ///     Defines the methods for an archive manager.
    /// </summary>
    public interface IArchiveManager
    {
        /// <summary>
        ///     Compresses the file.
        /// </summary>
        /// <param name="inputFilePath">The path of an input file.</param>
        /// <param name="outputFilePath">The path of an output compressed file.</param>
        void CompressFile(string inputFilePath, string outputFilePath);

        /// <summary>
        ///     Decompresses the file.
        /// </summary>
        /// <param name="inputFilePath">The path of an input compressed file.</param>
        /// <param name="outputFilePath">The path of an output original file.</param>
        void DecompressFile(string inputFilePath, string outputFilePath);
    }
}
