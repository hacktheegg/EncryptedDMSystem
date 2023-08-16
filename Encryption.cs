using System;
using System.Security.Cryptography;
using System.Text;

public class KeyGenerator
{
    public RSAParameters GenerateKeyPair(string loginName, string password)
    {
        // Combine the loginName and password into a single string
        string combined = loginName + password;

        // Hash the combined string to generate a seed
        SHA256 sha256 = SHA256Managed.Create();
        byte[] seed = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));

        // Use the seed to initialize a pseudorandom number generator
        var rng = new RNGCryptoServiceProvider(seed);

        // Generate a key pair using the pseudorandom number generator
        var rsa = new RSACryptoServiceProvider(2048, new CspParameters() { KeyNumber = (int)KeyNumber.Exchange, Flags = CspProviderFlags.UseMachineKeyStore, ProviderName = "Microsoft Strong Cryptographic Provider", RandomNumberGenerator = rng });

        // Return the key pair
        return rsa.ExportParameters(true);
    }

    public static byte[] Encrypt(string data, RSAParameters publicKey)
    {
        var byteArray = Encoding.ASCII.GetBytes(data);
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(publicKey);
            return rsa.Encrypt(byteArray, false);
        }
    }

    public static string Decrypt(byte[] encryptedData, RSAParameters privateKey)
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            var decryptedByteArray = rsa.Decrypt(encryptedData, false);
            return Encoding.ASCII.GetString(decryptedByteArray);
        }
    }
}
