using GZipTest.Interfaces;
using System;

namespace GZipTest.Archiver
{
    /// <summary>
    ///     Represents the GZip archiver. Provides the compression of a file. 
    /// </summary>
    public sealed class GZipCompressor : GZipArchiver
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GZipCompressor"/> class.
        /// </summary>
        /// <param name="archiveManager">Provides functionality to compress/decompress files.</param>
        public GZipCompressor(IArchiveManager archiveManager) : base(archiveManager) { }

        /// <summary>
        ///     Runs the compression of a file.
        /// </summary>
        /// <param name="inputFilePath">The path of an input file.</param>
        /// <param name="outputFilePath">The path of an output compressed file.</param>
        public override void Run(string inputFilePath, string outputFilePath)
        {
            Console.WriteLine("\nFile compression is processing...");
            base._archiveManager.CompressFile(inputFilePath, outputFilePath);
        }
    }
}
