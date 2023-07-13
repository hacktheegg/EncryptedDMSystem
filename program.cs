using System;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Chat.Generate.Name("LongMessage", "Short", "ChatRoom");

        Console.ReadLine();
    }

    public class Chat
    {
        public class Generate
        {
            public static void Name(string Name1, string Name2, string chatName) {
                string Name1Hex = utilities.encode.ByteArrayToHexString(Name1);
                string Name2Hex = utilities.encode.ByteArrayToHexString(Name2);

                string FileName = utilities.encode.WeaveStrings(Name1Hex, Name2Hex);

                utilities.fileCreation(FileName, chatName);
            }
        }
    }

    public class utilities {
        public static string longerString(string a, string b) {
            if (a.Length > b.Length) {
                return a;
            } else if (a.Length == b.Length) {
                return a;
            } else {
                return b;
            }
        }
        public static void fileCreation(string ChatName, string internalHeading) {
            string fileName = ChatName + ".txt";

            try
            {
                // Check if file already exists. If yes, delete it.
                if (File.Exists(fileName))
                {
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
        public class encode {
            public static string ByteArrayToHexString(string a)
            {
                byte[] ba = Encoding.ASCII.GetBytes(a);
                StringBuilder hex = new StringBuilder(ba.Length * 2);
                foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);

                return hex.ToString();
            }
            public static string WeaveStrings(string StringA, string StringB) {
                char[] CharA = StringA.ToCharArray();
                char[] CharB = StringB.ToCharArray();

                string longerString = utilities.longerString(CharA.ToString(), CharB.ToString());

                char[] returnVal = new char[longerString.Length*2];
                
                int FileNamePointer = 0;

                for (int charPointer = 0; charPointer < longerString.Length; charPointer++) {

                    if (charPointer < CharA.Length) {
                        returnVal[FileNamePointer] = CharA[charPointer];
                    } else {
                        returnVal[FileNamePointer] = '0';
                    }

                    FileNamePointer++;

                    if (charPointer < CharB.Length) {
                        returnVal[FileNamePointer] = CharB[charPointer];
                    } else {
                        returnVal[FileNamePointer] = '0';
                    }

                    FileNamePointer++;
                }
                
                return new string(returnVal);
            }
        }
    }
}