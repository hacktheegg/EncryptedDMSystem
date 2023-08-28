using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Parameters;
using System.IO;

namespace Security
{
    public class DeterministicRSA
    {
        private string publicKey;
        private string privateKey;

        public DeterministicRSA(string username, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var seed = sha256.ComputeHash(Encoding.UTF8.GetBytes(username + password));
                var rng = SecureRandom.GetInstance("SHA1PRNG");
                rng.SetSeed(seed);
                
                RsaKeyPairGenerator generator = new RsaKeyPairGenerator();
                generator.Init(new KeyGenerationParameters(rng, 2048));

                var pair = generator.GenerateKeyPair();

                //convert to string
                TextWriter textWriter = new StringWriter();
                PemWriter pemWriter = new PemWriter(textWriter);
                pemWriter.WriteObject(pair.Public);
                publicKey = textWriter.ToString();
                
                textWriter = new StringWriter();
                pemWriter = new PemWriter(textWriter);
                pemWriter.WriteObject(pair.Private);
                privateKey = textWriter.ToString();
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
    }
}