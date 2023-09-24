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

        //User.User User = User.User;

        
        DMSExtras.DMSExtras.TextListener TextListener = new DMSExtras.DMSExtras.TextListener();

        string TypedText = "";
        int Pointer = int.MaxValue;



        User.User.Generate_Database();

        new User.User("BugReport", "BugReport", "BugReport").Generate();
        User.User TempUser = new User.User("BugReport", "BugReport", "BugReport");
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
            User.User.Display.With.Pointer(Content, 0);
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
                User.User.Display.With.Pointer(Content, Pointer);
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
            User.User.Display.Content(Content, TypedText);
            while (!TextListener.GetEndProgram()) {
                if (TypedText != TextListener.GetTypedText()) {
                    TypedText = TextListener.GetTypedText();
                    // Pointer = TextListener.GetPointer();
                    User.User.Display.Content(Content, TypedText);
                    //Console.WriteLine(TypedText);
                }
            }
            TextListener.StopListening();

            if (BadWords.BadWords.list.Any(TypedText.Contains)) {
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
                    User.User.Display.Content(Content, TypedText);
                }
            }
            TextListener.StopListening();
            TempPassword = TypedText;

            // System.Threading.Thread.Sleep(100);

            new User.User(TempUsername, TempPassword).Generate();

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
                User.User.Display.Content(Content, TypedText);
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
                User.User.Display.Content(Content, TypedText);
            }
        }
        TextListener.StopListening();

        // threadKeys.Join();

        string Password = TypedText;

        User.User CurrentUser = new User.User(Username, Password);


        if (!(User.User.Exists(Username) && User.User.Exists_PublicKey(CurrentUser.Key.Public))) {
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
                    User.User.Display.With.Pointer(DisplayedChats, Pointer, TypedText);
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
                    User.User.Display.With.Pointer(DisplayedChats, Pointer, TypedText);
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

                User.User.Display.Content(CurrentUser.Read_Messages_Chat(SelectedChat),TypedText,true);
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



    
}
