using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Proxies;
using DailyHelperLibrary.Entities;
using System.ServiceModel;
using DailyHelperLibrary.Savers;

namespace DailyHelperLibrary.Notes
{
    public class NotesModule
    {
        private INoteSaver _saverService;

        public NotesModule(INoteSaver proxy)
        {
            _saverService = proxy;
        }

        public EventResult OnAddNote(NoteModuleEventArgs e)
        {
            User user = e.User;
            Note note = e.Note;
            user.Notes.Add(note.Id, note);
            try
            {
                _saverService.SaveNote(user, note);
                return new EventResult(true);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
        }

        public EventResult OnDeleteNote(NoteModuleEventArgs e)
        {
            User user = e.User;
            Note note = e.Note;
            try
            {
                _saverService.RemoveNote(user, note);
                user.Notes.Remove(note.Id);
                return new EventResult(true);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
        }

        public EventResult OnEditNote(NoteModuleEventArgs e)
        {
            User user = e.User;
            Note note = e.Note;
            try
            {
                _saverService.EditNote(user, note);
                user.Notes[note.Id] = note;
                return new EventResult(true);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
        }
    }
}