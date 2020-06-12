namespace GZipTest.Helpers
{
    /// <summary>
    ///     Converts int and long type values to a byte array.
    /// </summary>
    public class ByteConverter
    {
        public static void WriteBytes(int value, byte[] byteArray)
        {
            byteArray[0] = (byte)value;
            byteArray[1] = (byte)(value >> 8);
            byteArray[2] = (byte)(value >> 16);
            byteArray[3] = (byte)(value >> 24);
        }
        
        public static void WriteBytes(long value, byte[] byteArray)
        {
            var intValue = (int)value;
            byteArray[0] = (byte)intValue;
            byteArray[1] = (byte)(intValue >> 8);
            byteArray[2] = (byte)(intValue >> 16);
            byteArray[3] = (byte)(intValue >> 24);

            intValue = (int)(value >> 32);
            byteArray[4] = (byte)intValue;
            byteArray[5] = (byte)(intValue >> 8);
            byteArray[6] = (byte)(intValue >> 16);
            byteArray[7] = (byte)(intValue >> 24);
        }

        public static long ReadBytesToInt64(byte[] byteArray)
        {
            var value = (long) (byteArray[0] | byteArray[1] << 8 | byteArray[2] << 16 | byteArray[3] << 24) |
                        (long) (byteArray[4] | byteArray[5] << 8 | byteArray[6] << 16 | byteArray[7] << 24) << 32;

            return value;
        }

        public static int ReadBytesToInt32(byte[] byteArray)
        {
            return byteArray[0] | byteArray[1] << 8 | byteArray[2] << 16 | byteArray[3] << 24;
        }
    }
}