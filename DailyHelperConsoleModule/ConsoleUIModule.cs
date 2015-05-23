using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DailyHelperLibrary;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Entry;
using DailyHelperLibrary.Notes;
using DailyHelperLibrary.Timer;
using DailyHelperLibrary.TODO;

namespace DailyHelperConsoleModule
{
    public class ConsoleUIModule: IDailyHelperUI
    {
        public event Func<TimerEventArgs, EventResult> StartTimerSelect;
        public event Func<NoteModuleEventArgs, EventResult> AddNewNoteSelect;
        public event Func<NoteModuleEventArgs, EventResult> EditNoteSelect;
        public event Func<NoteModuleEventArgs, EventResult> DeleteNoteSelect;
        public event Func<RegistrationEventArgs, EventResult> RegisterNewUserSelect;
        public event Func<AcceptingCheckingKeyEventArgs, EventResult> AcceptCheckingKeySelect;
        public event Func<AuthorisationEventArgs, EventResult> UserEnter;
        public event Func<AuthorisationEventArgs, EventResult> ForgotPassword;
        public event Func<TodoModuleEventArgs, EventResult> AddNewTodoSelect;
        public event Func<TodoModuleEventArgs, EventResult> CompleteTodoSelect;

        private User _user;
        private List<Note> _notes;
        private List<TimeSpan> _timers;
        private List<TodoItem> _items;
        private List<string> _timerCaptions;

        public ConsoleUIModule()
        { }

        public ConsoleUIModule(User user)
        {
            this._user = user;
            if (user.Notes.Count == 0)
            {
                _notes = new List<Note>();
            }
            else
            {
                _notes = user.Notes.Values.ToList();
            }
        }

