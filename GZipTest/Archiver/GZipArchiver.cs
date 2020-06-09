using GZipTest.Interfaces;

namespace GZipTest.Archiver
{
    /// <summary>
    ///     Represents the GZip archiver. 
    /// </summary>
    public abstract class GZipArchiver : IArchiver
    {
        protected readonly IArchiveManager _archiveManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GZipArchiver"/> class.
        /// </summary>
        /// <param name="archiveManager">Provides functionality to compress/decompress files.</param>
        public GZipArchiver(IArchiveManager archiveManager)
        {
            this._archiveManager = archiveManager;
        }
        
        /// <summary>
        ///     Runs the compression/decompression of a file.
        /// </summary>
        /// <param name="inputFilePath">The path of an input file.</param>
        /// <param name="outputFilePath">The path of an output file.</param>
        public abstract void Run(string inputFilePath, string outputFilePath);
    }
}
