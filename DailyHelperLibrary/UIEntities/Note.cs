using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.UIEntities
{
    public class Note
    {
        public Guid Id { get; private set; }
        public string NoteText { get; private set; }
        
        public Note(string note)
        {
            Id = Guid.NewGuid();
            NoteText = note;
        }
    }
}
