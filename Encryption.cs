using System;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace EncryptionDecryptionSet
{
    public class EncryptionService
    {
        private RSACryptoServiceProvider rsa;

        public EncryptionService()
        {
            // Get the current Windows identity
            WindowsIdentity identity = WindowsIdentity.GetCurrent();

            // Generate a private-public key pair based on the Windows identity
            CspParameters cspParams = new CspParameters();
            cspParams.KeyContainerName = identity.User.Value;
            rsa = new RSACryptoServiceProvider(cspParams);
        }

        public string GetPublicKey()
        {
            // Get the public key
            return rsa.ToXmlString(false);
        }

        public byte[] EncryptMessage(string message, string publicKey)
        {
            byte[] encryptedMessage;

            using (RSACryptoServiceProvider rsaEncrypt = new RSACryptoServiceProvider())
            {
                rsaEncrypt.FromXmlString(publicKey);
                encryptedMessage = rsaEncrypt.Encrypt(Encoding.UTF8.GetBytes(message), true);
            }

            return encryptedMessage;
        }

        public string DecryptMessage(byte[] encryptedMessage)
        {
            byte[] decryptedMessage = rsa.Decrypt(encryptedMessage, true);
            return Encoding.UTF8.GetString(decryptedMessage);
        }
    }
}