using GZipTest.Helpers;
using GZipTest.Interfaces;
using GZipTest.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace GZipTest.Workers
{
    /// <summary>
    ///     Represents a stream compression worker.
    /// </summary>
    public sealed class CompressionWorker : BaseWorker
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CompressionWorker"/> class.
        /// </summary>
        /// <param name="settingsManager">Gets system info.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
        public CompressionWorker(ISettingsManager settingsManager, Stream inputStream, Stream outputStream)
            : base(settingsManager, inputStream, outputStream){}

        /// <summary>
        ///     Runs the compression.
        /// </summary>
        public override void Run()
        {
            var fileInfo = new FileInformation(_inputStream.Length, _settingsManager.BlockSize);

            var readThread = new Thread(() => base.ReadFile(fileInfo));
            readThread.Start();

            var threads = new List<Thread>(_settingsManager.ThreadCount);
            for (int i = 0; i < _settingsManager.ThreadCount; i++)
            {
                var compressThread = new Thread(Compress);
                threads.Add(compressThread);

                compressThread.Start();
            }

            var writeThread = new Thread(() => base.WriteFile(fileInfo));
            writeThread.Start();

            readThread.Join();

            foreach (var thread in threads)
            {
                thread.Join();
            }

            _outputQueue.Finish();

            writeThread.Join();
        }

        /// <summary>
        ///     Reads a block.
        /// </summary>
        /// <param name="id">The block's ID.</param>
        /// <param name="fileInfo">File information.</param>
        protected override Block ReadBlock(int id, FileInformation fileInfo)
        {
            var inputBlockStream = new MemoryStream(); 

            CopyBlock(_inputStream, inputBlockStream, fileInfo.BlockSize);
            inputBlockStream.Position = 0;

            return new Block(id, inputBlockStream);
        }

        /// <summary>
        ///     Adds file header info.
        /// </summary>
        /// <param name="fileInfo">File information.</param>
        protected override void DefineFileHeader(FileInformation fileInfo)
        {
            this.WriteInt64(fileInfo.FileLength);
            this.WriteInt32(fileInfo.BlockSize);
        }

        /// <summary>
        ///     Adds block header info.
        /// </summary>
        /// <param name="block">The block data.</param>
        /// <param name="fileInfo">File information.</param>
        protected override void DefineBlockHeader(Block block, FileInformation fileInfo)
        {
            this.WriteInt32(block.Id);
            this.WriteInt32((int)block.StreamContent.Length);
        }

        /// <summary>
        ///     Compresses the input blocks.
        /// </summary>
        private void Compress()
        {
            try
            {
                foreach (var inputBlock in _inputQueue.GetBlocks())
                {
                    var compressStream = new MemoryStream();

                    using (var gZipStream = new GZipStream(compressStream, CompressionMode.Compress, true))
                    {
                        CopyStream(inputBlock.StreamContent, gZipStream);
                    }

                    compressStream.Position = 0;

                    inputBlock.StreamContent.Dispose();

                    _outputQueue.Enqueue(new Block(inputBlock.Id, compressStream), Timeout.InfiniteTimeSpan);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        ///     Writes the Int32 value into a header.
        /// </summary>
        private void WriteInt32(int value)
        {
            ByteConverter.WriteBytes(value, _buffer);
            this._outputStream.Write(_buffer, 0, sizeof(int));
        }

        /// <summary>
        ///     Writes the Int64 value into a header.
        /// </summary>
        private void WriteInt64(long value)
        {
            ByteConverter.WriteBytes(value, _buffer);
            this._outputStream.Write(_buffer, 0, sizeof(long));
        }
    }
}