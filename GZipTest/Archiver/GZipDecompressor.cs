using GZipTest.Interfaces;
using System;

namespace GZipTest.Archiver
{
    /// <summary>
    ///     Represents the GZip archiver. Provides the decompression of a file. 
    /// </summary>
    public sealed class GZipDecompressor : GZipArchiver
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GZipDecompressor"/> class.
        /// </summary>
        /// <param name="archiveManager">Provides functionality to compress/decompress files.</param>
        public GZipDecompressor(IArchiveManager archiveManager) : base(archiveManager) { }

        /// <summary>
        ///     Runs the decompression of a file.
        /// </summary>
        /// <param name="inputFilePath">The path of an input compressed file.</param>
        /// <param name="outputFilePath">The path of an output original file.</param>
        public override void Run(string inputFilePath, string outputFilePath)
        {
            Console.WriteLine("File decompression is processing...");
            base._archiveManager.DecompressFile(inputFilePath, outputFilePath);
        }
    }
}
