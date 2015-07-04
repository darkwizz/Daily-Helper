using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Server.Entities;
using Server.ServiceContracts;
using Server.DataLayer;
using System.Data.SqlClient;
using System.IO;

namespace Server
{
    [ServiceBehavior(Namespace = "Server/")]
    class DataSaverService: IUserSaverService, INoteSaverService, ITodoSaverService,
        ISocialNetworkAccountInfoSaverService, IMusicStreamGetterService,
        IScheduleItemSaverService
    {
        //private static DataSaverService _saverService;
        private IDAL _dataLayer = new MsSqlDAL();
        private static List<string> _sounds = new List<string>();

        static DataSaverService()
        {
            _sounds.Add(@"D:\Folder\Music\Leasure And In Net\Deep Purple - Highway Star.mp3");
            _sounds.Add(@"D:\Folder\Music\Leasure And In Net\Deep Purple - Burn.mp3");
            _sounds.Add(@"D:\Folder\Music\Leasure And In Net\Deep Purple - Anya.mp3");
            _sounds.Add(@"D:\Folder\Music\Leasure And In Net\Blue Oyster Cult – Dancin' In The Ruins.mp3");
            _sounds.Add(@"D:\Folder\Music\Leasure And In Net\Iron Maiden - Mother Russia.mp3");
            _sounds.AddRange(Directory.GetFiles("../../Relax Sounds"));
        }

        void IUserSaverService.RegisterUser(User user)
        {
            Console.WriteLine("Register new user - {0}", user.Email);
            _dataLayer.SaveUser(user);
        }

        User IUserSaverService.GetUser(string email)
        {
            User user = _dataLayer.GetUser(email);
            return user;
        }

        void INoteSaverService.SaveNote(User user, Note note)
        {
            Console.WriteLine("Save new note...");
            _dataLayer.SaveNote(user, note);
        }

        void INoteSaverService.RemoveNote(Note note)
        {
            Console.WriteLine("Remove existing note...");
            _dataLayer.RemoveNote(note);
        }

        void INoteSaverService.EditNote(Note note)
        {
            Console.WriteLine("Edit existing note...");
            _dataLayer.UpdateNote(note);
        }

        Dictionary<Guid, Note> INoteSaverService.GetNotes(User user)
        {
            Console.WriteLine("Loading user {0} notes...", user.Email);
            return _dataLayer.GetNotes(user);
        }

        void ITodoSaverService.SaveTodoItem(User user, TodoItem item)
        {
            Console.WriteLine("Save new TODO item...");
            _dataLayer.SaveTodoItem(user, item);
        }

        void ITodoSaverService.RemoveTodoItem(TodoItem item)
        {
            Console.WriteLine("Remove existing todo item...");
            _dataLayer.RemoveTodoItem(item);
        }

        Dictionary<Guid, TodoItem> ITodoSaverService.GetTodoItems(User user)
        {
            Console.WriteLine("Loading user {0} todo items...", user.Email); // logging
            return _dataLayer.GetTodoItems(user);
        }

        void ISocialNetworkAccountInfoSaverService.UpdateAccountInfo(User user, SocialNetworkAccountInfo info)
        {
            Console.WriteLine("Updating user account info...");
        }

        Dictionary<Guid, SocialNetworkAccountInfo> ISocialNetworkAccountInfoSaverService.GetAccounts(User user)
        {
            Console.WriteLine("Loading user {0} accoutns...", user.Email);
            return _dataLayer.GetAccounts(user);
        }

        void IScheduleItemSaverService.SaveScheduleItem(User user, OnceRunningScheduleItem item, string machineName)
        {
            Console.WriteLine("Save new schedule item...");
            // logging
            // Console.WriteLine("Path: {0}\nTime: {1}\nMessage: {2}\nMachine name: {3}\n", item.ExecutablePath, item.TriggeringTime, item.Message, machineName);
            _dataLayer.SaveScheduleItem(user, item, machineName);
        }

        void IScheduleItemSaverService.DeleteScheduleItem(OnceRunningScheduleItem item)
        {
            Console.WriteLine("Delete existing schedule item...");
            _dataLayer.RemoveScheduleItem(item);
        }

        Dictionary<Guid, OnceRunningScheduleItem> IScheduleItemSaverService.GetScheduleItems(User user, string machineName)
        {
            Console.WriteLine("Loading schedule items of user {0} on machine {1}...", user.Email, machineName);
            return _dataLayer.GetScheduleItems(user, machineName);
        }

        Stream IMusicStreamGetterService.GetMusicStream()
        {
            while (true)
            {
                try
                {
                    int song = (new Random()).Next(_sounds.Count);
                    Console.WriteLine("Open song {0}", _sounds[song]);
                    Stream stream = new FileStream(_sounds[song], FileMode.Open);
                    return stream;
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message); // logging
                    continue;
                }
            }
        }
    }
}
