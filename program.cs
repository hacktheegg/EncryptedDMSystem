using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;

using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;

using ObjectList;
using Security;
using DMSExtras;
using BadWords;
using User;



class Program
{
    static void Main(string[] args)
    {
        int BoardWidth = 25;
        int BoardHeight = 25;

        Tuple<int,int> BoardDimensions = new Tuple<int, int>(BoardWidth, BoardHeight);



        User.User.Generate_Database();

        new User.User("BugReport", "BugReport", "BugReport").Generate();
        User.User TempUser = new User.User("BugReport", "BugReport", "BugReport");
        //TempUser.Generate_Chat("BugReport");

        

        Console.WriteLine("    !!Epilepsy Warning!!");
        Console.WriteLine("This program flickers when running\n");
        Console.WriteLine(
            "I am not responsible for what is said here\n"+
            "due to lack of moderation tools at this moment\n"+
            "(and I cannot access instances remotely due to\n"+
            "not wanting to do networking)\n"
        );
        Console.WriteLine("New updates can be found at: https://github.com/hacktheegg/EncryptedDMSystem/releases");
        Console.WriteLine("Check there regularly for newer versions (I don't have anything better to do)");
        Console.WriteLine("Version: 1.4.0");
        Console.ReadKey(true);
        Console.Clear();


        Start:
        

        string[] Content = new string[2]{"Login", "New"};

        Tuple<int, string> Tuple = MenuLoop(Content, BoardDimensions, false);


        if (Tuple.Item1 == 1) {
            string TempUsername;
            string TempPassword;

            BadNameUsername:
            
            Content = new string[1]{"New Account Username: "};

            Tuple = MenuLoop(Content, BoardDimensions, true);

            if (BadWords.BadWords.list.Any(Tuple.Item2.ToLower().Contains)) {
                Console.WriteLine("bad Word Found, Redo Step (wait a sec)");
                System.Threading.Thread.Sleep(3000);
                goto BadNameUsername;
            }

            TempUsername = Tuple.Item2;

            Content = new string[1]{"New Account Password: "};
            
            Tuple = MenuLoop(Content, BoardDimensions, true);

            TempPassword = Tuple.Item2;

            new User.User(TempUsername, TempPassword).Generate();

            Console.WriteLine("Account Made (wait a sec)");
            System.Threading.Thread.Sleep(3000);
            goto Start;
        }

        Content = new string[1]{"Username"};

        Tuple = MenuLoop(Content, BoardDimensions, true);
        
        string Username = Tuple.Item2;

        Content = new string[1]{"Password (Do Not Share)"};

        Tuple = MenuLoop(Content, BoardDimensions, true);


        string Password = Tuple.Item2;

        User.User CurrentUser = new User.User(Username, Password);


        if (!(User.User.Exists(Username) && User.User.Exists_PublicKey(CurrentUser.Key.Public))) {
            Console.WriteLine("Not The Correct Password");
            Console.WriteLine("Login Timeout: 5");
            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Login Timeout: 4");
            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Login Timeout: 3");
            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Login Timeout: 2");
            System.Threading.Thread.Sleep(500);
            Console.WriteLine("Login Timeout: 1");
            System.Threading.Thread.Sleep(500);
            goto Start;
        }

        Console.WriteLine("Correct Password");



        while (true) {
            Content = new string[3]{"Private Chat", "Public Chat", "Settings"};
            BoardDimensions = CurrentUser.Read_User_BoardSettings();
            Tuple = MenuLoop(Content, BoardDimensions, false);

            if (Tuple.Item1 == 0) {
                PrivateChat(CurrentUser);
            } else if (Tuple.Item1 == 1) {
                PublicChat(CurrentUser);
            } else if (Tuple.Item1 == 2) {
                ChangeSettings(CurrentUser);
            }
        }
    }



