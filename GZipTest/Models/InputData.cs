using GZipTest.Common;

namespace GZipTest.Models
{
    /// <summary>
    ///     Represents user's input arguments.
    /// </summary>
    public class InputData
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InputData"/> class.
        /// </summary>
        /// <param name="operationType">Type of archive operation.</param>
        /// <param name="inputFilePath">Input file path.</param>
        /// <param name="outputFilePath">Output file path.</param>
        public InputData(OperationType operationType, string inputFilePath, string outputFilePath)
        {
            this.OperationType = operationType;
            this.InputFilePath = inputFilePath;
            this.OutputFilePath = outputFilePath;
        }

        /// <summary>
        ///     Gets type of archive operation..
        /// </summary>
        public OperationType OperationType { get; private set; }

        /// <summary>
        ///     Gets the input file path.
        /// </summary>
        public string InputFilePath { get; private set; }

        /// <summary>
        ///     Gets the input file path.
        /// </summary>
        public string OutputFilePath { get; private set; }
    }
}
