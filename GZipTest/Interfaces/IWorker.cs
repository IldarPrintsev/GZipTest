using System;

namespace GZipTest.Interfaces
{
    /// <summary>
    ///     Provides methods to execute archive operations with file blocks.
    /// </summary>
    public interface IWorker : IDisposable
    {
        /// <summary>
        ///     Runs the worker's instructions.
        /// </summary>
        void Run();
    }
}
