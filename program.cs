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
                string Name1Hex = utilities.ByteArrayToHexString(Name1);
                string Name2Hex = utilities.ByteArrayToHexString(Name2);


                string longerString = utilities.longerString(Name1Hex, Name2Hex);

                char[] FileName = new char[new string(Name1CharArray).Length*2];


                int FileNamePointer = 0;
                //migrating code
                for (int NameCharArrayPointer = 0; NameCharArrayPointer < longerString.Length*2; NameCharArrayPointer++) {
                    if (NameCharArrayPointer < Name1CharArray.Length) {
                        FileName[FileNamePointer] = Name1CharArray[NameCharArrayPointer];
                    } else {
                        FileName[FileNamePointer] = '0';
                    }

                    FileNamePointer++;

                    if (NameCharArrayPointer < Name2CharArray.Length) {
                        FileName[FileNamePointer] = Name2CharArray[NameCharArrayPointer];
                    } else {
                        FileName[FileNamePointer] = '0';
                    }

                    FileNamePointer++;
                }

                Console.WriteLine(new string(FileName));

                utilities.fileCreation(new string(FileName), chatName);
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
                        Console.WriteLine(s);
                    }
                }
            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }
        }
    }
    public class encode {
        public static string ByteArrayToHexString(string a)
        {
            ba = Encoding.ASCII.GetBytes(a);
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            hex.AppendFormat("{0:x2}", b);

            Console.WriteLine(hex.ToString());
            return hex.ToString();
        }
        public static string WeaveStrings(string StringA, string StringB) {
            char[] CharA = Encoding.ASCII.GetBytes(StringA);
            char[] CharB = Encoding.ASCII.GetBytes(StringB);

            char[] return = new char[utilities.longerString(StringA, StringB).Length*2];

            //where code being migrated
        }
    }
}

