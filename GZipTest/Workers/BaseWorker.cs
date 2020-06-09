using GZipTest.Interfaces;
using GZipTest.Models;
using System;
using System.IO;
using System.Threading;

namespace GZipTest.Workers
{
    /// <summary>
    ///     Represents a stream worker.
    /// </summary>
    public abstract class BaseWorker : IWorker, IDisposable
    {
        protected readonly ISettingsManager _settingsManager;
        protected readonly Stream _inputStream;
        protected readonly Stream _outputStream;

        protected readonly IBlockQueue _inputQueue;
        protected readonly IBlockQueue _outputQueue;

        protected readonly byte[] _buffer = new byte[sizeof(long)];

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseWorker"/> class.
        /// </summary>
        /// <param name="settingsManager">Gets system info.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="interruptEvent">Notifies that interrupting event has occurred.</param>
        protected BaseWorker(ISettingsManager settingsManager, Stream inputStream, Stream outputStream)
        {
            this._settingsManager = settingsManager;
            this._inputStream = inputStream;
            this._outputStream = outputStream;

            this._inputQueue = new BlockQueue(settingsManager.MaxBlockCount);
            this._outputQueue = new BlockQueue(settingsManager.MaxBlockCount);
        }

        /// <summary>
        ///     Runs the compression/decompression.
        /// </summary>
        public abstract void Run();

        /// <summary>
        ///     Reads a block.
        /// </summary>
        /// <param name="id">The block's ID.</param>
        /// <param name="fileInfo">File information.</param>
        protected abstract Block ReadBlock(int id, FileInformation fileInfo);

        /// <summary>
        ///     Adds or gets file header info.
        /// </summary>
        /// <param name="fileInfo">File information.</param>
        protected abstract void DefineFileHeader(FileInformation pipelineInfo);

        /// <summary>
        ///     Adds or gets block header info.
        /// </summary>
        /// <param name="block">The block data.</param>
        /// <param name="fileInfo">File information.</param>
        protected abstract void DefineBlockHeader(Block block, FileInformation fileInfo);

        /// <summary>
        ///     Reads an input file.
        /// </summary>
        /// <param name="fileInfo">File information.</param>
        protected void ReadFile(FileInformation fileInfo)
        {
            try
            {
                for (var i = 0; i < fileInfo.BlockCount; i++)
                {
                    _inputQueue.Enqueue(ReadBlock(i, fileInfo), Timeout.InfiniteTimeSpan);
                }
                _inputQueue.Finish();
            }
            catch (Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        ///     Writes output file.
        /// </summary>
        /// <param name="fileInfo">File information.</param>
        protected void WriteFile(FileInformation fileInfo)
        {
            try
            {
                DefineFileHeader(fileInfo);

                foreach (var block in _outputQueue.GetBlocks())
                {
                    DefineBlockHeader(block, fileInfo);

                    CopyStream(block.StreamContent, _outputStream);
                    block.StreamContent.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new IOException(ex.Message);
            }
        }

        /// <summary>
        ///     Copies the block content.
        /// </summary>
        /// <param name="inputStream">Input stream.</param>
        /// <param name="outputStream">Output stream.</param>
        /// <param name="blockSize">The block size.</param>
        protected void CopyBlock(Stream inputStream, Stream outputStream, int blockSize)
        {
            int bytesOutstanding = blockSize;
            var buffer = new byte[_settingsManager.BufferSize];
            while (bytesOutstanding > 0)
            {
                var bytesToRead = Math.Min(buffer.Length, bytesOutstanding);
                var count = inputStream.Read(buffer, offset: 0, count: bytesToRead);

                if (count == 0)
                {
                    return;
                }

                outputStream.Write(buffer, offset: 0, count: count);

                bytesOutstanding -= count;
            }
        }

        /// <summary>
        ///     Copies a stream.
        /// </summary>
        /// <param name="inputStream">Input stream.</param>
        /// <param name="outputStream">Output stream.</param>
        protected void CopyStream(Stream inputStream, Stream outputStream)
        {
            var buffer = new byte[_settingsManager.BufferSize];
            int count;
            while ((count = inputStream.Read(buffer, offset: 0, count: buffer.Length)) != 0)
            {
                outputStream.Write(buffer, offset: 0, count: count);
            }
        }

        /// <summary>
        ///     Copies bytes from a stream to a buffer
        /// </summary>
        /// <param name="inputStream">Input stream.</param>
        /// <param name="buffer">A byte buffer.</param>
        /// <param name="byteCount">The byte count.</param>
        protected void ReadBytes(Stream inputStream, byte[] buffer, int byteCount)
        {
            int readBytesCount = 0;
            int remainingBytesCount = byteCount;
            while (remainingBytesCount > 0)
            {
                var count = inputStream.Read(buffer, readBytesCount, remainingBytesCount);
                if (count == 0)
                {
                    throw new Exception("Input file is unknown.");
                }

                remainingBytesCount -= count;
                readBytesCount += count;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                (_outputQueue as IDisposable)?.Dispose();
                (_inputQueue as IDisposable)?.Dispose();
            }
        }
    }
}