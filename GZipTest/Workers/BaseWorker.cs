using GZipTest.Interfaces;
using GZipTest.Exceptions;
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

        protected readonly IQueue<Block> _inputQueue;
        protected readonly IQueue<Block> _outputQueue;

        protected readonly byte[] _buffer = new byte[sizeof(long)];

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseWorker"/> class.
        /// </summary>
        /// <param name="settingsManager">Gets system info.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
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
            catch(IOException ex)
            {
                this.Finish();
                throw new IOException($"A stream reading error occurs during file reading: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                this.Finish();
                throw new ArchiverException($"An archiver error occurs during file reading: {ex.Message}", ex);
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
            catch (IOException ex)
            {
                this.Finish();
                throw new IOException($"A stream writing error occurs during file writing: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                this.Finish();
                throw new ArchiverException($"An archiver error occurs during file writing: {ex.Message}", ex);
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
            int remainingBytes = blockSize;
            var buffer = new byte[_settingsManager.BufferSize];
            while (remainingBytes > 0)
            {
                var bytesToRead = Math.Min(buffer.Length, remainingBytes);
                var count = inputStream.Read(buffer, offset: 0, count: bytesToRead);

                if (count == 0)
                {
                    return;
                }

                outputStream.Write(buffer, offset: 0, count: count);

                remainingBytes -= count;
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
        ///     Finishes all operations with the queues.
        /// </summary>
        protected void Finish()
        {
            this._inputQueue.Finish();
            this._outputQueue.Finish();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Finish();

                (this._inputQueue as IDisposable)?.Dispose();
                (this._outputQueue as IDisposable)?.Dispose();
            }
        }
    }
}