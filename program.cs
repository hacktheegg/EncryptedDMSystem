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
        Console.WriteLine("This program flickers a lot when running\n");
        Console.WriteLine(
            "I am not responsible for what is said here\n"+
            "though there is some moderation, I can only moderate if\n"+
            "something is reported (Security is the priority/no report\nSystem yet)"
        );
        Console.ReadKey(true);
        Console.Clear();


        Start:
        

        string[] Content = new string[3]{"Login", "New", "Public Room (404 not found)"};

        Tuple<int, string> Tuple = MenuLoop(Content, BoardDimensions, false);


        if (Tuple.Item1 == 1) {
            string TempUsername;
            string TempPassword;

            BadNameUsername:
            
            Content = new string[1]{"New Account Username: "};

            Tuple = MenuLoop(Content, BoardDimensions, true);

            if (BadWords.BadWords.list.Any(Tuple.Item2.Contains)) {
                Console.WriteLine("bad Word Found, Redo Step");
                System.Threading.Thread.Sleep(5000);
                goto BadNameUsername;
            }

            TempUsername = Tuple.Item2;

            Content = new string[1]{"New Account Password: "};
            
            Tuple = MenuLoop(Content, BoardDimensions, true);

            TempPassword = Tuple.Item2;

            new User.User(TempUsername, TempPassword).Generate();

            Console.WriteLine("Account Made");
            System.Threading.Thread.Sleep(5000);
            goto Start;
        } else if (Tuple.Item1 == 2) {
            Console.WriteLine("Public Chat not exist yet");
            System.Threading.Thread.Sleep(5000);
            //Not Valid Account Username
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
            System.Threading.Thread.Sleep(5000);
            goto Start;
        }

        Console.WriteLine("Correct Password");


        ChooseChat:
        bool ChooseChatLoop = true;

        string[] var = CurrentUser.Read_Chats_Allowed(@"chats\");

        int Multiplyer = 0;
        int Step = BoardDimensions.Item2-6;
        
        string[] DisplayedChats = new string[Step+2];

        while (ChooseChatLoop || Tuple.Item1 == Step || Tuple.Item1 == Step+1) {
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
        Multiplyer = 0;

        while (Tuple.Item1 == Step-1 || Tuple.Item1 == Step || Tuple.Item1 == Step+1) {
            for (int i = 0; i < Step-1; i++) {
                if ((Multiplyer*Step)+i < var.Length) {
                    DisplayedChats[i] = var[(Multiplyer*Step)+i];
                } if ((Multiplyer*Step)+i >= var.Length) { DisplayedChats[i] = "{EMPTY}"; }
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

        SelectedChat = CurrentUser.Read_Messages_FileName(SelectedChat);



        string[] DecryptedChatContent = CurrentUser.Read_Messages_Chat(SelectedChat);

        //User.Display.Content(DecryptedChatContent);

        Tuple<string, string> SymmetricKey = CurrentUser.Read_Chat_SymmetricKey(SelectedChat);

        
        // while (true) { System.Threading.Thread.Sleep(5000); }


        PrivateChatLoop(SelectedChat, BoardDimensions, CurrentUser, SymmetricKey);

        
    }



    public static Tuple<int, string> MenuLoop(string[] Content, Tuple<int,int> BoardDimensions, bool GetTypedText = true) {

        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();
        string TypedText = "";
        int Pointer = int.MaxValue;

        TextListener.SetPointer(0);
        TextListener.SetTypedText("  ");
        TextListener.SetEndProgram(false);



        TextListener.StartListening();
        while (!TextListener.GetEndProgram()) {

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

    public static void PrivateChatLoop(string SelectedChat, Tuple<int, int> BoardDimensions, User.User CurrentUser, Tuple<string, string> SymmetricKey) {
        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();
        DMSExtras.DMSExtras.ChatListener ChatListener = new DMSExtras.DMSExtras.ChatListener();

        string TypedText = " ";

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

            Thread.Sleep(100);

            if (TextListener.GetEndProgram()) {
                if (!string.IsNullOrEmpty(TypedText)) {
                    
                    if (BadWords.BadWords.list.Any(TypedText.Contains)) {
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

                User.User.Display.Content(CurrentUser.Read_Messages_Chat(SelectedChat),BoardDimensions,TypedText, false);
            }


            // Write_Messages_Chat(Tuple<string, string> SymmetricKey, string ChatName, string Content)
        }
    }
}