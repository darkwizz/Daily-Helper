using DailyHelperLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Notes;
using DailyHelperLibrary.Proxies;
using DailyHelperLibrary.Entry;
using System.Threading;
using DailyHelperLibrary.Timer;
using DailyHelperLibrary.TODO;

namespace DailyHelperConsoleModule
{
    class Program
    {
        static void Main(string[] args)
        {
            // Uncomment if you want to test EmailSender
            // TestSender();
            // return;

            // if server doesn't run when proxies are created, then program won't throw en exception;
            // and when Close() is called then no exceptions

            NoteSaver noteProxy = new NoteSaver();
            NotesModule notesModule = new NotesModule(noteProxy);

            UserSaver userProxy = new UserSaver();
            RegistrationModule regModule = new RegistrationModule(userProxy);
            AuthorisationModule authModule = new AuthorisationModule(userProxy);

            TodoSaver todoProxy = new TodoSaver();
            TodoModule todoModule = new TodoModule(todoProxy);

            TimerModule timerModule = new TimerModule();

            IDailyHelperUI ui = new ConsoleUIModule(); // new User("toxa@gmail.com", "12345")
            ui.AddNewNoteSelect += notesModule.OnAddNote;
            ui.DeleteNoteSelect += notesModule.OnDeleteNote;
            ui.EditNoteSelect += notesModule.OnEditNote;
            ui.RegisterNewUserSelect += regModule.OnRegisterUser;
            ui.UserEnter += authModule.OnEnter;
            ui.ForgotPassword += authModule.OnForgotPassword;
            ui.AcceptCheckingKeySelect += regModule.OnCheckingCodeAccept;
            ui.StartTimerSelect += timerModule.OnTimerStarted;
            ui.AddNewTodoSelect += todoModule.OnTodoAdded;
            ui.CompleteTodoSelect += todoModule.OnTodoCompleted;

            ui.RunMainDialogProc();
            // if connection is failed before these Close(), but after at least one executing of service operation,
            // then there'll be thrown an exception
            // all exception handling is on Logic side
            noteProxy.Dispose();
            userProxy.Dispose();
            todoProxy.Dispose();
            Thread.Sleep(4000);
        }

        private static void TestSender()
        {
            Console.WriteLine("Test sending message...");
            EmailSender sender = new EmailSender();
            sender.Send("tohendiy@gmail.com", "ДАААА! Письма отправляются! DH к упеху идёт!");
        }
    }
}
