using System;

namespace GZipTest.Helpers
{
    /// <summary>
    ///     Converts int and long type values to a byte array.
    /// </summary>
    public class ByteConverter
    {
        public static void WriteBytes(int value, ref byte[] byteArray)
        {
            byteArray = BitConverter.GetBytes(value);
        }
        
        public static void WriteBytes(long value, ref byte[] byteArray)
        {
            byteArray = BitConverter.GetBytes(value);
        }

        public static long ReadBytesToInt64(byte[] byteArray)
        {
            var value = BitConverter.ToInt64(byteArray, 0);

            return value;
        }

        public static int ReadBytesToInt32(byte[] byteArray)
        {
            var value = BitConverter.ToInt32(byteArray, 0);

            return value;
        }
    }
}