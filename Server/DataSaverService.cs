using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using Server.Entities;
using Server.ServiceContracts;
using Server.DataLayer;

namespace Server
{
    [ServiceBehavior(Namespace = "Server/")]
    class DataSaverService: IUserSaverService, INoteSaverService, ITodoSaverService
    {
        //private static DataSaverService _saverService;
        private IDAL _dataLayer = new DALStub();

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

        void INoteSaverService.RemoveNote(User user, Note note)
        {
            Console.WriteLine("Remove existing note...");
            _dataLayer.RemoveNote(user, note);
        }

        void INoteSaverService.EditNote(User user, Note note)
        {
            Console.WriteLine("Edit existing note...");
            _dataLayer.UpdateNote(user, note);
        }

        void ITodoSaverService.SaveTodoItem(User user, TodoItem item)
        {
            Console.WriteLine("Save new TODO item...");
            _dataLayer.SaveTodoItem(user, item);
        }

        void ITodoSaverService.RemoveTodoItem(User user, TodoItem item)
        {
            Console.WriteLine("Remove existing todo item...");
            _dataLayer.RemoveTodoItem(user, item);
        }
    }
}
