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

        //private DataSaverService(IDAL dataLayer)
        //{
        //    _dataLayer = dataLayer;
        //}

        //private DataSaverService()
        //{ }

        //public static DataSaverService GetSaverService()
        //{
        //    if (_saverService == null)
        //    {
        //        _saverService = new DataSaverService(null);
        //    }
        //    return _saverService;
        //}

        //public static DataSaverService GetSaverService(IDAL dataLayer)
        //{
        //    if (_saverService == null)
        //    {
        //        _saverService = new DataSaverService(dataLayer);
        //    }
        //    return _saverService;
        //}

        bool IUserSaverService.RegisterUser(User user)
        {
            Console.WriteLine("Register new user - {0}", user.Email);
            try
            {
                _dataLayer.SaveUser(user);
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Such user already exists"); // logging
                return false;
            }
        }

        User IUserSaverService.GetUser(string email, string machineName)
        {
            User user = _dataLayer.GetUser(email, machineName);
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

        void ISocialNetworkAccountInfoSaverService.UpdateAccountInfo(User user, SocialNetworkAccountInfo info)
        {
            Console.WriteLine("Updating user account info...");
        }

        void IScheduleItemSaverService.SaveScheduleItem(User user, OnceRunningScheduleItem item, string machineName)
        {
            try
            {
                Console.WriteLine("Save new schedule item...");
                Console.WriteLine("Path: {0}\nTime: {1}\nMessage: {2}\nMachine name: {3}\n", item.ExecutablePath, item.TriggeringTime, item.Message, machineName);
                _dataLayer.SaveScheduleItem(user, item, machineName);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Outer exception\n" + ex.Message);
            }
        }

        void IScheduleItemSaverService.DeleteScheduleItem(OnceRunningScheduleItem item)
        {
            Console.WriteLine("Delete existing schedule item...");
            _dataLayer.RemoveScheduleItem(item);
        }

        Stream IMusicStreamGetterService.GetMusicStream()
        {
            int song = (new Random()).Next(_sounds.Count);
            return new FileStream(_sounds[song], FileMode.Open);
        }
    }
}
