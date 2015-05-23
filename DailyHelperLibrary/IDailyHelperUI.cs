using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entry;
using DailyHelperLibrary.Notes;
using DailyHelperLibrary.Timer;
using DailyHelperLibrary.TODO;

namespace DailyHelperLibrary
{
    public interface IDailyHelperUI
    {
        event Func<TimerEventArgs, EventResult> StartTimerSelect;
        event Func<NoteModuleEventArgs, EventResult> AddNewNoteSelect;
        event Func<NoteModuleEventArgs, EventResult> EditNoteSelect;
        event Func<NoteModuleEventArgs, EventResult> DeleteNoteSelect;
        event Func<RegistrationEventArgs, EventResult> RegisterNewUserSelect;
        event Func<AcceptingCheckingKeyEventArgs, EventResult> AcceptCheckingKeySelect;
        event Func<AuthorisationEventArgs, EventResult> UserEnter;
        event Func<AuthorisationEventArgs, EventResult> ForgotPassword;
        event Func<TodoModuleEventArgs, EventResult> AddNewTodoSelect;
        event Func<TodoModuleEventArgs, EventResult> CompleteTodoSelect;

        void RunMainDialogProc();
    }
}
