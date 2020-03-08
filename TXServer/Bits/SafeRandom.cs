using System;

namespace TXServer.Bits
{
    static class SafeRandom
    {
        public static int Next()
        {
            lock (random)
            {
                return random.Next();
            }
        }

        private static Random random = new Random();
    }
}
