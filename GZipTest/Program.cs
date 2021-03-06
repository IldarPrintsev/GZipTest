﻿using GZipTest.Exceptions;
using GZipTest.Infrastructure;
using GZipTest.Common;
using GZipTest.Interfaces;
using GZipTest.Models;
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

                var inputData = CheckArguments(args);

                var wrapper = new NinjectWrapper();
                archiver = wrapper.Get<IArchiver, OperationType>(inputData.OperationType);

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                archiver.Run(inputData.InputFilePath, inputData.OutputFilePath);

                stopwatch.Stop();

                Console.WriteLine($"\nCompletely done in {stopwatch.Elapsed.TotalSeconds} seconds.");

                return 0;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"\nAn error of input arguments occurs: {ex.Message}");
                ShowHelp();

                return 1;
            }
            catch (IOException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
                ShowInfo();

                return 1;
            }
            catch (ArchiverException ex)
            {
                Console.WriteLine($"\n{ex.Message}");
                Console.WriteLine($"Method: {ex.TargetSite}");
                ShowInfo();

                return 1;
            }
            catch (OutOfMemoryException)
            {
                Console.WriteLine($"\nThere is not enought memory to continue the execution. Upgrade your PC or use an another file.");

                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nAn unknown error occurs: {ex.Message}");
                Console.WriteLine($"Method: {ex.TargetSite}");
                ShowInfo();

                return 1;
            }
        }

        /// <summary>
        ///     Shows help information.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine("\n\nPlease use the following pattern:\n" +
                              "Compression: GZipTest.exe compress [original_file_path] [compressed_file_path]\n" +
                              "Decompression: GZipTest.exe decompress [compressed_file_path] [original_file_path]\n");
        }

        /// <summary>
        ///     Shows contact information.
        /// </summary>
        private static void ShowInfo()
        {
            Console.WriteLine("Try again or contact the developers!");
        }

        /// <summary>
        ///    Adds the handler of UnhandledException.
        /// </summary>
        private static void AddUnhandledExceptionHandler()
        {
            AppDomain.CurrentDomain.UnhandledException += (o, e) =>
            {
                Console.Error.WriteLine($"An unhandled exception occurs: {e}");
                ShowInfo();
            };
        }

        /// <summary>
        ///    Check the arguments.
        /// </summary>
        private static InputData CheckArguments(string[] args)
        {
            if (args == null || args.Length != 3)
            {
                throw new ArgumentException("Three command-line parameters are expected: operation type (compress or decompress), input file path, output file path.");
            }

            string operationString = args[0].ToLower();
            OperationType operationType;
            switch (operationString)
            {
                case "compress":
                    operationType = OperationType.Compress;
                    break;
                case "decompress":
                    operationType = OperationType.Decompress;
                    break;
                default:
                    throw new ArgumentException($"Unsupported operation: {operationString}. Supported operations: compress, decompress.");
            }

            string inputFilePath = args[1];
            if (inputFilePath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException($"Invalid input file path: {inputFilePath}. Change file's name.");
            }

            if (!File.Exists(inputFilePath))
            {
                throw new ArgumentException($"Input file {inputFilePath} doesn't exist. Use an existing file.");
            }

            string outputFilePath = args[2];
            if (outputFilePath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                throw new ArgumentException($"Invalid output file path: {outputFilePath}. Change file's name.");
            }

            if (File.Exists(outputFilePath))
            {
                throw new ArgumentException($"Output file with path {outputFilePath} already exists. Delete it or choose another name for output file.");
            }

            var inputFile = new FileInfo(args[1]);
            var outputFile = new FileInfo(args[2]);

            if (operationType == OperationType.Compress && inputFile.Extension == ".gz")
            {
                throw new ArgumentException("File has already been compressed.");
            }

            if (operationType == OperationType.Compress && outputFile.Extension != ".gz")
            {
                throw new ArgumentException("Output file must have .gz extension. Change file's name.");
            }

            if (operationType == OperationType.Decompress && inputFile.Extension != ".gz")
            {
                throw new ArgumentException("Input file must have .gz extension.");
            }

            if (operationType == OperationType.Decompress && outputFile.Extension == ".gz")
            {
                throw new ArgumentException("Output file shouldn't have .gz extension. Change file's name.");
            }

            return new InputData(operationType, inputFilePath, outputFilePath);
        }
    }
}
