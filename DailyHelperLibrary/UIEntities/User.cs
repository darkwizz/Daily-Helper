using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyHelperLibrary.UIEntities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }
        public Dictionary<Guid, Note> Notes { get; private set; }

        public User(string email, string password)
        {
            email = Email;
            password = Password;
            Id = Guid.NewGuid();
            Notes = new Dictionary<Guid, Note>();
        }
    }
}