    public static void PublicChat(User.User CurrentUser) {
        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();
        Tuple<int, string> Tuple = new Tuple<int, string>(0, "");
        Tuple<int,int> BoardDimensions = CurrentUser.Read_User_BoardSettings();



        ChooseChat:
        bool ChooseChatLoop = true;

        string[] var = Admin.Read.Chats.Public();
        Array.Sort(var);

        int Multiplyer = 0;
        int Step = BoardDimensions.Item2-6;
        
        string[] DisplayedChats = new string[Step+2];

        while (!TextListener.GetESCPressed() && (ChooseChatLoop || Tuple.Item1 == Step || Tuple.Item1 == Step+1)) {
            ChooseChatLoop = false;
            for (int i = 0; i < Step-1; i++) {
                if ((Multiplyer*Step)+i < var.Length) {
                    DisplayedChats[i] = var[(Multiplyer*Step)+i];
                }
                if ((Multiplyer*Step)+i >= var.Length) {
                    DisplayedChats[i] = "{EMPTY}";
                }
            }
            DisplayedChats[Step+1] = "Previous";
            DisplayedChats[Step] = "Next (pg "+Multiplyer+"/"+Math.Ceiling((decimal)(var.Length/Step))+")";
            DisplayedChats[Step-1] = "New Chat";

            Tuple = MenuLoop(DisplayedChats, BoardDimensions, true);

            if (Tuple.Item1 == Step) {
                Multiplyer++;
            } else if (Tuple.Item1 == Step+1 && Multiplyer > 0) {
                Multiplyer--;
            }
        }

        var = Admin.Read.Chats.Public();
        Array.Sort(var);
        Multiplyer = 0;

        while (!TextListener.GetESCPressed() && (Tuple.Item1 == Step-1 || Tuple.Item1 == Step || Tuple.Item1 == Step+1)) {
            for (int i = 0; i < Step-1; i++) {
                if ((Multiplyer*Step)+i < var.Length) {
                    DisplayedChats[i] = var[(Multiplyer*Step)+i];
                } if ((Multiplyer*Step)+i >= var.Length) {
                    DisplayedChats[i] = "{EMPTY}";
                }
            }
            DisplayedChats[Step-1] = "Go Back";
            DisplayedChats[Step] = "Next (pg "+Multiplyer+"/"+Math.Ceiling((decimal)(var.Length/Step))+")";
            DisplayedChats[Step+1] = "Previous";

            Tuple = MenuLoop(DisplayedChats, BoardDimensions, true);

            if (Tuple.Item1 == Step) {
                Multiplyer++;
            } else if (Tuple.Item1 == Step+1) {
                Multiplyer--;
            } else if (Tuple.Item1 == Step-1) {
                goto ChooseChat;
            } else {
                CurrentUser.Generate_Chat(DisplayedChats[Tuple.Item1]);
            }
        }
        string SelectedChat = DisplayedChats[Tuple.Item1];
        if (!TextListener.GetESCPressed()) {
            TextListener.StopListening();
            PublicChatLoop(SelectedChat, CurrentUser);
        } else {
            TextListener.StopListening();
        }
    }


