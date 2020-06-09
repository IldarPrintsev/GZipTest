using GZipTest.Models;
using System;
using System.Collections.Generic;

namespace GZipTest.Interfaces
{
    /// <summary>
    ///     Provides methods for the queue of file blocks.
    /// </summary>
    public interface IBlockQueue
    {
        /// <summary>
        ///     Adds a new block.
        /// </summary>
        /// <param name="block">New block.</param>
        /// <param name="timeout">A time interval.</param>
        bool Enqueue(Block block, TimeSpan timeout);

        /// <summary>
        ///     Gets all blocks.
        /// </summary>
        IEnumerable<Block> GetBlocks();

        /// <summary>
        ///     Finishes all operations with the queue.
        /// </summary>
        void Finish();
    }
}