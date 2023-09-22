using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

using ObjectList;
using Security;
using DMSExtras;
using BadWords;



class Program
{
    static void Main(string[] args)
    {

        
        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();

        string TypedText = "";
        int Pointer = int.MaxValue;



        User.Generate_Database();

        new User("BugReport", "BugReport", "BugReport").Generate();
        User TempUser = new User("BugReport", "BugReport", "BugReport");
        TempUser.Generate_Chat("BugReport");

        

        Console.WriteLine("    !!Epilepsy Warning!!");
        Console.WriteLine("This program flickers a lot when running\n");
        Console.WriteLine(
            "I am not responsible for what is said here\n"+
            "though there is some moderation, I can only moderate if\n"+
            "something is reported (Security is the priority/no report\nSystem yet)"
        );
        Console.ReadKey(true);
        Console.Clear();


        Start:
        
        TextListener.SetPointer(0);
        TextListener.SetTypedText(" ");
        TextListener.SetEndProgram(false);

        string[] Content = new string[3]{"Login", "New", "Public Room (404 not found)"};
        
        if (Pointer != int.MaxValue) {
            User.Display.With.Pointer(Content, 0);
        }

        TextListener.StartListening();
        while (!TextListener.GetEndProgram()) {
            if (Pointer != TextListener.GetPointer()) {
                // TypedText = TextListener.GetTypedText();
                Pointer = TextListener.GetPointer();
                if (Pointer >= Content.Length) {
                    TextListener.SetPointer(0);
                } else if (Pointer < 0) {
                    TextListener.SetPointer(Content.Length-1);
                }
                User.Display.With.Pointer(Content, Pointer);
            }
        }
        TextListener.StopListening();


        if (Pointer == 1) {
            string TempUsername;
            string TempPassword;

            BadNameUsername:
            TextListener.SetPointer(0);
            TextListener.SetTypedText(" ");
            TextListener.SetEndProgram(false);
            
            Content = new string[1]{"New Account Username: "};

            TextListener.StartListening();
            User.Display.Content(Content, TypedText);
            while (!TextListener.GetEndProgram()) {
                if (TypedText != TextListener.GetTypedText()) {
                    TypedText = TextListener.GetTypedText();
                    // Pointer = TextListener.GetPointer();
                    User.Display.Content(Content, TypedText);
                    //Console.WriteLine(TypedText);
                }
            }
            TextListener.StopListening();

            if (BadWords.BadWords.Retrieve().Any(TypedText.Contains)) {
                Console.WriteLine("bad Word Found, Redo Step");
                System.Threading.Thread.Sleep(5000);
                goto BadNameUsername;
            }



            TempUsername = TypedText;

            Content = new string[1]{"New Account Password: "};
            
            TextListener.SetPointer(0);
            TextListener.SetTypedText(" ");
            TextListener.SetEndProgram(false);

            TextListener.StartListening();
            while (!TextListener.GetEndProgram()) {
                if (TypedText != TextListener.GetTypedText()) {
                    TypedText = TextListener.GetTypedText();
                    Pointer = TextListener.GetPointer();
                    User.Display.Content(Content, TypedText);
                }
            }
            TextListener.StopListening();
            TempPassword = TypedText;

            // System.Threading.Thread.Sleep(100);

            new User(TempUsername, TempPassword).Generate();

            Console.WriteLine("Account Made");
            System.Threading.Thread.Sleep(5000);
            goto Start;
        } else if (Pointer == 2) {
            Console.WriteLine("Public Chat not exist yet");
            System.Threading.Thread.Sleep(5000);
            //Not Valid Account Username

            Pointer = 10;
            goto Start;
        }

        TextListener.SetPointer(0);
        TextListener.SetTypedText(" ");
        TextListener.SetEndProgram(false);
        Content = new string[1]{"Username"};

        TextListener.StartListening();
        while (!TextListener.GetEndProgram()) {
            if (TypedText != TextListener.GetTypedText()) {
                TypedText = TextListener.GetTypedText();
                Pointer = TextListener.GetPointer();
                User.Display.Content(Content, TypedText);
            }
        }
        TextListener.StopListening();
        
        string Username = TypedText;

        Content = new string[1]{"Password (Do Not Share)"};

        TextListener.SetPointer(0);
        TextListener.SetTypedText(" ");
        TextListener.SetEndProgram(false);

        // threadKeys.Start();
        TextListener.StartListening();
        while (!TextListener.GetEndProgram()) {
            if (TypedText != TextListener.GetTypedText()) {
                Console.Clear();
                TypedText = TextListener.GetTypedText();
                Pointer = TextListener.GetPointer();
                User.Display.Content(Content, TypedText);
            }
        }
        TextListener.StopListening();

        // threadKeys.Join();

        string Password = TypedText;

        User CurrentUser = new User(Username, Password);


        if (!(User.Exists(Username) && User.Exists_PublicKey(CurrentUser.Key.Public))) {
            Console.WriteLine("Not The Correct Password");
            System.Threading.Thread.Sleep(5000);
            goto Start;
        }


        string[] var = CurrentUser.Read_Chats_Allowed(@"chats\");
        string[] DisplayedChats = new string[9];

        int Multiplyer = 0;

        while (Multiplyer == 0 || Pointer == 7) {
            for (int i = 0; i < 7; i++) {
                if ((Multiplyer*7)+i < var.Length) {
                    DisplayedChats[i] = var[(Multiplyer*7)+i];
                } if ((Multiplyer*7)+i >= var.Length) { DisplayedChats[i] = "{EMPTY}"; }
            }
            DisplayedChats[7] = "Next (pg "+Multiplyer+"/"+Math.Ceiling((decimal)(var.Length/7))+")";
            DisplayedChats[8] = "New";

            

            // threadKeys.Start();

            TextListener.SetPointer(0);
            TextListener.SetTypedText(" ");
            TextListener.SetEndProgram(false);

            TextListener.StartListening();
            while (!TextListener.GetEndProgram()) {
                if (TypedText != TextListener.GetTypedText() || Pointer != TextListener.GetPointer()) {
                    Console.Clear();
                    TypedText = TextListener.GetTypedText();
                    Pointer = TextListener.GetPointer();
                    if (Pointer >= DisplayedChats.Length) {
                        TextListener.SetPointer(0);
                    } else if (Pointer < 0) {
                        TextListener.SetPointer(DisplayedChats.Length-1);
                    }
                    User.Display.With.Pointer(DisplayedChats, Pointer, TypedText);
                }
            }
            TextListener.StopListening();

            // threadKeys.Join();



            Multiplyer++;
        }

        var = Admin.Read.Users.All();
        Multiplyer = 0;

        while (Pointer == 8 || Pointer == 7) {
            for (int i = 0; i < 7; i++) {
                if ((Multiplyer*7)+i < var.Length) {
                    DisplayedChats[i] = var[(Multiplyer*7)+i];
                } if ((Multiplyer*7)+i >= var.Length) { DisplayedChats[i] = "{EMPTY}"; }
            }
            DisplayedChats[7] = "Next (pg "+Multiplyer+"/"+Math.Ceiling((decimal)(var.Length/7))+")";
            DisplayedChats[8] = "  Choose Chat to Create";

            // threadKeys.Start();

            TextListener.SetPointer(0);
            TextListener.SetTypedText(" ");
            TextListener.SetEndProgram(false);

            TextListener.StartListening();
            while (!TextListener.GetEndProgram()) {
                if (TypedText != TextListener.GetTypedText() || Pointer != TextListener.GetPointer()) {
                    Console.Clear();
                    TypedText = TextListener.GetTypedText();
                    Pointer = TextListener.GetPointer();
                    if (Pointer >= DisplayedChats.Length) {
                        TextListener.SetPointer(0);
                    } else if (Pointer < 0) {
                        TextListener.SetPointer(DisplayedChats.Length-1);
                    }
                    User.Display.With.Pointer(DisplayedChats, Pointer, TypedText);
                }
            }
            TextListener.StopListening();

            // threadKeys.Join();

            if (Pointer == 7) {
                Multiplyer++;
            } else {
                CurrentUser.Generate_Chat(DisplayedChats[Pointer]);
            }
        }



        string SelectedChat = DisplayedChats[Pointer];

        SelectedChat = CurrentUser.Read_Messages_FileName(SelectedChat);



        string[] DecryptedChatContent = CurrentUser.Read_Messages_Chat(SelectedChat);

        //User.Display.Content(DecryptedChatContent);

        Tuple<string, string> SymmetricKey = CurrentUser.Read_Chat_SymmetricKey(SelectedChat);

        



        /* Utilities.KeyListener KeyListener = new Utilities.KeyListener();

        Thread threadKeys = new Thread(() =>
        {
            KeyListener.StartListening();
        }); */

        DMSExtras.DMSExtras.ChatListener ChatListener = new DMSExtras.DMSExtras.ChatListener();

        Thread threadChat = new Thread(() =>
        {
            ChatListener.StartListening(SelectedChat);
        });

        threadChat.Start();
        
        // threadKeys.Start();

        TypedText = ""; // Move this declaration outside the while loop
        string[] ChatMessages = {"TEMPVALUE", "TEMPVALUE"};

        TextListener.SetPointer(0);
        TextListener.SetTypedText(" ");
        TextListener.SetEndProgram(false);

        TextListener.StartListening();
        while (true)
        {

            Thread.Sleep(100); // Sleep for a short duration to avoid excessive CPU usage

            //Console.WriteLine(KeyListener.GetTypedText().ToLower() + "\n" + TypedText);
                
            //for (int i = 0; i < ChatMessages.Length; i++) { Console.WriteLine(ChatMessages[i]); }
            //Console.WriteLine("eeee");
            //for (int i = 0; i < ChatListener.GetChatContent().Length; i++) { Console.WriteLine(ChatListener.GetChatContent()[i]); }
            //Console.WriteLine("eeee");

            if (TextListener.GetEndProgram()) {
                if (!string.IsNullOrEmpty(TypedText)) {
                    // Console.WriteLine(Encode.ByteArrayToHexString(Encoding.ASCII.GetBytes(TypedText)));

                    if (BadWords.BadWords.Retrieve().Any(TypedText.Contains)) {
                        Console.WriteLine("bad Word Found, Message not Sent");
                        System.Threading.Thread.Sleep(1000);
                    } else {
                    CurrentUser.Write_Messages_Chat(SymmetricKey, SelectedChat, TypedText);

                    TextListener.StopListening();
                    TextListener.SetTypedText("");
                    TextListener.SetEndProgram(false);
                    TextListener.StartListening();
                    }
                }
            }

            if ((TypedText != TextListener.GetTypedText()) || !ChatMessages.SequenceEqual(ChatListener.GetChatContent())) {

                TypedText = TextListener.GetTypedText();
                ChatMessages = ChatListener.GetChatContent();

                User.Display.Content(CurrentUser.Read_Messages_Chat(SelectedChat),TypedText,true);
            }


            // Write_Messages_Chat(Tuple<string, string> SymmetricKey, string ChatName, string Content)
        }



        // User.Write_Messages_Chat(SymmetricKey, SelectedChat, "hello, this is a new message");















/*
DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();

string TypedText = "";
int Pointer = int.MaxValue;

Thread threadKeys = new Thread(() => {
    TextListener.StartListening();
});

threadKeys.Start();

while (!TextListener.GetEndProgram()) {
    if (TypedText != TextListener.GetTypedText() || Pointer != TextListener.GetPointer()) {
            TypedText = TextListener.GetTypedText();
            Pointer = TextListener.GetPointer();
        if (Pointer >= Content.Length) {
            TextListener.SetPointer(0);
        } else if (Pointer < 0) {
            TextListener.SetPointer(Content.Length-1);
        }
        User.Display.With.Pointer(Content, Pointer, TypedText);
    }
}

TextListener.SetPointer(0);
TextListener.SetTypedText(" ");
TextListener.SetEndProgram(false);

while (!TextListener.GetEndProgram()) {
    if (TypedText != TextListener.GetTypedText()) {
        TypedText = TextListener.GetTypedText();
        Pointer = TextListener.GetPointer();
        User.Display.Content(Content, TypedText);
    }
}
*/      











        //User.Read_Messages_Chat();













        //User CurrentUser = new User("luigi", "player2", "luigi");



        //string[] var = CurrentUser.Read_Chats_Allowed(@"chats\");

            // //System.Threading.Thread.Sleep(5000);

        //User.Display.Messages(var);

        //Console.WriteLine(Chat.Read.Messages.FileName(CurrentUser.Name, var[0]));

        // while (true) 
        // {
            // Console.Write("the end is never");
            // Console.ReadKey(false);
        // } */
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

                using (StreamWriter writer = new StreamWriter(@"chats\"+FileName+".txt"))
                {
                    writer.WriteLine(         Name+":"+ this.Key.Encrypt(SymmetricKey.Item1+":"+SymmetricKey.Item2, User.Read_User_PublicKey(Name))
                    );
                    //writer.WriteLine(Name);
                    //writer.WriteLine(User.Read_User_PublicKey(Name));
                    writer.WriteLine(this.Username+":"+ this.Key.Encrypt(SymmetricKey.Item1+":"+SymmetricKey.Item2, User.Read_User_PublicKey(this.Username))
                    );
                    //writer.WriteLine(this.Username);
                    //writer.WriteLine(User.Read_User_PublicKey(this.Username));
                    /*writer.WriteLine(
                        SymmetricEncryption.EncryptString(SymmetricKey.Item1, this.Username+"   "+Name, SymmetricKey.Item2)
                    );
                    writer.WriteLine(
                        SymmetricEncryption.EncryptString(SymmetricKey.Item1, this.Username+"   "+Name, SymmetricKey.Item2)
                    );
                    writer.WriteLine(
                        SymmetricEncryption.EncryptString(SymmetricKey.Item1, this.Username+"   "+Name, SymmetricKey.Item2)
                    );*/
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

            /*
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
            */
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
            string SymmetricKey;
            string iv;

            string[] EncryptedMessages = File.ReadLines(@"chats\"+SelectedChat+".txt").ToArray();

            if (EncryptedMessages[0].StartsWith(this.Username)) {
                SymmetricKey = this.Key.Decrypt(EncryptedMessages[0].Split(':')[1], this.Key.Private).Split(':')[0];
                iv = this.Key.Decrypt(EncryptedMessages[0].Split(':')[1], this.Key.Private).Split(':')[1];
            } else {
                SymmetricKey = this.Key.Decrypt(EncryptedMessages[1].Split(':')[1], this.Key.Private).Split(':')[0];
                iv = this.Key.Decrypt(EncryptedMessages[1].Split(':')[1], this.Key.Private).Split(':')[1];
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
                public static void Pointer(string[] Content, int pointer, string TypedText = "") {
                    Board Board = new Board(20, 20);
                    Board = Square.Create(new Square(20,20,Tuple.Create(0,0)), Board);

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
            public static void Content(string[] Content, string TypedText = "", bool IfChat = false) {
                Board Board = new Board(20, 20);
                Board = Square.Create(new Square(20,20,Tuple.Create(0,0)), Board);
                
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
