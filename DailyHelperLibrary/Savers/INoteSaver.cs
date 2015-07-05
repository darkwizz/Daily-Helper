using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;

namespace DailyHelperLibrary.Savers
{
    public interface INoteSaver
    {
        void SaveNote(User user, Note note);
        void RemoveNote(Note note);
        void EditNote(Note note);
        Dictionary<Guid, Note> GetNotes(User user);
    }
}
