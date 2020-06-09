namespace GZipTest.Models
{
    /// <summary>
    ///     Represents information about the file.
    /// </summary>
    public sealed class FileInformation
    {
        private readonly long _fileLength;
        private readonly int _blockSize;
        private readonly int _blockCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileInformation"/> class.
        /// </summary>
        /// <param name="fileLength">The file's length.</param>
        /// <param name="blockSize">The block's size.</param>
        public FileInformation(long fileLength, int blockSize)
        {
            _fileLength = fileLength;
            _blockSize = blockSize;
            _blockCount = this.GetBlockCount(fileLength, blockSize);
        }

        /// <summary>
        ///     Gets the file length.
        /// </summary>
        public long FileLength => this._fileLength;

        /// <summary>
        ///     Gets the block size.
        /// </summary>
        public int BlockSize => this._blockSize;

        /// <summary>
        ///     Gets the block count.
        /// </summary>
        public int BlockCount => this._blockCount;

        /// <summary>
        ///     Calculates the block count.
        /// </summary>
        private int GetBlockCount(long fileLength, int blockSize)
        {
            var blockCount = (int)(fileLength / blockSize);
            if (fileLength % blockSize != 0)
            {
                blockCount++;
            }

            return blockCount;
        }
    }
}