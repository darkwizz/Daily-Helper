using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Savers;
using DailyHelperLibrary.ServiceContracts;

namespace DailyHelperLibrary.Proxies
{
    public class NoteSaver : INoteSaver, IDisposable
    {
        private NoteSaverProxy _proxy = new NoteSaverProxy();

        public void SaveNote(User user, Note note)
        {
            _proxy.SaveNote(user, note);
        }

        public void EditNote(Note note)
        {
            _proxy.EditNote(note);
        }

        public void RemoveNote(Note note)
        {
            _proxy.RemoveNote(note);
        }

        public Dictionary<Guid, Note> GetNotes(User user)
        {
            return _proxy.GetNotes(user);
        }

        public void Dispose()
        {
            _proxy.Close();
        }

        
        class NoteSaverProxy : ClientBase<INoteSaverService>
        {
            public NoteSaverProxy() :
                base("SaveNoteEndpoint")
            { }

            public void SaveNote(User user, Note note)
            {
                Channel.SaveNote(user.ServiceUser, note.ServiceNote);
            }

            public void EditNote(Note note)
            {
                Channel.EditNote(note.ServiceNote);
            }

            public void RemoveNote(Note note)
            {
                Channel.RemoveNote(note.ServiceNote);
            }

            public Dictionary<Guid, Note> GetNotes(User user)
            {
                return Channel.GetNotes(user.ServiceUser).ToDictionary(x => x.Key, x => x.Value.Note);
            }

            new public void Close()
            {
                try
                {
                    base.Close();
                }
                catch (CommunicationException ex)
                {
                    Console.WriteLine("Some problems with connection. " + ex.Message); // logging
                }
            }
        }
    }
}