        public void RunMainDialogProc()
        {
            // Entry part (log in or register) + Main menu
            bool isEntered = false;
            while (true)
            {
                if (!isEntered)
                {
                    if (!EnterInSystem())
                    {
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        break;
                    }
                    isEntered = true;
                }

                // End of entry part

                string choice;
                Console.Clear();

                // here there are some commands
                ShowMainMenu();
                Console.Write("\nWhat is your choise: ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": // if notes point
                        GoInNoteModule();
                        break;


                    case "2":
                        GoInTimerModule();
                        break;

                        
                    case "3":
                        GoInTODOModule();
                        break;

                    case "0": // log out
                        isEntered = false;
                        if (_timers != null)
                        {
                            _timers.Clear();
                        }
                        if (_timerCaptions != null)
                        {
                            _timerCaptions.Clear();
                        }
                        break;


                    case "-1": // exit
                        if (_timers != null)
                        {
                            _timers.Clear();
                        }
                        if (_timerCaptions != null)
                        {
                            _timerCaptions.Clear();
                        }
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey();
                        return;


                    default:
                        Console.WriteLine("No such function");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private bool EnterInSystem()
        {
            while (true)
            {
                Console.Clear();
                string menu = "1 - to log in;\n" +
                              "2 - to register;\n" +
                              "0 - to quit;\n" +
                              "-1 - if you forgot your password;\n";
                Console.WriteLine(menu);

                string email;
                string password;
                AuthorisationEventArgs args;
                EventResult result;
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": // Log in
                        Console.Write("Enter your email: ");
                        email = Console.ReadLine();
                        // some validation checkings

                        Console.Write("Enter your password: ");
                        password = Console.ReadLine();
                        // some validation checkings

                        args = new AuthorisationEventArgs(email, password);
                        result = UserEnter(args);
                        if (result.IsSuccessful)
                        {
                            Console.WriteLine("Welcome!");
                            _user = result.OptionalInfo as User;
                            if (_user == null)
                            {
                                // some error actions, maybe exception throwing
                                return false;
                            }
                            _notes = _user.Notes.Values.ToList();
                            _items = _user.TodoItems.Values.ToList();
                            _timers = new List<TimeSpan>();
                            _timerCaptions = new List<string>();
                            Thread.Sleep(3000);
                            return true;
                        }
                        else
                        {
                            Console.WriteLine(result.ErrorDescription);
                            break;
                        }


                    case "2": // Register
                        Console.Write("Enter your email: ");
                        email = Console.ReadLine();
                        // some validation checkings

                        Console.Write("Enter your password: ");
                        password = Console.ReadLine();
                        // some validation checkings

                        Console.Write("Reenter your password: ");
                        string checkingPassword = Console.ReadLine();

                        if (!password.Equals(checkingPassword))
                        {
                            // some actions when passwords aren't equal
                        }

                        RegistrationEventArgs regArgs = new RegistrationEventArgs(email);
                        RegisterNewUserSelect(regArgs);

                        Console.WriteLine("Enter sent key: ");
                        string enteredKey = Console.ReadLine();

                        AcceptingCheckingKeyEventArgs arg = new AcceptingCheckingKeyEventArgs(email, password, enteredKey);
                        result = AcceptCheckingKeySelect(arg);
                        if (result.IsSuccessful)
                        {
                            Console.WriteLine("Registration has been done. Welcome");
                        }
                        else
                        {
                            Console.WriteLine(result.ErrorDescription);
                        }
                        break;


                    case "0": // Quit
                        return false;


                    case "-1": // Forgot
                        Console.Write("Enter your email: ");
                        email = Console.ReadLine();
                        // some validation checkings

                        args = new AuthorisationEventArgs(email);
                        result = ForgotPassword(args);
                        if (!result.IsSuccessful)
                        {
                            Console.WriteLine(result.ErrorDescription);
                        }
                        break;


                    default:
                        Console.WriteLine("No such function");
                        break;
                }
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void GoInNoteModule()
        {
            string choice;
            while (true)
            {
                Console.Clear();
                ShowNoteMenu();
                Console.Write("\nWhat is your choice: ");
                choice = Console.ReadLine();

                string noteText;
                NoteModuleEventArgs args;
                Note note;
                string number;
                int noteNumber;
                EventResult result;
                switch (choice)
                {
                    case "1": // Add new note
                        Console.WriteLine("Enter new note:");
                        noteText = Console.ReadLine();
                        note = new Note(noteText);

                        args = new NoteModuleEventArgs(_user, note);

                        _notes.Add(note);
                        result = AddNewNoteSelect(args);
                        if (!result.IsSuccessful)
                        {
                            Console.WriteLine(result.ErrorDescription);
                        }
                        break;


                    case "2": // Edit note
                        Console.Write("Enter number of edited note: ");
                        number = Console.ReadLine();
                        if (!Int32.TryParse(number, out noteNumber))
                        {
                            Console.WriteLine("Not a number");
                        }
                        else
                        {
                            if (noteNumber < 1 || noteNumber > _notes.Count)
                            {
                                Console.WriteLine("Out of range");
                            }
                            else
                            {
                                note = _notes[noteNumber - 1];
                                Console.WriteLine("Enter new note:");
                                noteText = Console.ReadLine();
                                note.NoteText = noteText;
                                
                                args = new NoteModuleEventArgs(_user, note);

                                result = EditNoteSelect(args);
                                if (!result.IsSuccessful)
                                {
                                    Console.WriteLine(result.ErrorDescription);
                                }
                            }
                        }
                        break;


                    case "3": // Delete note
                        Console.Write("Enter number of deleted note: ");
                        number = Console.ReadLine();
                        if (!Int32.TryParse(number, out noteNumber))
                        {
                            Console.WriteLine("Not a number");
                        }
                        else
                        {
                            if (noteNumber < 1 || noteNumber > _notes.Count)
                            {
                                Console.WriteLine("Out of range");
                            }
                            else
                            {
                                note = _notes[noteNumber - 1];
                                _notes.RemoveAt(noteNumber - 1);
                                
                                args = new NoteModuleEventArgs(_user, note);

                                result = DeleteNoteSelect(args);
                                if (!result.IsSuccessful)
                                {
                                    Console.WriteLine(result.ErrorDescription);
                                }
                            }
                        }
                        break;


                    case "4": // Watch notes
                        for (int i = 0; i < _notes.Count; i++)
                        {
                            Console.WriteLine((i + 1) + ". " + _notes[i].NoteText);
                        }
                        break;


                    case "0": // Quit
                        return;


                    default:
                        Console.WriteLine("No such function");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void GoInTODOModule()
        {
            string choice;
            while (true)
            {
                Console.Clear();
                ShowTODOMenu();
                Console.Write("\nWhat is your choice: ");
                choice = Console.ReadLine();

                string todoText;
                TodoModuleEventArgs args;
                TodoItem item;
                string number;
                int itemNumber;
                EventResult result;
                switch (choice)
                {
                    case "1": // Add new TODO item
                        Console.WriteLine("Enter new TODO:");
                        todoText = Console.ReadLine();
                        item = new TodoItem(todoText);

                        args = new TodoModuleEventArgs(_user, item);

                        _items.Add(item);
                        result = AddNewTodoSelect(args);
                        if (!result.IsSuccessful)
                        {
                            Console.WriteLine(result.ErrorDescription);
                        }
                        break;


                    case "2": // Complete TODO
                        Console.Write("Enter number of completed TODO: ");
                        number = Console.ReadLine();
                        if (!Int32.TryParse(number, out itemNumber))
                        {
                            Console.WriteLine("Not a number");
                        }
                        else
                        {
                            if (itemNumber < 1 || itemNumber > _items.Count)
                            {
                                Console.WriteLine("Out of range");
                            }
                            else
                            {
                                item = _items[itemNumber - 1];
                                _notes.RemoveAt(itemNumber - 1);

                                args = new TodoModuleEventArgs(_user, item);

                                result = CompleteTodoSelect(args);
                                if (!result.IsSuccessful)
                                {
                                    Console.WriteLine(result.ErrorDescription);
                                }
                            }
                        }
                        break;


                    case "3": // Watch TODOs
                        for (int i = 0; i < _items.Count; i++)
                        {
                            Console.WriteLine((i + 1) + ". " + _items[i].TodoText);
                        }
                        break;


                    case "0": // Quit
                        return;


                    default:
                        Console.WriteLine("No such function");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void GoInTimerModule()
        {
            string choice;
            while (true)
            {
                Console.Clear();
                ShowTimerMenu();
                Console.Write("\nWhat is your choice: ");
                choice = Console.ReadLine();
                switch (choice)
                {
                    case "1": // Add new timer
                        Console.Write("Enter hours: ");
                        string enteredHours = Console.ReadLine();
                        int hours;
                        if (!int.TryParse(enteredHours, out hours))
                        {
                            Console.WriteLine("Incorrect hours");
                            break;
                        }
                        Console.Write("Enter minutes: ");
                        string enteredMinutes = Console.ReadLine();
                        int minutes;
                        if (!int.TryParse(enteredMinutes, out minutes))
                        {
                            Console.WriteLine("Incorrect minutes");
                            break;
                        }
                        Console.Write("Enter seconds: ");
                        string enteredSeconds = Console.ReadLine();
                        int seconds;
                        if (!int.TryParse(enteredSeconds, out seconds))
                        {
                            Console.WriteLine("Incorrect seconds");
                            break;
                        }

                        TimeSpan time = new TimeSpan(hours, minutes, seconds);
                        _timers.Add(time);

                        Console.Write("Enter caption: "); // here there must be checking on input caption length
                        string caption = Console.ReadLine();
                        _timerCaptions.Add(caption);
                        break;


                    case "2": // Start timer
                        Console.Write("Enter number of timer to start: ");
                        string enteredNumber = Console.ReadLine();
                        int number;
                        if (!int.TryParse(enteredNumber, out number))
                        {
                            Console.WriteLine("Incorrect number");
                            break;
                        }
                        if (number < 1 || number > _timers.Count)
                        {
                            Console.WriteLine("Out of range");
                            break;
                        }
                        TimerEventArgs arg = new TimerEventArgs(_timers[number - 1]);
                        arg.Tick = TimerProcedure;
                        StartTimerSelect(arg);
                        _timers.RemoveAt(number - 1);
                        _timerCaptions.RemoveAt(number - 1);
                        break;


                    case "3": // Remove timer
                        Console.Write("Enter number of timer to remove: ");
                        string entered = Console.ReadLine();
                        int num;
                        if (!int.TryParse(entered, out num))
                        {
                            Console.WriteLine("Incorrect number");
                            break;
                        }
                        if (num < 1 || num > _timers.Count)
                        {
                            Console.WriteLine("Out of range");
                            break;
                        }
                        _timers.RemoveAt(num - 1);
                        _timerCaptions.RemoveAt(num - 1);
                        break;


                    case "4": // Watch all timers
                        for (int i = 0; i < _timers.Count; i++)
                        {
                            Console.WriteLine((i + 1) + ". " + _timerCaptions[i] + " - " + _timers[i]);
                        }
                        break;


                    case "0": // Quit
                        return;


                    default:
                        Console.WriteLine("No such function");
                        break;
                }

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }

        private void TimerProcedure(object state)
        {
            Console.Beep(18000, 3000);
            Console.WriteLine("ALAAAAAAAAAARM");
        }

        private void ShowNoteMenu()
        {
            string menu = "1 - to add new note;\n" +
                          "2 - to edit existing note;\n" +
                          "3 - to delete existing note;\n" +
                          "4 - to watch all notes;\n" +
                          "0 - to exit from note module;\n";
            Console.WriteLine(menu);
        }

        private void ShowMainMenu()
        {
            string menu = "1 - go to note module;\n" +
                          "2 - go to timer module;\n" +
                          "3 - go to TODO module;\n" +
                          "0 - to log out;\n" +
                          "-1 - to exit;\n";
            Console.WriteLine(menu);
        }

        private void ShowTimerMenu()
        {
            string menu = "1 - create new timer;\n" +
                          "2 - start timer;\n" +
                          "3 - remove existing timer;\n" +
                          "4 - watch all timers;\n" +
                          "0 - go back to main menu;\n";
            Console.WriteLine(menu);
        }

        private void ShowTODOMenu()
        {
            string menu = "1 - to add new TODO;\n" +
                          "2 - to complete existing TODO;\n" +
                          "3 - to watch all TODO records;\n" +
                          "0 - to exit from TODO module;\n";
            Console.WriteLine(menu);
        }
    }
}
