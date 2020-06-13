using GZipTest.Common;
using GZipTest.Interfaces;
using GZipTest.Workers;
using System.IO;

namespace GZipTest.Managers
{
    /// <summary>
    ///     Provides functionality to compress/decompress files.
    /// </summary>
    public sealed class ArchiveManager : IArchiveManager
    {
        private readonly ISettingsManager _settingsManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArchiveManager"/> class.
        /// </summary>
        /// <param name="settingsManager">Gets system info.</param>
        public ArchiveManager(ISettingsManager settingsManager)
        {
            this._settingsManager = settingsManager;
        }

        /// <summary>
        ///     Executes the selected archive operation.
        /// </summary>
        /// <param name="operationType">The type of archive operation.</param>
        /// <param name="inputFilePath">The path of an input file.</param>
        /// <param name="outputFilePath">The path of an output compressed file.</param>
        public void Execute(OperationType operationType, string inputFilePath, string outputFilePath)
        {
            using (var inputStream = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, _settingsManager.BufferSize))
            {
                using (var outputStream = new FileStream(outputFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, _settingsManager.BufferSize))
                {
                    IWorker worker;
                    using (operationType == OperationType.Compress
                        ? worker = new CompressionWorker(this._settingsManager, inputStream, outputStream)
                        : worker = new DecompressionWorker(this._settingsManager, inputStream, outputStream))
                    {
                        worker.Run();
                    }
                }
            }
        }
    }
}

