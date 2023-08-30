using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SQLite;

using ObjectList;
using Security;
using DMSExtras;



class Program
{
    static void Main(string[] args)
    {
        // example state //
        User.Generate_Database();

        User TEMPUSER = new User("mario", "plumber", "mario");
        TEMPUSER.Generate_Chat("luigi");
        TEMPUSER.Generate_Chat("toad");

        TEMPUSER = new User("luigi", "player2", "luigi");
        TEMPUSER.Generate_Chat("wario");
        TEMPUSER.Generate_Chat("toad");

        TEMPUSER = new User("toad", "fungi", "toad");

        TEMPUSER = new User("waluigi", "waluigi", "waluigi");
        TEMPUSER.Generate_Chat("waluigi");
        // example state //



        User CurrentUser = new User("PlaceHolder", "Password");



        string[] var = CurrentUser.Read_Chats_Allowed(@"chats\");

        //System.Threading.Thread.Sleep(5000);

        User.Display.Messages(var);

        //Console.WriteLine(Chat.Read.Messages.FileName(CurrentUser.Name, var[0]));

        Console.ReadLine();
    }

    public class User
    {
        public string Username;
        public string Password;
        public string LoginName;
        public string PublicKey;
        public string PrivateKey;

        public User(string UsernameInput, string PasswordInput,
            string LoginNameInput = "TEMPVALUE") {

            Username = UsernameInput;
            Password = PasswordInput;

            LoginName = LoginNameInput == "TEMPVALUE" ?
                System.Security.Principal.WindowsIdentity.GetCurrent().Name :
                LoginNameInput;

            PublicKey = new DeterministicRSA(this.LoginName, this.Password).PublicKey;
            PrivateKey = new DeterministicRSA(this.LoginName, this.Password).PrivateKey;

            if (!User.Exists(Username)) {
                User.Generate_Database();

                string connectionString = "Data Source=UserList.db;Version=3;";
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                connection.Open();

                string sqlCommand = "INSERT INTO Main (Username, PublicKey) VALUES ('"+this.Username+"', '"+this.PublicKey+"');";
                SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);

                command = new SQLiteCommand(sqlCommand, connection);

                command.ExecuteNonQuery();
            }
        }



        public static bool Exists(string Value) {
            string connectionString = "Data Source=UserList.db;Version=3;";
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            string sqlCommand = "SELECT COUNT(*) from Main where Username = '"+Value+"'";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);

            command = new SQLiteCommand(sqlCommand, connection);

            command.ExecuteNonQuery();

            SQLiteCommand countCommand = new SQLiteCommand("SELECT COUNT(*) from Main where Username = '"+Value+"'", connection);
            bool isAllExist = Convert.ToInt32(countCommand.ExecuteScalar()) >= 1;

            return isAllExist;
        }



        public void Generate_Chat(string Name) {
            string Name1Hex = Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(this.Username));
            string Name2Hex = Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(Name));

            string FileName = Encode.WeaveStrings(Name1Hex, Name2Hex);
            DMSExtras.DMSExtras.FileCreation(FileName);
        }



        public static void Generate_Database() {
            if (!System.IO.File.Exists("UserList.db"))
            {
                string connectionString = @"Data Source=UserList.db;Version=3;";
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                connection.Open();

                string query = "CREATE TABLE Main (Username TEXT, PublicKey INTEGER)";
                SQLiteCommand command = new SQLiteCommand(query, connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }



        public string[] Read_Chats_Allowed(string folder) {
            string[][] ChatNames = Admin.Read.Chats.All(folder);

            string[] returnVal = new string[ChatNames.Length];

            int j = 0;

            int LengthVar = ChatNames.Length;

            for (int i = 0; i < ChatNames.Length; i++) {
                if (ChatNames[i].Contains(this.Username)) {
                    if (ChatNames[i][0] == this.Username) {
                        returnVal[j] = ChatNames[i][1];
                    }else if (ChatNames[i][1] == this.Username) {
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
        public string Read_Messages_FileName(string TargetChat) {

            string HexCurrentUser = Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(this.Username));
            string HexTargetChat = Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(TargetChat));

            string ChatNameA = Encode.WeaveStrings(HexCurrentUser, HexTargetChat);
            string ChatNameB = Encode.WeaveStrings(HexTargetChat, HexCurrentUser);

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



    public class Admin
    {

        public class Read
        {

            public class Chats
            {

                public static string[][] All(string folder) {
                    FileInfo[] files = new DirectoryInfo(folder).GetFiles("*.txt");

                    string[] fileNames = new string[files.Length];

                    for (int i = 0; i < fileNames.Length; i++) {
                        fileNames[i] = files[i].Name.Replace(".txt", "");
                    }

                    string[][] returnValHex = new string[fileNames.Length][];

                    for (int i = 0; i < fileNames.Length; i++) {
                        returnValHex[i] = Encode.UnravelStrings(fileNames[i]);
                    }

                    string[][] returnVal = new string[fileNames.Length][];
                    
                    for (int i = 0; i < fileNames.Length; i++) {
                        returnVal[i] = new string[2];
                        returnVal[i][0] = System.Text.Encoding.Default.GetString(Encode.HexStringToByteArray(returnValHex[i][0]));
                        returnVal[i][1] = System.Text.Encoding.Default.GetString(Encode.HexStringToByteArray(returnValHex[i][1]));
                    }

                    return returnVal;
                }
            }
        }
    }
}
