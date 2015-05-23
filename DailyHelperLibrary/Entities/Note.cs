using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.ServiceEntities;

namespace DailyHelperLibrary.Entities
{
    public class Note
    {
        internal ServiceNote ServiceNote
        {
            get
            {
                return new ServiceNote { Id = Id, NoteText = NoteText };
            }
        }

        public Guid Id { get; private set; }
        public string NoteText { get; set; }

        internal Note(ServiceNote note)
        {
            Id = note.Id;
            NoteText = note.NoteText;
        }

        public Note(string noteText)
        {
            Id = Guid.NewGuid();
            NoteText = noteText;
        }
    }
}
