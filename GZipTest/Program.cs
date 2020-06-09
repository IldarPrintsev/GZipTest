using GZipTest.Infrastructure;
using GZipTest.Interfaces;
using System;
using System.Diagnostics;
using System.IO;

namespace GZipTest
{
    class Program
    {
        private static IArchiver archiver;

        static int Main(string[] args)
        {
            try
            {
                AddUnhandledExceptionHandler();

                CheckArguments(args);

                var wrapper = new NinjectWrapper();
                archiver = wrapper.Get<IArchiver>(args[0]);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                archiver.Run(args[1], args[2]);

                stopwatch.Stop();

                Console.WriteLine($"Successfully done in {stopwatch.Elapsed.TotalSeconds} seconds.");

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                ShowHelp();

                return 1;
            }
        }

        /// <summary>
        ///     Shows help information.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("\nTo compress or decompress files use the following pattern:\n" +
                              "Compression: GZipTest.exe compress [original_file_path] [compressed_file_path]\n" +
                              "Decompression: GZipTest.exe decompress [compressed_file_path] [original_file_path]\n");
        }

        /// <summary>
        ///    Adds the handler of UnhandledException.
        /// </summary>
        private static void AddUnhandledExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) => { Console.Error.WriteLine(e); };
        }

        /// <summary>
        ///    Check the arguments.
        /// </summary>
        private static void CheckArguments(string[] args)
        {
            if (args == null || args.Length != 3)
                throw new ArgumentException("Three command-line parameters are expected: operation type (compress or decompress), input file path, output file path.");

            if (args == null || args.Length != 3)
                throw new ArgumentException("Three command-line parameters are expected: operation type (compress or decompress), input file path, output file path.");

            string operationString = args[0].ToLower();
            if (!(string.Equals(operationString, "compress") || string.Equals(operationString, "decompress")))
            {
                throw new ArgumentException($"Unsupported operation: {operationString}. Supported operations: compress, decompress.");
            }

            string inputFilePath = args[1];
            if (inputFilePath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException($"Invalid input file path: {inputFilePath}.");
            }

            if (!File.Exists(inputFilePath))
            {
                throw new ArgumentException($"Input file /{inputFilePath}/ is not exists.");
            }

            string outputFilePath = args[2];
            if (outputFilePath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException($"Invalid output file path: {outputFilePath}.");
            }

            if (File.Exists(outputFilePath))
            {
                throw new ArgumentException($"Output file with path {outputFilePath} is already exists. Remove it or choose another name for output file.");
            }

            var inputFile = new FileInfo(args[1]);
            var outputFile = new FileInfo(args[2]);

            if (operationString == "compress" && inputFile.Extension == ".gz")
            {
                throw new Exception("File has already been compressed.");
            }

            if (operationString == "compress" && outputFile.Extension != ".gz")
            {
                throw new Exception("File to be decompressed shall have .gz extension.");
            }

            if (operationString == "decompress" && inputFile.Extension != ".gz")
            {
                throw new Exception("File to be decompressed shall have .gz extension.");
            }
        }
    }
}
