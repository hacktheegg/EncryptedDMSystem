using System;
using System.Security.Cryptography;
using System.Text;



namespace EncryptionDecryptionSet
{
    public static class RSACryptography
    {
        // Function to generate a 32-byte key from the loginName and password
        public static byte[] GenerateKeyPair(string loginName, string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // Combining the loginName and password to generate a unique key for each user
                return sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(loginName + password));
            }
        }

        // Function to encrypt data
        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    // Import the RSA Key information
                    rsa.ImportParameters(ConvertToRSAParameters(key));

                    // Encrypt the data and return the result
                    return rsa.Encrypt(data, false);
                }
                finally
                {
                    // Always clear securable data from memory when you're done with it
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        // Function to decrypt data
        public static string Decrypt(byte[] data, byte[] key)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                try
                {
                    // Import the RSA Key information
                    rsa.ImportParameters(ConvertToRSAParameters(key));

                    // Decrypt the data and return the result
                    return Encoding.UTF8.GetString(rsa.Decrypt(data, false));
                }
                finally
                {
                    // Always clear securable data from memory when you're done with it
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        // Convert a key to RSAParameters
        private static RSAParameters ConvertToRSAParameters(byte[] key)
        {
            // Create a new RSAParameters structure to receive the key 
            RSAParameters RSAKeyInfo = new RSAParameters();

            // Set RSAKeyInfo to the public key values: 
            RSAKeyInfo.Modulus = key;
            RSAKeyInfo.Exponent = new byte[] { 01, 00, 01, 00 };  // default for 65537 in .NET

            // Import key parameters into RSA 
            return RSAKeyInfo;
        }

        public static RSAParameters GetPublicKey(string loginName, string password)
        {
            byte[] key = GenerateKeyPair(loginName, password);
            return ConvertToRSAParameters(key);
        }
    }
}