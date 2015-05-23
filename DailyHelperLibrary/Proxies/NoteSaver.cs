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

        public void EditNote(User user, Note note)
        {
            _proxy.EditNote(user, note);
        }

        public void RemoveNote(User user, Note note)
        {
            _proxy.RemoveNote(user, note);
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

            public void EditNote(User user, Note note)
            {
                Channel.EditNote(user.ServiceUser, note.ServiceNote);
            }

            public void RemoveNote(User user, Note note)
            {
                Channel.RemoveNote(user.ServiceUser, note.ServiceNote);
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
