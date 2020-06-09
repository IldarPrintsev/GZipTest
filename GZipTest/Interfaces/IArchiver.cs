namespace GZipTest.Interfaces
{
    /// <summary>
    ///     Defines the methods of an archiver.
    /// </summary>
    public interface IArchiver
    {
        /// <summary>
        ///     Runs the compression/decompression of a file.
        /// </summary>
        /// <param name="inputFilePath">The path of an input file.</param>
        /// <param name="outputFilePath">The path of an output file.</param>
        void Run(string inputFilePath, string outputFilePath);
    }
}
