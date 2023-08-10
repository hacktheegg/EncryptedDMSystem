using System.Security.Cryptography;
using System.IO;

public class EncryptionClass
{
    private RSAParameters publicKeyA, publicKeyB;
    private RSAParameters privateKey;

    public EncryptionClass(RSAParameters publicKeyA, RSAParameters publicKeyB, RSAParameters privateKey)
    {
        this.publicKeyA = publicKeyA;
        this.publicKeyB = publicKeyB;
        this.privateKey = privateKey;
    }

    public byte[] GenerateSymmetricKey()
    {
        byte[] symmetricKey;
        using (Aes aes = Aes.Create())
        {
            aes.KeySize = 256; // Key size is 256 bits
            aes.GenerateKey();
            symmetricKey = aes.Key;
        }
        return symmetricKey;
    }

    public byte[] EncryptSymmetricKey(byte[] symmetricKey, RSAParameters publicKey)
    {
        byte[] encryptedKey;
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(publicKey);
            encryptedKey = rsa.Encrypt(symmetricKey, false);
        }
        return encryptedKey;
    }

    public byte[] EncryptMessage(byte[] message, byte[] symmetricKey)
    {
        byte[] encryptedMessage;
        using (Aes aes = Aes.Create())
        {
            aes.Key = symmetricKey;
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(message, 0, message.Length);
                cs.Close();
                encryptedMessage = ms.ToArray();
            }
        }
        return encryptedMessage;
    }

    public byte[] DecryptSymmetricKey(byte[] encryptedKey, RSAParameters privateKey)
    {
        byte[] decryptedKey;
        using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            decryptedKey = rsa.Decrypt(encryptedKey, false);
        }
        return decryptedKey;
    }

    public byte[] DecryptMessage(byte[] encryptedMessage, byte[] decryptedKey)
    {
        byte[] decryptedMessage;
        using (Aes aes = Aes.Create())
        {
            aes.Key = decryptedKey;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream ms = new MemoryStream(encryptedMessage))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                decryptedMessage = new byte[encryptedMessage.Length];
                var bytesRead = cs.Read(decryptedMessage, 0, decryptedMessage.Length);
            }
        }
        return decryptedMessage;
    }
}
