using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace TXServer.Utils
{
    public static class StringUtil
    {
        const string AllowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
        const string UsernameAdditionalCharList = @"!@#$%^&^**()_-+=[]\|/";
        public static bool isUsernameValid(string username)
        {
            return true;
            if (string.IsNullOrWhiteSpace(username) || username.Length < 2)
                return false;

            foreach (char letter in username)
                if (!(AllowedChars.Contains(letter) || UsernameAdditionalCharList.Contains(letter)))
                {
                    //Logger.Log($"Invalid char: '{letter}' => [allowedChar: {AllowedChars.Contains(letter)}, additionalChar: {UsernameAdditionalCharList.Contains(letter)}");
                    return false;
                }

            return true;
        }

        const string EmailAllowedCharsAdditional = "_.+-";
        public static bool isEmailValid(string email)
        {
            // username must be at least length of 1
            char[] username = email.Substring(0, email.IndexOf('@')).ToCharArray();
            if (username.Length < 2)
                return false;
            foreach (char letter in username)
                if (!AllowedChars.Contains(letter) || !EmailAllowedCharsAdditional.Contains(EmailAllowedCharsAdditional))
                    return false;

            if (email.LastIndexOf('@') > email.LastIndexOf('.')) return false;

            char[] hostname = email.Substring(email.IndexOf('@') + 1, email.LastIndexOf('.') - (email.IndexOf('@') + 1)).ToCharArray();
            if (hostname.Length < 2)
                return false;
            foreach (char letter in hostname)
                if (!AllowedChars.Contains(letter) && '.' != letter)
                    return false;

            char[] tld = email.Substring(email.LastIndexOf('.')).ToCharArray();
            if (tld.Length < 2) return false;
            foreach (char letter in hostname)
                if (!AllowedChars.Contains(letter))
                    return false;
            return true;
        }
    }
    public static class MD5Util
    {
        public static string ComputeFromPath(string relativePath, bool capitalize = false)
        {
            using (var Hasher = MD5.Create())
            {
                using (var stream = File.OpenRead(Path.Combine(Environment.CurrentDirectory, relativePath)))
                {
                    byte[] hash = Hasher.ComputeHash(stream);
                    StringBuilder result = new StringBuilder(hash.Length * 2);
                    foreach (byte bit in hash)
                        result.Append(bit.ToString(capitalize ? "X2" : "x2"));
                    return result.ToString();
                }
            }
        }

        public static string Compute(string input, bool capitalize = false)
        {
            using (var Hasher = MD5.Create())
            {
                byte[] hash = Hasher.ComputeHash(Encoding.ASCII.GetBytes(input));
                StringBuilder result = new StringBuilder(hash.Length * 2);
                foreach (byte bit in hash)
                    result.Append(bit.ToString(capitalize ? "X2" : "x2"));
                return result.ToString();
            }
        }

        public static string Compute(string input, string alg, bool capitalize = false)
        {
            if (string.IsNullOrWhiteSpace(alg))
                return Compute(input, capitalize);
            using (var Hasher = MD5.Create(alg))
            {
                byte[] hash = Hasher.ComputeHash(Encoding.ASCII.GetBytes(input)); // Error here for some reason
                StringBuilder result = new StringBuilder(hash.Length * 2);
                foreach (byte bit in hash)
                    result.Append(bit.ToString(capitalize ? "X2" : "x2"));
                return result.ToString();
            }
        }
    }
    [Serializable]
    public class UserError : Exception
    {
        // This is to define an error from the clients input (Eg: 'Username Invalid' or 'Username Unavailable')
        public UserError(string message) : base(message) { }
    }
}
