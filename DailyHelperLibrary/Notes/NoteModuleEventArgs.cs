using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Notes
{
    public class NoteModuleEventArgs : EventArgs
    {
        public Note Note { get; set; }
        public User User { get; set; }

        public NoteModuleEventArgs(User user, Note note)
        {
            User = user;
            Note = note;
        }
    }
}
