using DMSExtras;
using ObjectList;
using Security;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

namespace User
{
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
            if (User.Exists(this.Username)) {
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
            connection.Close();
        }
            public void Generate_Chat(string Name) {
                string Name1Hex = Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(this.Username));
                string Name2Hex = Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(Name));

                string FileName = Encode.WeaveStrings(Name1Hex, Name2Hex);
                DMSExtras.DMSExtras.FileCreation(FileName);

                Tuple<string,string> SymmetricKey = SymmetricEncryption.GenerateKeyAndIV();

                // using (StreamWriter writer = new StreamWriter(@"chats\Keys.txt"))
                // {
                //     writer.WriteLine(SymmetricKey.Item1);
                //     writer.WriteLine(SymmetricKey.Item2);
                // }

                using (StreamWriter writer = new StreamWriter(@"chats\"+FileName+".txt"))
                {
                    writer.WriteLine(
                        Name+
                        ":"+// SymmetricKey.Item1+
                        Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(
                            this.Key.Encrypt(SymmetricKey.Item1, User.Read_User_PublicKey(Name))
                        ))+
                        ":"+// SymmetricKey.Item2
                        Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(
                            this.Key.Encrypt(SymmetricKey.Item2, User.Read_User_PublicKey(Name))
                        ))
                    );

                    // User.Read_User_PublicKey(this.Username);

                    writer.WriteLine(
                        this.Username+
                        ":"+// SymmetricKey.Item1+
                        Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(
                            this.Key.Encrypt(SymmetricKey.Item1, User.Read_User_PublicKey(this.Username))
                        ))+
                        ":"+// SymmetricKey.Item2
                        Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(
                            this.Key.Encrypt(SymmetricKey.Item2, User.Read_User_PublicKey(this.Username))
                        ))
                    );

                    // User.Read_User_PublicKey(Name);
                }

                using (StreamWriter writer = new StreamWriter(@"chats\Write.txt"))
                {
                    writer.WriteLine(SymmetricKey.Item1);
                    writer.WriteLine(SymmetricKey.Item2);
                }
            }
            public static void Generate_Database() {
                while (!System.IO.File.Exists("UserList.db"))
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

            int j = 0;

            for (int i = 0; i < ChatNames.Length; i++) {
                if (ChatNames[i].Contains(this.Username)) {
                    j++;
                }
            }

            string[] ReturnVal = new string[j];

            j = 0;

            for (int i = 0; i < ChatNames.Length; i++) {
                if (ChatNames[i][0] == this.Username) {
                    ReturnVal[j] = ChatNames[i][1];
                    j++;
                } else if (ChatNames[i][1] == this.Username) {
                    ReturnVal[j] = ChatNames[i][0];
                    j++;
                }
            }

            return ReturnVal;
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

        public string[] Read_Messages_Chat(string Chat) {
            string[] DecryptedMessages = new string[File.ReadLines(@"chats\"+Chat+".txt").ToArray().Length-2];

            
            Tuple<string,string> SymmetricKey = this.Read_Chat_SymmetricKey(Chat);
            

            // for (int i = File.ReadLines(@"chats\"+Chat+".txt").ToArray().Length-1; i >= 2; i--) {

            // }


            for (int i = 2; i < File.ReadLines(@"chats\"+Chat+".txt").ToArray().Length; i++) {

                DecryptedMessages[i-2] = SymmetricEncryption.DecryptString(SymmetricKey.Item1, File.ReadLines(@"chats\"+Chat+".txt").ToArray()[i], SymmetricKey.Item2);

            }

            return DecryptedMessages;
        }





        public Tuple<string, string> Read_Chat_SymmetricKey(string SelectedChat) {
            System.Threading.Thread.Sleep(500);
            string SymmetricKey;
            string iv;

            string[] EncryptedMessages = File.ReadLines(@"chats\"+SelectedChat+".txt").ToArray();

            // SymmetricKey = EncryptedMessages[0];
            // iv = EncryptedMessages[1];

            if (EncryptedMessages[0].StartsWith(this.Username)) {

                SymmetricKey = EncryptedMessages[0].Split(':')[1];

                iv = EncryptedMessages[0].Split(':')[2];

            } else {
                
                SymmetricKey = EncryptedMessages[1].Split(':')[1];

                iv = EncryptedMessages[1].Split(':')[2];

            }



            byte[] encryptedData = Encode.HexStringToByteArray(SymmetricKey);
            string encryptedDataString = Convert.ToBase64String(encryptedData);
            string decryptedData = this.Key.Decrypt(encryptedDataString, this.Key.Private);
            SymmetricKey = decryptedData;

            encryptedData = Encode.HexStringToByteArray(iv);
            encryptedDataString = Convert.ToBase64String(encryptedData);
            decryptedData = this.Key.Decrypt(encryptedDataString, this.Key.Private);
            iv = decryptedData;




            // SymmetricKey = Encoding.ASCII.GetString(Encode.HexStringToByteArray(SymmetricKey));
            
            // iv = Encoding.ASCII.GetString(Encode.HexStringToByteArray(iv));

            // SymmetricKey = this.Key.Decrypt(SymmetricKey, this.Key.Private);

            // iv = this.Key.Decrypt(iv, this.Key.Private);


            // Console.WriteLine(SymmetricKey);
            // Console.WriteLine(iv);

            using (StreamWriter writer = new StreamWriter(@"chats\Read.txt"))
            {
                writer.WriteLine(SymmetricKey);
                writer.WriteLine(iv);
            }

            return new Tuple<string,string>(SymmetricKey,iv);
        }





        public void Write_Messages_Chat(Tuple<string, string> SymmetricKey, string ChatName, string Content) {
            //SymmetricEncryption.EncryptString(SymmetricKey.Item1, this.Username+"   "+Name, SymmetricKey.Item2);
            File.AppendAllText(@"chats\"+ChatName+".txt", "\n"+SymmetricEncryption.EncryptString(SymmetricKey.Item1, "["+this.Username+"] "+Content, SymmetricKey.Item2));
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
                string TempString = rdr.GetString(0);
                rdr.Close();
                Console.WriteLine(TempString);
                // System.Threading.Thread.Sleep(5000);
                // System.Threading.Thread.Sleep(5000);
                // System.Threading.Thread.Sleep(5000);
                return TempString;
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
            public class With
            {
                public static void Pointer(string[] Content, Tuple<int, int> BoardDimensions, int pointer, string TypedText = "") {
                    Board Board = new Board(BoardDimensions.Item1, BoardDimensions.Item2);
                    Board = Square.Create(new Square(Board.Width,Board.Height,Tuple.Create(0,0)), Board);

                    // Text Text = new Text(Tuple.Create(0,0), "");

                    for (int i = 0; i < Content.Length; i++) {
                        
                        if (pointer == i) {
                            // Console.WriteLine("yes");
                            Board = Text.Create(new Text(Tuple.Create(2, 2+i), "><"+Content[i]), Board);
                        } else {
                            // Console.WriteLine("no");
                            Board = Text.Create(new Text(Tuple.Create(2, 2+i), "[]"+Content[i]), Board);
                        }
                    }

                    // Console.WriteLine(TypedText);

                    Board = Text.Create(new Text(Tuple.Create(1,1), TypedText), Board);

                    Board.Print(Board.smoothBoard(Board), true);
                }

            }
            public static void Content(string[] Content, Tuple<int, int> BoardDimensions, string TypedText = "", bool IfChat = false) {
                Board Board = new Board(BoardDimensions.Item1, BoardDimensions.Item2);
                Board = Square.Create(new Square(Board.Width,Board.Height,Tuple.Create(0,0)), Board);
                
                if (IfChat) {
                    for (int i = Content.Length-1; i >= 0; i--) {
                        Board = Text.Create(new Text(Tuple.Create(2,2-(i-(Content.Length-1))), Content[i]), Board);
                    }
                } else {
                    for (int i = 0; i < Content.Length; i++) {
                        Board = Text.Create(new Text(Tuple.Create(2,2+i), Content[i]), Board);
                    }
                }

                Board = Text.Create(new Text(Tuple.Create(1,1), TypedText), Board);

                Board.Print(Board.smoothBoard(Board), true);
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
            public class Users
            {
                public static string[] All() {
                    var conn = new SQLiteConnection(@"Data Source=UserList.db;Version=3;");
                    conn.Open();

                    string stm = "SELECT Username FROM Main";
                    var cmd = new SQLiteCommand(stm, conn);

                    SQLiteDataReader rdr = cmd.ExecuteReader();

                    List<string> usernames = new List<string>();
                    while (rdr.Read())
                    {
                        usernames.Add(rdr.GetString(0));
                    }
                    rdr.Close();
                    return usernames.ToArray();
                }
            }
        }
    }
}