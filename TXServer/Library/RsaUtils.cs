using System.Security.Cryptography;

namespace TXServer.Library
{
    public static class RsaUtils
    {
        public static byte[] Encrypt(byte[] data, RSAParameters parameters, int keySize)
        {
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            rsa.ImportParameters(parameters);

            return rsa.Encrypt(data, false);
        }

        public static byte[] Decrypt(byte[] encrypted, RSAParameters parameters, int keySize)
        {
            using RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(keySize);
            rsa.ImportParameters(parameters);

            return rsa.Decrypt(encrypted, false);
        }
    }
}
