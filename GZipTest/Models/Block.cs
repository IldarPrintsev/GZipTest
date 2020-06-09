using System.IO;

namespace GZipTest.Models
{
    /// <summary>
    ///     Represents the data of a file block.
    /// </summary>
    public sealed class Block
    {
        private readonly int _id;
        private readonly MemoryStream _streamContent;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Block"/> class.
        /// </summary>
        /// <param name="id">The block's ID.</param>
        /// <param name="streamContent">The block's content.</param>
        public Block(int id, MemoryStream streamContent)
        {
            this._id = id;
            this._streamContent = streamContent;
        }

        /// <summary>
        ///     Gets the block's ID.
        /// </summary>
        public int Id => this._id;

        /// <summary>
        ///     Gets the block's content.
        /// </summary>
        public MemoryStream StreamContent => this._streamContent;
    }
}