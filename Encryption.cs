using System;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DeterministicRSA
{
    private string publicKey;
    private string privateKey;

    public DeterministicRSA(string username, string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var seed = sha256.ComputeHash(Encoding.UTF8.GetBytes(username + password));
            var rng = new DeterministicRandomNumberGenerator(seed);

            using (var rsa = new RSACryptoServiceProvider(2048, rng))
            {
                publicKey = Convert.ToBase64String(StructToBytes(rsa.ExportParameters(false)));
                privateKey = Convert.ToBase64String(StructToBytes(rsa.ExportParameters(true)));
            }
        }
    }

    public string PublicKey
    {
        get { return publicKey; }
    }

    public string PrivateKey
    {
        get { return privateKey; }
    }

    private byte[] StructToBytes(RSAParameters param)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, param);
            return ms.ToArray();
        }
    }
}

public class DeterministicRandomNumberGenerator : RandomNumberGenerator
{
    private Random random;

    public DeterministicRandomNumberGenerator(byte[] seed)
    {
        var seedInt = BitConverter.ToInt32(seed, 0);
        random = new Random(seedInt);
    }

    public override void GetBytes(byte[] data)
    {
        random.NextBytes(data);
    }
}
