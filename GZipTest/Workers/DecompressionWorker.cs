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
    ///     Represents a stream decompression worker.
    /// </summary>
    public sealed class DecompressionWorker : BaseWorker
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DecompressionWorker"/> class.
        /// </summary>
        /// <param name="settingsManager">Gets system info.</param>
        /// <param name="inputStream">The input stream.</param>
        /// <param name="outputStream">The output stream.</param>
        public DecompressionWorker(ISettingsManager settingsManager, Stream inputStream, Stream outputStream)
            : base(settingsManager, inputStream, outputStream){}

        /// <summary>
        ///     Runs the decompression.
        /// </summary>
        public override void Run()
        {
            var fileLength = ReadInt64();
            if (fileLength < 0)
            {
                throw new Exception("Input file is unknown.");
            }

            var blockSize = ReadInt32();
            if (blockSize <= 0)
            {
                throw new Exception("Input file is unknown.");
            }

            var fileInfo =  new FileInformation(fileLength, blockSize);

            var readThread = new Thread(() => base.ReadFile(fileInfo));
            readThread.Start();

            var threads = new List<Thread>(_settingsManager.ThreadCount);
            for (int i = 0; i < _settingsManager.ThreadCount; i++)
            {
                var decompressThread = new Thread(Decompress);
                threads.Add(decompressThread);

                decompressThread.Start();
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
            var newId = ReadInt32();
            if (newId < 0 || newId >= fileInfo.BlockCount)
            {
                throw new Exception("Input file is unknown.");
            }

            var blockLength = ReadInt32();
            if (blockLength <= 0)
            {
                throw new Exception("Input file is unknown.");
            }

            var newInputStream = new MemoryStream();

            CopyBlock(base._inputStream, newInputStream, blockLength);
            newInputStream.Position = 0;

            return new Block(newId, newInputStream);
        }

        /// <summary>
        ///     Gets file header info.
        /// </summary>
        /// <param name="fileInfo">File information.</param>
        protected override void DefineFileHeader(FileInformation fileInfo)
        {
            base._outputStream.SetLength(fileInfo.FileLength);
        }

        /// <summary>
        ///     Gets block header info.
        /// </summary>
        /// <param name="block">The block data.</param>
        /// <param name="fileInfo">File information.</param>
        protected override void DefineBlockHeader(Block block, FileInformation fileInfo)
        {
            var offset = (long)block.Id * fileInfo.BlockSize;
            base._outputStream.Seek(offset, SeekOrigin.Begin);
        }

        /// <summary>
        ///     Decompresses the input blocks.
        /// </summary>
        private void Decompress()
        {
            try
            {
                foreach (var inputBlock in _inputQueue.GetBlocks())
                {
                    var decompessStream = new MemoryStream();

                    using (var gZipStream = new GZipStream(inputBlock.StreamContent, CompressionMode.Decompress, leaveOpen: true))
                    {
                        CopyStream(gZipStream, decompessStream);
                    }

                    decompessStream.Position = 0;

                    inputBlock.StreamContent.Dispose();

                    _outputQueue.Enqueue(new Block(inputBlock.Id, decompessStream), Timeout.InfiniteTimeSpan);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(ex.Message);
            }
        }

        /// <summary>
        ///     Reads the Int64 value from a header.
        /// </summary>
        private long ReadInt64()
        {
            ReadBytes(this._inputStream, _buffer, sizeof(long));
            return ByteConverter.ReadInt64(_buffer);
        }

        /// <summary>
        ///     Reads the Int32 value from a header.
        /// </summary>
        private int ReadInt32()
        {
            ReadBytes(this._inputStream, _buffer, sizeof(int));
            return ByteConverter.ReadInt32(_buffer);
        }
    }
}