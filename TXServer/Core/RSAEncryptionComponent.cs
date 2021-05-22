using System;
using System.Security.Cryptography;
using TXServer.Core.Logging;

namespace TXServer.Core
{
    public class RSAEncryptionComponent : IDisposable
    {
        RSACryptoServiceProvider provider;
        public string publicKey { get; private set; }
        public SHA256Managed DIGEST = new SHA256Managed();
        public RSAEncryptionComponent(int keyLength = 512)
        {
            provider = new RSACryptoServiceProvider(keyLength);

            RSAParameters parameters = provider.ExportParameters(false);

            publicKey = $"{Convert.ToBase64String(parameters.Modulus)}:{Convert.ToBase64String(parameters.Exponent)}";

            /*string xml = provider.ToXmlString(false);
            // Get Modulus
            Logger.Debug("\n\n\n\n" + Convert.ToBase64String(provider.ExportParameters(false).Exponent));
            int startingIndex = xml.IndexOf("<Modulus>") + 9;
            int length = xml.IndexOf("</Modulus>") - startingIndex;
            publicKey = xml.Substring(startingIndex, length);
            // Get Exponent
            startingIndex = xml.IndexOf("<Exponent>") + 10;
            length = xml.IndexOf("</Exponent>") - startingIndex;
            publicKey += ":" + xml.Substring(startingIndex, length); */

            //Logger.Debug($"\n\n\n\n{publicKey}\n\n\n\n");
            // Not sure if this works
        }

        public byte[] Decrypt(string text)
        {
            byte[] cipherText = Convert.FromBase64String(text);
            Logger.Debug("Input length => " + text.Length);

            //string decrypted = Encoding.UTF8.GetString(plainText);
            return Decrypt(cipherText);
        }

        public byte[] Decrypt(byte[] data)
        {
            return provider.Decrypt(data, false);
        }

        public void Dispose()
        {
            publicKey = null;
            provider.Dispose();
            DIGEST.Dispose();
            GC.SuppressFinalize(this);
        }

        public byte[] XorArrays(byte[] a, byte[] b)
        {
            if (a.Length == b.Length)
            {
                byte[] array = new byte[a.Length];
                for (int i = 0; i < a.Length; i++)
                {
                    array[i] = (byte)(a[i] ^ b[i]);
                }
                return array;
            }
            throw new ArgumentException();
        }

        public byte[] ConcatenateArrays(byte[] a, byte[] b)
        {
            byte[] array = new byte[a.Length + b.Length];
            a.CopyTo(array, 0);
            b.CopyTo(array, a.Length);
            return array;
        }
    }
}
