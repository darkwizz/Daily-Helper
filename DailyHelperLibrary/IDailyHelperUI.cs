using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Entry;
using DailyHelperLibrary.Notes;
using DailyHelperLibrary.Scheduler;
using DailyHelperLibrary.Timer;
using DailyHelperLibrary.TODO;

namespace DailyHelperLibrary
{
    public interface IDailyHelperUI
    {
        // Scheduler Module
        event Func<SchedulerModuleEventArgs<OnceRunningScheduleItem>, EventResult> PlaceOnceRunningSelect;
        event Func<SchedulerModuleEventArgs<RegularlyRunningScheduleItem>, EventResult> PlaceRegularlyRunningSelect;
        event Func<SchedulerModuleEventArgs<OnceRunningScheduleItem>, EventResult> DeleteScheduleItem;
        // Timer module
        event Func<TimerEventArgs, EventResult> StartTimerSelect;
        // Notes module
        event Func<NoteModuleEventArgs, EventResult> AddNewNoteSelect;
        event Func<NoteModuleEventArgs, EventResult> EditNoteSelect;
        event Func<NoteModuleEventArgs, EventResult> DeleteNoteSelect;
        // Registration module
        event Func<RegistrationEventArgs, EventResult> RegisterNewUserSelect;
        event Func<AcceptingCheckingKeyEventArgs, EventResult> AcceptCheckingKeySelect;
        // Authorization module
        event Func<AuthorisationEventArgs, EventResult> UserEnter;
        event Func<AuthorisationEventArgs, EventResult> ForgotPassword;
        event Func<ExitEventArgs, EventResult> Logout;
        // TODO Module
        event Func<TodoModuleEventArgs, EventResult> AddNewTodoSelect;
        event Func<TodoModuleEventArgs, EventResult> CompleteTodoSelect;

        void RunMainDialogProc();
    }
}
