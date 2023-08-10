using System;
using System.IO;
using System.Linq;
using System.Text;

using ObjectList;

/*


    Current Script to Impliment:
        algorithm:  PrimeNo(LoginName) = PrimeNo(Password+UserName)*PublicKey
            ratio:  1:5


*/

class Program
{
    static void Main(string[] args)
    {
        // example state //
        Chat.Generate.Name("luigi", "wario", "enemies");
        Chat.Generate.Name("mario", "luigi", "brothers");
        Chat.Generate.Name("mario", "toad", "friends");
        Chat.Generate.Name("luigi", "toad", "active users");
        Chat.Generate.Name("waluigi", "waluigi", "waluigi");
        // example state //



        User CurrentUser = new User("luigi", "Placeholder");




        string[] var = Chat.Read.Name.Allowed(CurrentUser.Name, @"chats\");

        System.Threading.Thread.Sleep(5000);

        Chat.Display.Messages(var);

        Console.WriteLine(Chat.Read.Messages.FileName(CurrentUser.Name, var[0]));

        Console.ReadLine();
    }

    public class User
    {
        public string Name;
        public string Password;
        public string LoginName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

        public User(string NameInput, string PasswordInput) {
            Name = NameInput;
            Password = PasswordInput;
        }

        public static string Generate(User GeneratingUser) {
            bool Complete = false;
            bool FindNextPrime = true;
            int NoPrime = 0;

            while (!Complete) {
                if () {
                    
                }
            }

        }
    }

    public class Chat
    {
        public class Generate
        {
            public static void Name(string Name1, string Name2, string chatName = "") {
                string Name1Hex = Utilities.Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(Name1));
                string Name2Hex = Utilities.Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(Name2));

                string FileName = Utilities.Encode.WeaveStrings(Name1Hex, Name2Hex);
                Utilities.FileCreation(FileName, chatName);
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
                public static string[] Allowed(string username, string folder) {
                    string[][] ChatNames = Chat.Read.Name.All(folder);

                    string[] returnVal = new string[ChatNames.Length];

                    int j = 0;

                    int LengthVar = ChatNames.Length;

                    for (int i = 0; i < ChatNames.Length; i++) {
                        if (ChatNames[i].Contains(username)) {
                            if (ChatNames[i][0] == username) {
                                returnVal[j] = ChatNames[i][1];
                            }else if (ChatNames[i][1] == username) {
                                returnVal[j] = ChatNames[i][0];
                            }
                        } else {
                            returnVal[j] = "{RESTRICTEDCHATROOM}";
                            if (LengthVar > i) {
                                LengthVar = i;
                            }
                        }
                        j++;
                    }

                    string[] NewVal = new string[LengthVar];

                    for (int i = 0; i < LengthVar; i++) {
                        NewVal[i] = returnVal[i];
                    }

                    return NewVal;
                }
            }
            public class Messages
            {
                public static string FileName(string CurrentUser, string TargetChat) {

                    string HexCurrentUser = Utilities.Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(CurrentUser));
                    string HexTargetChat = Utilities.Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(TargetChat));

                    string ChatNameA = Utilities.Encode.WeaveStrings(HexCurrentUser, HexTargetChat);
                    string ChatNameB = Utilities.Encode.WeaveStrings(HexTargetChat, HexCurrentUser);

                    if (File.Exists(@"chats\" + ChatNameA + ".txt")) {
                        return ChatNameA;
                    } else if (File.Exists(@"chats\" + ChatNameB + ".txt")) {
                        return ChatNameB;
                    } else {
                        Console.WriteLine("That Chat Does Not Exist");
                        while (true) {
                            Console.Read();
                        }
                    }

                }
            }
            
        }

        public class Display
        {
            public static void Messages(string[] Content) {
                Board Board = new Board(30, 30);

                
                Board = Square.Create(new Square(30,30,Tuple.Create(0,0)), Board);

                // Console.WriteLine("Content.Length: " + Content.Length);

                for (int i = 0; i < Content.Length; i++) {
                    // Console.WriteLine("Content: " + Content[i] + "\ni: " + i);
                    Board = Text.Create(new Text(Tuple.Create(2,2+i), Content[i]), Board);
                }

                Board.Print(Board.smoothBoard(Board));
            }
        }
    }

    public class Utilities
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
        public static void FileCreation(string ChatName, string internalHeading) {
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
    }
    public class Encrypt
    {

    }
}
