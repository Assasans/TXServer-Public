using System;
using System.Security.Cryptography;
using System.Text;
using TXServer.Library;

namespace TXServer.Core
{
    public class PlayerEncryptionComponent : IDisposable
    {
        private readonly RSACryptoServiceProvider _provider;
        private readonly SHA256Managed _sha256;

        private readonly byte[] _passcode;
        private readonly RSAParameters _parameters;

        public string PublicKey => $"{Convert.ToBase64String(_parameters.Modulus)}:{Convert.ToBase64String(_parameters.Exponent)}";

        public PlayerEncryptionComponent(RSAParameters parameters, byte[] passcode, int keyLength = 520)
        {
            _provider = new RSACryptoServiceProvider(keyLength);
            _sha256 = new SHA256Managed();

            _parameters = parameters;
            _passcode = passcode;

            _provider.ImportParameters(_parameters);
        }

        public byte[] GetLoginPasswordHash(byte[] passwordHash)
        {
            byte[] hashPasscodeXor = XorArrays(passwordHash, _passcode);
            byte[] concat = ConcatenateArrays(hashPasscodeXor, passwordHash);

            return _sha256.ComputeHash(concat);
        }

        public byte[] EncryptAutoLoginToken(byte[] token, byte[] passwordHash) => XorArrays(token, passwordHash);
        public byte[] DecryptAutoLoginToken(byte[] encryptedToken, byte[] passwordHash) => XorArrays(encryptedToken, passwordHash);

        public byte[] RsaEncrypt(byte[] data) => RsaUtils.Encrypt(data, _parameters, 520);
        public byte[] RsaEncryptString(string password) => RsaEncrypt(Encoding.UTF8.GetBytes(password));

        public byte[] RsaDecrypt(byte[] encrypted) => RsaUtils.Decrypt(encrypted, _parameters, 520);
        public string RsaDecryptString(byte[] encrypted) => Encoding.UTF8.GetString(RsaDecrypt(encrypted));

        public void Dispose()
        {
            _provider.Dispose();
            _sha256.Dispose();

            GC.SuppressFinalize(this);
        }

        private byte[] XorArrays(byte[] a, byte[] b)
        {
            byte[] array = new byte[b.Length];
            for (int i = 0; i < b.Length; i++)
            {
                array[i] = (byte) (b[i] ^ a[i % a.Length]);
            }
            return array;
        }

        private byte[] ConcatenateArrays(byte[] a, byte[] b)
        {
            byte[] array = new byte[a.Length + b.Length];
            a.CopyTo(array, 0);
            b.CopyTo(array, a.Length);
            return array;
        }
    }
}
