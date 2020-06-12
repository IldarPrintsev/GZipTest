using System;
using System.Collections.Generic;

namespace GZipTest.Interfaces
{
    /// <summary>
    ///     Provides methods for a queue.
    /// </summary>
    public interface IQueue<T>
    {
        /// <summary>
        ///     Adds a new block.
        /// </summary>
        /// <param name="item">New item.</param>
        /// <param name="timeout">A time interval.</param>
        bool Enqueue(T item, TimeSpan timeout);

        /// <summary>
        ///     Gets all blocks.
        /// </summary>
        IEnumerable<T> GetBlocks();

        /// <summary>
        ///     Finishes all operations with the queue.
        /// </summary>
        void Finish();
    }
}