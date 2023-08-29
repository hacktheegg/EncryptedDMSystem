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



    public class Encode
    {
        public static string ByteArrayToHexString(byte[] ba) {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba) hex.AppendFormat("{0:x2}", b);

            return hex.ToString();
        }
        public static byte[] HexStringToByteArray(String hexString) {
            int byteCount = hexString.Length / 2;
            byte[] byteArray = new byte[byteCount];

            for (int i = 0; i < byteCount; i++) {
                string byteValue = hexString.Substring(i * 2, 2);
                byteArray[i] = Convert.ToByte(byteValue, 16);
            }

            return byteArray;
        }
        public static string WeaveStrings(string StringA, string StringB) {
            char[] CharA = StringA.ToCharArray();
            char[] CharB = StringB.ToCharArray();

            string longerString = Utilities.LongerString(new string(CharA), new string(CharB));

            char[] returnVal = new char[longerString.Length*2];
            
            int FileNamePointer = 0;

            for (int CharPointer = 0; CharPointer < longerString.Length; CharPointer++) {

                if (CharPointer < CharA.Length) {
                    returnVal[FileNamePointer] = CharA[CharPointer];
                } else {
                    returnVal[FileNamePointer] = '0';
                }

                FileNamePointer++;

                if (CharPointer < CharB.Length) {
                    returnVal[FileNamePointer] = CharB[CharPointer];
                } else {
                    returnVal[FileNamePointer] = '0';
                }

                FileNamePointer++;
            }
            
            return new string(returnVal);
        }

        public static string[] UnravelStrings(string StringVar) {

            if ((StringVar.Length%2) != 0) {
                StringVar = StringVar + "0";
            }

            char[] CharA = new char[StringVar.Length/2];
            char[] CharB = new char[StringVar.Length/2];

            char[] CharVar = StringVar.ToCharArray();

            bool flipper = true;

            int CharPointer = 0;

            for (int FileNamePointer = 0; FileNamePointer < CharVar.Length; FileNamePointer++) {

                if (flipper) {

                    CharA[CharPointer] = CharVar[FileNamePointer];

                } else {

                    CharB[CharPointer] = CharVar[FileNamePointer];

                }

                if (!flipper) {
                    CharPointer++;
                }

                flipper = !flipper;

            }

            string[] returnVal = { new string(CharA).TrimEnd('0'), new string(CharB).TrimEnd('0') };
            
            return returnVal;
        }
    }



    private class Utilities
    {
        public static string LongerString(string a, string b) {
            if (a.Length > b.Length) {
                return a;
            } else if (a.Length == b.Length) {
                return a;
            } else {
                return b;
            }
        }
        public static void FileCreation(string ChatName, string internalHeading = "") {
            string fileName = @"chats\"+ ChatName + ".txt";

            try
            {
                // Check if file already exists. If yes, delete it.
                if (!Directory.Exists("chats")) {
                    Directory.CreateDirectory("chats");
                }
                if (File.Exists(fileName)) {
                    File.Delete(fileName);
                }
                
                // Create a new file
                using (FileStream fs = File.Create(fileName))
                {
                    // Add some text to file
                    Byte[] title = new UTF8Encoding(true).GetBytes(internalHeading);
                    fs.Write(title, 0, title.Length);
                }
                
                // Open the stream and read it back.
                using (StreamReader sr = File.OpenText(fileName))
                {
                    string s = "";
                    while ((s = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(s);
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }
    }
}