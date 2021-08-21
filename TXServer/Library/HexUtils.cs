using System;
using System.Linq;

namespace TXServer.Library
{
    public static class HexUtils
    {
        public static byte[] StringToBytes(string hex) {
            if (hex.Length % 2 == 1) throw new ArgumentException("The binary key cannot have an odd number of digits", nameof(hex));

            byte[] array = new byte[hex.Length / 2];

            for (int i = 0; i < hex.Length / 2; ++i)
            {
                array[i] = (byte)((GetHexValue(hex[i << 1]) << 4) + GetHexValue(hex[(i << 1) + 1]));
            }

            return array;
        }

        private static int GetHexValue(char hex) {
            int value = hex;
            return value - (value < 58 ? 48 : value < 97 ? 55 : 87);
        }
    }
}
