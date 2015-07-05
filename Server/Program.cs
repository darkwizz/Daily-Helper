﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Server.DataLayer;
using Server.Entities;
using Server.ServiceContracts;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            //TestDataLayer();
            //TestDataUpdatingDeleting();
            //TestUserLoading();
            //Console.ReadKey();

            //Console.WriteLine(typeof(IDataSaverService).FullName);
            //DataSaverService saverService = DataSaverService.GetSaverService(new DALStub());
            //ServiceHost host = new ServiceHost(saverService);

            ServiceHost host = null;
            try
            {
                host = new ServiceHost(typeof(DataSaverService));
                Console.WriteLine("Listening address: " + host.BaseAddresses[0]);
                host.Open();
                Console.WriteLine("Server has started listening...");
                Console.ReadKey();
            }
            catch (CommunicationException ex)
            {
                Console.WriteLine("Error with " + ex);
            }
            finally
            {
                if (host != null)
                {
                    host.Close();
                }
            }
        }

        // TESTS

        private static void TestDataLayer()
        {
            IDAL dal = new MsSqlDAL();
            User user = new User();
            user.Id = Guid.NewGuid();
            user.Email = "fake@mail.com";
            user.Password = "fake_pwd";
            dal.SaveUser(user);

            Note note = new Note();
            note.Id = Guid.NewGuid();
            note.NoteText = "AZAZAZA";

            dal.SaveNote(user, note);
            note.Id = Guid.NewGuid();
            note.NoteText = "OLOLO";

            dal.SaveNote(user, note);
            note.Id = Guid.NewGuid();
            note.NoteText = "HEHEHE";

            dal.SaveNote(user, note);
            note.Id = Guid.NewGuid();
            note.NoteText = "RORORORO";

            dal.SaveNote(user, note);

            //IDAL dal = new MsSqlDAL();
            //User user = dal.GetUser("fake@mail.com");
            //Console.WriteLine(user.Email + " " + user.Password);
        }

        private static void TestUserLoading()
        {
            IDAL dal = new MsSqlDAL();
            DataSaverService saver = new DataSaverService();
            IUserSaverService userSaver = saver;
            IScheduleItemSaverService scheduleSaver = saver;
            User user = userSaver.GetUser("fake@mail.com");
            user.ScheduleItems = scheduleSaver.GetScheduleItems(user, "azaza");
            string output = string.Format("User {0} - {1}", user.Email, user.Password);
            Console.WriteLine(output);
            foreach (OnceRunningScheduleItem item in user.ScheduleItems.Values)
            {
                Console.WriteLine(item.ExecutablePath);
            }
        }

        private static void TestDataUpdatingDeleting()
        {
            IDAL dal = new MsSqlDAL();
            User user = dal.GetUser("fake@mail.com");
            user.Notes = dal.GetNotes(user);
            Note removedNote = user.Notes.Values.ToArray()[0];
            dal.RemoveNote(removedNote);
            user.Notes.Remove(removedNote.Id);
            Note editedNote = user.Notes.Values.ToArray()[0];
            editedNote.NoteText = "TROLOLO";
            dal.UpdateNote(editedNote);
            foreach (Note note in user.Notes.Values)
            {
                Console.WriteLine(note.NoteText);
            }
        }
    }
}
