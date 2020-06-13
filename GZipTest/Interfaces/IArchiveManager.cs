using GZipTest.Common;

namespace GZipTest.Interfaces
{
    /// <summary>
    ///     Defines the methods for an archive manager.
    /// </summary>
    public interface IArchiveManager
    {
        /// <summary>
        ///     Executes the selected archive operation.
        /// </summary>
        /// <param name="operationType">The type of archive operation.</param>
        /// <param name="inputFilePath">The path of an input file.</param>
        /// <param name="outputFilePath">The path of an output compressed file.</param>
        void Execute(OperationType operationType, string inputFilePath, string outputFilePath);
    }
}
