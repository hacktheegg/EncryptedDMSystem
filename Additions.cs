using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Security
{
    public class DeterministicSecureRandom : SecureRandom
    {
        private SHA256 sha256 = SHA256.Create();
        private byte[] seed;

        public DeterministicSecureRandom(byte[] seed)
        {
            this.seed = seed;
        }

        public override void NextBytes(byte[] buffer)
        {
            int offset = 0;
            while (offset < buffer.Length)
            {
                // Generate a new hash value based on the seed.
                byte[] hashValue = sha256.ComputeHash(seed);

                // Determine the number of bytes to copy in this round.
                int bytesToCopy = Math.Min(hashValue.Length, buffer.Length - offset);

                // Copy the bytes from the hash value to the buffer.
                Array.Copy(hashValue, 0, buffer, offset, bytesToCopy);

                // Update the seed with the new hash value for the next round.
                seed = hashValue;

                // Move the offset for the next round.
                offset += bytesToCopy;
            }
        }

    }

    public class DeterministicRSA
    {
        private string publicKey;
        private string privateKey;

        public DeterministicRSA(string username, string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var seed = sha256.ComputeHash(Encoding.UTF8.GetBytes(username + password));
                var rng = new DeterministicSecureRandom(seed);

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

        public string Public
        {
            get { return publicKey; }
        }

        public string Private
        {
            get { return privateKey; }
        }

        public string Encrypt(string data, string recipientPublicKey)
        {
            var cipher = CipherUtilities.GetCipher("RSA/ECB/NoPadding");
            cipher.Init(true, new PemReader(new StringReader(recipientPublicKey)).ReadObject() as AsymmetricKeyParameter);

            var dataToEncrypt = Encoding.UTF8.GetBytes(data);
            var encryptedData = cipher.DoFinal(dataToEncrypt);

            return Convert.ToBase64String(encryptedData);
        }

        public string Decrypt(string data, string recipientPrivateKey)
        {
            var cipher = CipherUtilities.GetCipher("RSA/ECB/NoPadding");
            var keyPair = (AsymmetricCipherKeyPair)new PemReader(new StringReader(recipientPrivateKey)).ReadObject();
            cipher.Init(false, keyPair.Private);

            var dataToDecrypt = Convert.FromBase64String(data);
            var decryptedData = cipher.DoFinal(dataToDecrypt);

            return Encoding.UTF8.GetString(decryptedData);
        }
    }



    public class SymmetricEncryption
    {
        public static Tuple<string, string> GenerateKeyAndIV()
        {
            using (Aes aes = Aes.Create())
            {
                aes.GenerateKey();
                aes.GenerateIV();

                return new Tuple<string,string>(Convert.ToBase64String(aes.Key), Convert.ToBase64String(aes.IV));
                //return (Convert.ToBase64String(aes.Key), Convert.ToBase64String(aes.IV));
            }
        }

        public static string EncryptString(string key, string plainText, string iv)
        {
            byte[] encrypted;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(encrypted);
        }

        public static string DecryptString(string key, string cipherText, string iv)
        {
            string plaintext = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Convert.FromBase64String(key);
                aes.IV = Convert.FromBase64String(iv);

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
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

            string longerString = DMSExtras.DMSExtras.LongerString(new string(CharA), new string(CharB));

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
}


namespace DMSExtras
{
    public class DMSExtras
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
        public class ChatListener {
            private string[] ChatContent;
            private readonly object lockObject = new object();

            public string[] GetChatContent() {
                //Thread.Sleep(50);
                lock (lockObject) {
                    return ChatContent;
                }
            }

            public void StartListening(string ChatRoom) {

                while (true) {
                    try {
                        ChatContent = File.ReadLines(ChatRoom+".txt").ToArray();
                    } catch {
                        Console.Write("");
                    }
                    Thread.Sleep(50);
                }
            }
        }

        public class TextListener {
            private string TypedText;
            private int Pointer = 0;
            private bool EnterPressed = false;
            private bool ESCPressed = false;
            private bool isListening = false;
            private Thread listeningThread;
            private readonly object lockObject = new object();

            public void SetEnterPressed(bool var) {
                Monitor.Enter(lockObject);
                try {
                    EnterPressed = var;
                } finally {
                    Monitor.Exit(lockObject);
                }
            }

            public void SetESCPressed(bool var) {
                Monitor.Enter(lockObject);
                try {
                    ESCPressed = var;
                } finally {
                    Monitor.Exit(lockObject);
                }
            }

            public void SetTypedText(string var) {
                Monitor.Enter(lockObject);
                try {
                    TypedText = var;
                } finally {
                    Monitor.Exit(lockObject);
                }
            }

            public void SetPointer(int var) {
                Monitor.Enter(lockObject);
                try {
                    Pointer = var;
                } finally {
                    Monitor.Exit(lockObject);
                }
            }

            public bool GetEnterPressed() {
                Monitor.Enter(lockObject);
                try {
                    return EnterPressed;
                } finally {
                    Monitor.Exit(lockObject);
                }
            }

            public bool GetESCPressed() {
                Monitor.Enter(lockObject);
                try {
                    return ESCPressed;
                } finally {
                    Monitor.Exit(lockObject);
                }
            }

            public string GetTypedText() {
                Monitor.Enter(lockObject);
                try {
                    return TypedText;
                } finally {
                    Monitor.Exit(lockObject);
                }
            }

            public int GetPointer() {
                Monitor.Enter(lockObject);
                try {
                    return Pointer;
                } finally {
                    Monitor.Exit(lockObject);
                }
            }

            public void StartListening() {
                isListening = true;
                listeningThread = new Thread(() => {
                    StringBuilder sb = new StringBuilder();
                    while (isListening) {
                        if (Console.KeyAvailable) {
                            var key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Enter) {
                                SetEnterPressed(true);
                            } else if (key.Key == ConsoleKey.Escape) {
                                SetESCPressed(true);
                            } else if (key.Key == ConsoleKey.Backspace && !string.IsNullOrEmpty(GetTypedText())) {
                                Monitor.Enter(lockObject);
                                try {
                                    TypedText = TypedText.Remove(TypedText.Length - 1);
                                    sb.Remove(sb.Length - 1, 1);
                                } finally {
                                    Monitor.Exit(lockObject);
                                }
                            } else if (key.Key == ConsoleKey.UpArrow) {
                                Monitor.Enter(lockObject);
                                try {
                                    Pointer++;
                                } finally {
                                    Monitor.Exit(lockObject);
                                }
                            } else if (key.Key == ConsoleKey.DownArrow) {
                                Monitor.Enter(lockObject);
                                try {
                                    Pointer--;
                                } finally {
                                    Monitor.Exit(lockObject);
                                }
                            } else {
                                Monitor.Enter(lockObject);
                                try {
                                    sb.Append(key.KeyChar);
                                    TypedText = sb.ToString();
                                    // Console.WriteLine(TypedText);
                                } finally {
                                    Monitor.Exit(lockObject);
                                }
                            }
                        }
                    }
                });
                listeningThread.Start();
            }
            public void StopListening() {
                isListening = false;
                if (listeningThread != null) {
                    listeningThread.Join(); // Wait for the listening thread to finish
                }
            }
        }
    }
}