    public static void PublicChatLoop(string SelectedChat, User.User CurrentUser) {
        Tuple<int,int> BoardDimensions = CurrentUser.Read_User_BoardSettings();
        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();
        DMSExtras.DMSExtras.ChatListener ChatListener = new DMSExtras.DMSExtras.ChatListener();
        bool UpdateBoard = false;
        int Pointer = 0;

        string TypedText = " ";

        Thread threadChat = new Thread(() =>
        {
            ChatListener.StartListening(@"chatsPublic\"+SelectedChat+".txt");
        });

        threadChat.Start();
        
        // threadKeys.Start();

        TypedText = ""; // Move this declaration outside the while loop
        string[] ChatMessages = {"TEMPVALUE", "TEMPVALUE"};

        TextListener.SetPointer(0);
        TextListener.SetTypedText(" ");
        TextListener.SetEnterPressed(false);

        TextListener.StartListening();
        while (!TextListener.GetESCPressed())
        {

            Thread.Sleep(50);

            if (TextListener.GetEnterPressed() && !string.IsNullOrEmpty(TypedText)) {
                    
                if (BadWords.BadWords.list.Any(TypedText.ToLower().Contains)) {
                    Console.WriteLine("bad Word Found, Message not Sent");
                    System.Threading.Thread.Sleep(1000);
                    TextListener.StopListening();
                    TextListener.SetEnterPressed(false);
                    TextListener.StartListening();
                } else {
                    WriteToChatPublic:
                    try {
                        File.AppendAllText(@"chatsPublic\"+SelectedChat+".txt", "\n"+"["+CurrentUser.Username+"] "+TypedText);
                    } catch {
                        goto WriteToChatPublic;
                    }

                    TextListener.StopListening();
                    TextListener.SetTypedText("");
                    TextListener.SetPointer(0);
                    TextListener.SetEnterPressed(false);
                    TextListener.StartListening();
                }
            }

            if (TypedText != TextListener.GetTypedText()) {
                TypedText = TextListener.GetTypedText();
                UpdateBoard = true;
            }

            if (Pointer != TextListener.GetPointer()) {
                if (TextListener.GetPointer() < 0) {
                    TextListener.StopListening();
                    TextListener.SetPointer(0);
                    TextListener.StartListening();
                }
                UpdateBoard = true;
                Pointer = TextListener.GetPointer();
            }

            if (!ChatMessages.SequenceEqual(ChatListener.GetChatContent())) {
                ChatMessages = ChatListener.GetChatContent();
                UpdateBoard = true;
            }

            if (UpdateBoard) {
                User.User.Display.Content(ChatMessages,BoardDimensions,TypedText, true, SelectedChat, Pointer);
                UpdateBoard = false;
            }
        }



        TextListener.StopListening();
    }








    public static void PrivateChat(User.User CurrentUser) {
        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();
        Tuple<int, string> Tuple = new Tuple<int, string>(0, "");
        Tuple<int,int> BoardDimensions = CurrentUser.Read_User_BoardSettings();

        ChooseChat:
        bool ChooseChatLoop = true;

        string[] var = CurrentUser.Read_Chats_Allowed(@"chats\");
        Array.Sort(var);

        int Multiplyer = 0;
        int Step = BoardDimensions.Item2-6;
        
        string[] DisplayedChats = new string[Step+2];

        while (!TextListener.GetESCPressed() && (ChooseChatLoop || Tuple.Item1 == Step || Tuple.Item1 == Step+1)) {
            ChooseChatLoop = false;
            for (int i = 0; i < Step-1; i++) {
                if ((Multiplyer*Step)+i < var.Length) {
                    DisplayedChats[i] = var[(Multiplyer*Step)+i];
                }
                if ((Multiplyer*Step)+i >= var.Length) {
                    DisplayedChats[i] = "{EMPTY}";
                }
            }
            DisplayedChats[Step+1] = "Previous";
            DisplayedChats[Step] = "Next (pg "+Multiplyer+"/"+Math.Ceiling((decimal)(var.Length/Step))+")";
            DisplayedChats[Step-1] = "New Chat";

            Tuple = MenuLoop(DisplayedChats, BoardDimensions, true);

            if (Tuple.Item1 == Step) {
                Multiplyer++;
            } else if (Tuple.Item1 == Step+1 && Multiplyer > 0) {
                Multiplyer--;
            }
        }

        var = Admin.Read.Users.All();
        Array.Sort(var);
        Multiplyer = 0;

        while (!TextListener.GetESCPressed() && (Tuple.Item1 == Step-1 || Tuple.Item1 == Step || Tuple.Item1 == Step+1)) {
            for (int i = 0; i < Step-1; i++) {
                if ((Multiplyer*Step)+i < var.Length) {
                    DisplayedChats[i] = var[(Multiplyer*Step)+i];
                } if ((Multiplyer*Step)+i >= var.Length) {
                    DisplayedChats[i] = "{EMPTY}";
                }
            }
            DisplayedChats[Step-1] = "Go Back";
            DisplayedChats[Step] = "Next (pg "+Multiplyer+"/"+Math.Ceiling((decimal)(var.Length/Step))+")";
            DisplayedChats[Step+1] = "Previous";

            Tuple = MenuLoop(DisplayedChats, BoardDimensions, true);

            if (Tuple.Item1 == Step) {
                Multiplyer++;
            } else if (Tuple.Item1 == Step+1) {
                Multiplyer--;
            } else if (Tuple.Item1 == Step-1) {
                goto ChooseChat;
            } else {
                CurrentUser.Generate_Chat(DisplayedChats[Tuple.Item1]);
            }
        }
        if (!TextListener.GetESCPressed()) {
            if (DisplayedChats[Tuple.Item1] == "BugReport") {
                Console.WriteLine("submit bugs here, https://github.com/hacktheegg/EncryptedDMSystem/issues/new/choose");
                Console.ReadKey();
                Console.ReadKey();
            }
            string SelectedChat = DisplayedChats[Tuple.Item1];
            string OtherUser = SelectedChat;
            SelectedChat = CurrentUser.Read_Messages_FileName(SelectedChat);
            string[] ChatContent = File.ReadLines(@"chats\"+SelectedChat+".txt").ToArray();
            string[] DecryptedChatContent = CurrentUser.Read_Messages_Chat(ChatContent);
            Tuple<string, string> SymmetricKey = CurrentUser.Read_Chat_SymmetricKey(ChatContent);
            TextListener.StopListening();
            PrivateChatLoop(SelectedChat, BoardDimensions, CurrentUser, SymmetricKey, OtherUser);
        } else {
            TextListener.StopListening();
        }
    }



    public static Tuple<int, string> MenuLoop(string[] Content, Tuple<int,int> BoardDimensions, bool GetTypedText = true) {

        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();
        string TypedText = "";
        int Pointer = int.MaxValue;

        TextListener.SetPointer(0);
        TextListener.SetTypedText("  ");
        TextListener.SetEnterPressed(false);



        TextListener.StartListening();
        while (!TextListener.GetEnterPressed() && !TextListener.GetESCPressed()) {

            if (
                Pointer != TextListener.GetPointer()
                ||
                TypedText != TextListener.GetTypedText()
            ) {
                
                Pointer = TextListener.GetPointer();
                TypedText = TextListener.GetTypedText();

                if (Pointer >= Content.Length) {
                    TextListener.SetPointer(0);
                } else if (Pointer < 0) {
                    TextListener.SetPointer(Content.Length-1);
                }

                if (Content.Length > 1) {
                    User.User.Display.With.Pointer(Content, BoardDimensions, Pointer);
                } else if (GetTypedText) {
                    User.User.Display.Content(Content, BoardDimensions, TypedText);
                }

            }
        }
        TextListener.StopListening();
        return new Tuple<int, string>(Pointer, TypedText);
    }

    public static void PrivateChatLoop(string SelectedChat, Tuple<int, int> BoardDimensions, User.User CurrentUser, Tuple<string, string> SymmetricKey, string OtherUser) {
        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();
        DMSExtras.DMSExtras.ChatListener ChatListener = new DMSExtras.DMSExtras.ChatListener();
        bool UpdateBoard = false;
        int Pointer = 0;

        string TypedText = " ";

        Thread threadChat = new Thread(() =>
        {
            ChatListener.StartListening(@"chats\"+SelectedChat+".txt");
        });

        threadChat.Start();
        
        // threadKeys.Start();

        TypedText = ""; // Move this declaration outside the while loop
        string[] ChatMessages = {"TEMPVALUE", "TEMPVALUE"};

        string[] DecryptedMessages = {"TEMPVALUE", "TEMPVALUE"};

        TextListener.SetPointer(0);
        TextListener.SetTypedText(" ");
        TextListener.SetEnterPressed(false);

        TextListener.StartListening();
        while (!TextListener.GetESCPressed())
        {

            Thread.Sleep(50);

            if (TextListener.GetEnterPressed() && !string.IsNullOrEmpty(TypedText)) {
                    
                    if (BadWords.BadWords.list.Any(TypedText.ToLower().Contains)) {
                        Console.WriteLine("bad Word Found, Message not Sent");
                        System.Threading.Thread.Sleep(1000);
                        TextListener.StopListening();
                        TextListener.SetEnterPressed(false);
                        TextListener.StartListening();
                    } else {
                        CurrentUser.Write_Messages_Chat(SymmetricKey, SelectedChat, "["+CurrentUser.Username+"] "+TypedText);

                        TextListener.StopListening();
                        TextListener.SetTypedText("");
                        TextListener.SetPointer(0);
                        TextListener.SetEnterPressed(false);
                        TextListener.StartListening();
                    }
            }

            if (TypedText != TextListener.GetTypedText()) {
                TypedText = TextListener.GetTypedText();
                UpdateBoard = true;
            }

            if (Pointer != TextListener.GetPointer()) {
                if (TextListener.GetPointer() < 0) {
                    TextListener.StopListening();
                    TextListener.SetPointer(0);
                    TextListener.StartListening();
                }
                UpdateBoard = true;
                Pointer = TextListener.GetPointer();
            }

            if (!ChatMessages.SequenceEqual(ChatListener.GetChatContent())) {
                ChatMessages = ChatListener.GetChatContent();
                DecryptedMessages = CurrentUser.Read_Messages_Chat(ChatMessages);
                UpdateBoard = true;

            }

            if (UpdateBoard) {
                User.User.Display.Content(DecryptedMessages,BoardDimensions,TypedText, true, OtherUser, Pointer);
                UpdateBoard = false;
            }
        }
        TextListener.StopListening();
    }

    public static void ChangeSettings(User.User CurrentUser) {
        Tuple<int,int> BoardDimensions = CurrentUser.Read_User_BoardSettings();
        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();
        string connectionString = "Data Source=UserList.db;Version=3;";

        string[] Content = new string[3]{"Exit Settings", "Width: "+BoardDimensions.Item1, "Height: "+BoardDimensions.Item2};
        
        TextListener.SetPointer(0);
        TextListener.SetTypedText(" ");
        TextListener.SetEnterPressed(false);

        int Pointer = 0;
        string TypedText = " ";
        int TempInt = 0;
        string TempString = "";
        var Value = 0;

        User.User.Display.With.Pointer(Content,BoardDimensions,Pointer,TypedText);

        TextListener.StartListening();

        while (!TextListener.GetESCPressed()) {
            Thread.Sleep(50);
            // Console.WriteLine(Pointer);
            if (TypedText != TextListener.GetTypedText()) {

                TypedText = TextListener.GetTypedText();

                User.User.Display.With.Pointer(Content,BoardDimensions,Pointer,TypedText);
            }

            if (Pointer != TextListener.GetPointer()) {

                Pointer = TextListener.GetPointer();

                if (Pointer >= Content.Length) {
                    TextListener.SetPointer(0);
                } else if (Pointer < 0) {
                    TextListener.SetPointer(Content.Length-1);
                }

                User.User.Display.With.Pointer(Content,BoardDimensions,Pointer,TypedText);
                
            }

            if (TextListener.GetEnterPressed()) {
                if (Pointer == 0) {
                    TextListener.StopListening();
                    return;
                } else if (Pointer == 1) {
                    if (int.TryParse(TypedText, out TempInt)) {
                        Value = int.Parse(TypedText);
                    } else {
                        Value = 25;
                    }
                    TempString = "Width";
                } else if (Pointer == 2) {
                    if (int.TryParse(TypedText, out TempInt)) {
                        Value = int.Parse(TypedText);
                    } else {
                        Value = 25;
                    }
                    TempString = "Height";
                }

                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string sql = "UPDATE Main SET "+TempString+" = @newValue WHERE Username = @username";
                    using (SQLiteCommand command = new SQLiteCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@newValue", Value);
                        command.Parameters.AddWithValue("@username", CurrentUser.Username);
                        command.ExecuteNonQuery();
                    }
                }

                TextListener.StopListening();
                TextListener.SetEnterPressed(false);
                TextListener.SetTypedText(" ");
                TextListener.StartListening();
                BoardDimensions = CurrentUser.Read_User_BoardSettings();
                Content = new string[3]{"Exit", "Width: "+BoardDimensions.Item1, "Height: "+BoardDimensions.Item2};
            }
        }
        TextListener.StopListening();
    }
}