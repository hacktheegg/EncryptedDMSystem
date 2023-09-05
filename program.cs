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
        //System.Threading.Thread.Sleep(5000);

        // example state //
        User.Generate_Database();

        new User("mario", "plumber", "mario").Generate();
        new User("luigi", "player2", "luigi").Generate();
        new User("toad", "fungi", "toad").Generate();
        new User("waluigi", "waluigi", "waluigi").Generate();


        User TempUser = new User("mario", "plumber", "mario");
        TempUser.Generate_Chat("luigi");
        TempUser.Generate_Chat("toad");

        TempUser = new User("luigi", "player2", "luigi");
        TempUser.Generate_Chat("toad");

        TempUser = new User("toad", "fungi", "toad");

        TempUser = new User("waluigi", "waluigi", "waluigi");
        TempUser.Generate_Chat("waluigi");
        // example state //


        Start:
        Console.Clear();

        //Board Board = User.Display.New(20,20);
        Board Board = new Board(20, 20);
        Board = Square.Create(new Square(20,20,Tuple.Create(0,0)), Board);

        Menu Menu = new Menu(2, Tuple.Create(2,2));
        Menu.Values = new string[2]{"Login (Username)", "New"};

        Tuple<int, string> Result = Menu.Activate(Board);

        if (Result.Item1 == 1) {
            Console.WriteLine("Fresh Account");
            System.Threading.Thread.Sleep(5000);
            //Fresh Account

            goto Start;
        } else if (!User.Exists(Result.Item2.ToLower())) {
            Console.WriteLine("Not Valid Username");
            System.Threading.Thread.Sleep(5000);
            //Not Valid Account Username

            goto Start;
        }

        string TempString = Result.Item2.ToLower();

        //Board = User.Display.New(20,20);
        Board = new Board(20, 20);
        Board = Square.Create(new Square(20,20,Tuple.Create(0,0)), Board);

        Menu = new Menu(1, Tuple.Create(2,2));
        Menu.Values = new string[1]{"Password (Do Not Share)"};

        Result = Menu.Activate(Board);

        Board = new Board(20, 20);
        Board = Square.Create(new Square(20,20,Tuple.Create(0,0)), Board);

        User CurrentUser = new User(TempString, Result.Item2.ToLower(), "luigi");


        if (!(User.Exists(TempString) && User.Exists_PublicKey(CurrentUser.Key.Public))) {
            Console.WriteLine("Not The Correct Password");
            System.Threading.Thread.Sleep(5000);
            goto Start;
        }


        string[] var = CurrentUser.Read_Chats_Allowed(@"chats\");

        Menu = new Menu(var.Length, Tuple.Create(2,2));
        Menu.Values = var;

        Result = Menu.Activate(Board);

        string SelectedChat = Menu.Values[Result.Item1];

        SelectedChat = CurrentUser.Read_Messages_FileName(SelectedChat);

        string[] ChatContent = File.ReadLines(@"chats\"+SelectedChat+".txt").ToArray();

        string[] DecryptedChatContent = CurrentUser.Read_Messages_Chat(ChatContent);

        User.Display.Content(DecryptedChatContent);

        //User.Read_Messages_Chat();













        //User CurrentUser = new User("luigi", "player2", "luigi");



        //string[] var = CurrentUser.Read_Chats_Allowed(@"chats\");

            // //System.Threading.Thread.Sleep(5000);

        //User.Display.Messages(var);

        //Console.WriteLine(Chat.Read.Messages.FileName(CurrentUser.Name, var[0]));

        while (true) 
        {
            Console.Write("the end is never");
            Console.ReadKey(false);
        }
    }

    public class User
    {
        public string Username;
        public string Password;
        public string LoginName;
        public DeterministicRSA Key;

        public User(string UsernameInput, string PasswordInput,
            string LoginNameInput = "TEMPVALUE") {

            Username = UsernameInput;
            Password = PasswordInput;

            LoginName = LoginNameInput == "TEMPVALUE" ?
                System.Security.Principal.WindowsIdentity.GetCurrent().Name :
                LoginNameInput;

            Key = new DeterministicRSA(this.LoginName, this.Password);
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
            public static bool Exists_PublicKey(string Value) {
                string connectionString = "Data Source=UserList.db;Version=3;";
                SQLiteConnection connection = new SQLiteConnection(connectionString);
                connection.Open();

                string sqlCommand = "SELECT COUNT(*) from Main where PublicKey = '"+Value+"'";
                SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);

                command = new SQLiteCommand(sqlCommand, connection);

                command.ExecuteNonQuery();

                SQLiteCommand countCommand = new SQLiteCommand("SELECT COUNT(*) from Main where PublicKey = '"+Value+"'", connection);
                bool isAllExist = Convert.ToInt32(countCommand.ExecuteScalar()) >= 1;

                return isAllExist;
            }



        public void Generate() {
            if (User.Exists(this.Username) || User.Exists_PublicKey(this.Key.Public)) {
                return;
            }
            User.Generate_Database();

            string connectionString = "Data Source=UserList.db;Version=3;";
            SQLiteConnection connection = new SQLiteConnection(connectionString);
            connection.Open();

            string sqlCommand = "INSERT INTO Main (Username, PublicKey) VALUES ('"+this.Username+"', '"+this.Key.Public+"');";
            SQLiteCommand command = new SQLiteCommand(sqlCommand, connection);

            command = new SQLiteCommand(sqlCommand, connection);

            command.ExecuteNonQuery();
        }
            public void Generate_Chat(string Name) {
                string Name1Hex = Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(this.Username));
                string Name2Hex = Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(Name));

                string FileName = Encode.WeaveStrings(Name1Hex, Name2Hex);
                DMSExtras.DMSExtras.FileCreation(FileName);

                string SymmetricKey = SymmetricEncryption.GenerateKey();

                using (StreamWriter writer = new StreamWriter(@"chats\"+FileName+".txt"))
                {
                    writer.WriteLine(
                        this.Key.Encrypt("thisTheKey: "+ SymmetricKey,
                        User.Read_User_PublicKey(Name)
                    ));
                    writer.WriteLine(
                        this.Key.Encrypt("thisTheKey: "+ SymmetricKey,
                        User.Read_User_PublicKey(this.Username)
                    ));
                }
            }
            public static void Generate_Database() {
                if (!System.IO.File.Exists("UserList.db"))
                {
                    string connectionString = @"Data Source=UserList.db;Version=3;";
                    SQLiteConnection connection = new SQLiteConnection(connectionString);
                    connection.Open();

                    string query = "CREATE TABLE Main (Username TEXT, PublicKey TEXT)";
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

        public string[] Read_Messages_Chat(string[] EncryptedMessages) {
            string[] DecryptedMessages = new string[EncryptedMessages.Length];

            for (int i = 0; i < EncryptedMessages.Length; i++) {
                Console.WriteLine(this.Key.Private);

                DecryptedMessages[i] = this.Key.Decrypt(EncryptedMessages[i], this.Key.Private);
            }

            return DecryptedMessages;
        }
        public static void Write_Messages_Chat() {

        }
        public static string Read_User_PublicKey(string User) {
            var conn = new SQLiteConnection(@"Data Source=UserList.db;Version=3;");
            conn.Open();

            string stm = "SELECT PublicKey FROM Main WHERE Username = @username";
            var cmd = new SQLiteCommand(stm, conn);
            cmd.Parameters.AddWithValue("@username", User);

            SQLiteDataReader rdr = cmd.ExecuteReader();

            if (rdr.Read())
            {
                //Console.WriteLine(rdr.GetString(0));
                //System.Threading.Thread.Sleep(5000);
                return rdr.GetString(0);
            }
            else 
            {
                while (true) {
                    Console.WriteLine("error at chat Read_User_PublicKey");
                    Console.Read();
                }
            }
        }

        public class Display
        {
            public static void Content(string[] Content) {
                Board Board = new Board(20, 20);

                
                Board = Square.Create(new Square(20,20,Tuple.Create(0,0)), Board);

                // Console.WriteLine("Content.Length: " + Content.Length);

                for (int i = 0; i < Content.Length; i++) {
                    // Console.WriteLine("Content: " + Content[i] + "\ni: " + i);
                    Board = Text.Create(new Text(Tuple.Create(2,2+i), Content[i]), Board);
                }

                Board.Print(Board.smoothBoard(Board));
            }
            public static Board New(int Width, int Height) {
                Board Board = new Board(Width, Height);
                Board = Square.Create(new Square(Width,Height,Tuple.Create(0,0)), Board);
                return Board;
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
