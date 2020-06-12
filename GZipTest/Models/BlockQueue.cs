using GZipTest.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace GZipTest.Models
{
    /// <summary>
    ///     Represents a queue of file blocks.
    /// </summary>
    public sealed class BlockQueue : IQueue<Block>
    {
        private const int Finished = 1;

        private readonly ConcurrentQueue<Block> _queue;

        private readonly ManualResetEvent _complete;
        private readonly Semaphore _enqueueSemaphore;
        private readonly Semaphore _dequeueSemaphore;
        private int _finishFlag;

        private readonly WaitHandle[] _enqueueWaitHandlers;
        private readonly WaitHandle[] _dequeWaitHandlers;

        /// <summary>
        ///     Initializes a new instance of the <see cref="BlockQueue"/> class.
        /// </summary>
        /// <param name="maxBlockCount">Max block count into the queue.</param>
        public BlockQueue(int maxBlockCount)
        {
            this._queue = new ConcurrentQueue<Block>();
            this._complete = new ManualResetEvent(false);
            this._enqueueSemaphore = new Semaphore(maxBlockCount, maxBlockCount);
            this._dequeueSemaphore = new Semaphore(0, maxBlockCount);
            this._finishFlag = 0;
            this._enqueueWaitHandlers = new WaitHandle[] {this._enqueueSemaphore, this._complete };
            this._dequeWaitHandlers = new WaitHandle[] { this._dequeueSemaphore, this._complete };
        }

        private bool IsFinished => _finishFlag == Finished;

        /// <summary>
        ///     Adds a new block.
        /// </summary>
        /// <param name="block">New block.</param>
        /// <param name="timeout">A time interval.</param>
        public bool Enqueue(Block block, TimeSpan timeout)
        {
            if (IsFinished)
            {
                throw new InvalidOperationException("The queue is closed.");
            }

            int waitHandlerResult = WaitHandle.WaitAny(_enqueueWaitHandlers, timeout);
            if (waitHandlerResult == WaitHandle.WaitTimeout)
            {
                return false;
            }

            if (waitHandlerResult != 0)
            {
                throw new InvalidOperationException("CompleteAdding during Enqueue is not supported.");
            }

            _queue.Enqueue(block);

            _dequeueSemaphore.Release();

            return true;
        }

        /// <summary>
        ///     Gets all blocks.
        /// </summary>
        public IEnumerable<Block> GetBlocks()
        {
            while (!IsFinished)
            {
                if (this.Dequeue(out var value, Timeout.InfiniteTimeSpan))
                {
                    yield return value;
                }
            }

            while (_queue.TryDequeue(out var value))
            {
                yield return value;
            }
        }

        /// <summary>
        ///     Finishes all operations with the queue.
        /// </summary>
        public void Finish()
        {
            Interlocked.Exchange(ref _finishFlag, Finished);

            this._complete.Set();
        }

        /// <summary>
        ///     Gets a block.
        /// </summary>
        /// <param name="block">A block.</param>
        /// <param name="timeout">A time interval.</param>
        private bool Dequeue(out Block block, TimeSpan timeout)
        {
            block = null;

            if (IsFinished)
            {
                return false;
            }

            var waitHandlerResult = WaitHandle.WaitAny(_dequeWaitHandlers, timeout);
            if (IsFinished || waitHandlerResult != 0)
            {
                return false;
            }

            if (!_queue.TryDequeue(out block))
            {
                return false;
            }

            _enqueueSemaphore.Release();

            return true;
        }
    }
}