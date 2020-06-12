using System;

namespace GZipTest.Exceptions
{
    /// <summary>
    ///     The exception that is thrown when an archiver error occurs.
    /// </summary>
    public sealed class ArchiverException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ArchiverException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public ArchiverException(string message) : base(message) { }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ArchiverException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception.</param>
        public ArchiverException(string message, Exception innerException) : base(message, innerException) { }
    }
}
