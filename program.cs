using System;
using System.IO;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        Chat.Generate.Name("Long", "Short", "Length");
        Chat.Generate.Name("username1One", "person2Fit", "People");

        string[][] var = Chat.Read.Name.All(@"chats\");

        for (int i = 0; i < var.Length; i++) {
            Console.WriteLine(var[i][0] + "  |  " + var[i][1]);
        }

        Console.ReadLine();
    }

    public class Chat
    {
        public class Generate
        {
            public static void Name(string Name1, string Name2, string chatName = "") {
                string Name1Hex = Utilities.Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(Name1));
                string Name2Hex = Utilities.Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(Name2));

                string FileName = Utilities.Encode.WeaveStrings(Name1Hex, Name2Hex);
                Utilities.fileCreation(FileName, chatName);
            }
        }

        public class Read
        {
            public class Name
            {
                public static string[][] All(string folder) {
                    FileInfo[] files = new DirectoryInfo(folder).GetFiles("*.txt");

                    string[] fileNames = new string[files.Length];

                    for (int i = 0; i < fileNames.Length; i++) {
                        fileNames[i] = files[i].Name.Replace(".txt", "");
                    }

                    string[][] returnValHex = new string[fileNames.Length][];

                    for (int i = 0; i < fileNames.Length; i++) {
                        returnValHex[i] = Utilities.Encode.UnravelStrings(fileNames[i]);
                    }

                    string[][] returnVal = new string[fileNames.Length][];
                    
                    for (int i = 0; i < fileNames.Length; i++) {
                        returnVal[i] = new string[2];
                        returnVal[i][0] = System.Text.Encoding.Default.GetString(Utilities.Encode.HexStringToByteArray(returnValHex[i][0]));
                        returnVal[i][1] = System.Text.Encoding.Default.GetString(Utilities.Encode.HexStringToByteArray(returnValHex[i][1]));
                    }

                    return returnVal;
                }
            }
        }
    }

    public class Utilities
    {
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
            string fileName = @"chats\"+ ChatName + ".txt";

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

                string longerString = Utilities.longerString(new string(CharA), new string(CharB));

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
}
