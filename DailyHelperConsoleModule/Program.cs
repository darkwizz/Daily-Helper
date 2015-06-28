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
using DailyHelperLibrary.Scheduler;
using DailyHelperLibrary.Relax;

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
            IScheduler scheduler = new WindowsScheduler();
            ScheduleItemSaver scheduleProxy = new ScheduleItemSaver();
            RegistrationModule regModule = new RegistrationModule(userProxy);
            AuthorisationModule authModule = new AuthorisationModule(userProxy, scheduleProxy, scheduler);

            SchedulerModule schedulerModule = new SchedulerModule(scheduler, scheduleProxy);

            TodoSaver todoProxy = new TodoSaver();
            TodoModule todoModule = new TodoModule(todoProxy);

            TimerModule timerModule = new TimerModule();

            RelaxModule relaxModule = new RelaxModule();

            IDailyHelperUI ui = new ConsoleUIModule(); // new User("toxa@gmail.com", "12345")
            ui.Logout += authModule.OnExited;
            ui.DeleteScheduleItem += schedulerModule.OnDeletedScheduleItem;
            ui.PlaceOnceRunningSelect += schedulerModule.OnOnceRunningSelected;
            ui.PlaceRegularlyRunningSelect += schedulerModule.OnRegularlyRunningSelected;
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
            ui.RelaxChoose += relaxModule.OnRelaxChosen;
            ui.NextChoose += relaxModule.OnNextChosen;
            ui.StopChoose += relaxModule.OnStopChosen;

            ui.RunMainDialogProc();
            // if connection is failed before these Close(), but after at least one executing of service operation,
            // then there'll be thrown an exception
            // all exception handling is on Logic side
            noteProxy.Dispose();
            userProxy.Dispose();
            todoProxy.Dispose();
            scheduler.Dispose();
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
