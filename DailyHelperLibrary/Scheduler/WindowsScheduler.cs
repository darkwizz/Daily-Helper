using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.ServiceEntities;
using Microsoft.Win32.TaskScheduler;

namespace DailyHelperLibrary.Scheduler
{
    public class WindowsScheduler: IScheduler
    {
        private const int DAYS_COUNT = 7;
        private static DaysOfTheWeek[] _days;
        private TaskService _taskService;

        public WindowsScheduler()
        {
            _taskService = new TaskService();
            if (_days == null)
            {
                //int i = 0;
                _days = new DaysOfTheWeek[DAYS_COUNT];
                _days[0] = DaysOfTheWeek.Monday;
                _days[1] = DaysOfTheWeek.Tuesday;
                _days[2] = DaysOfTheWeek.Wednesday;
                _days[3] = DaysOfTheWeek.Thursday;
                _days[4] = DaysOfTheWeek.Friday;
                _days[5] = DaysOfTheWeek.Saturday;
                _days[6] = DaysOfTheWeek.Sunday;
                //foreach (var day in Enum.GetValues(typeof(DaysOfTheWeek)))
                //{
                //    _days[i++] = (DaysOfTheWeek)day;
                //}
            }
        }

        public void PlaceOnScheduling(OnceRunningScheduleItem item)
        {
            TaskDefinition task = _taskService.NewTask();
            if (item.Message != null && item.Message != String.Empty)
            {
                task.Actions.Add(new ShowMessageAction(item.Message, "SURPRISE"));
            }
            task.Actions.Add(new ExecAction(item.ExecutablePath));

            RegularlyRunningScheduleItem regItem = item as RegularlyRunningScheduleItem;
            // If we place on scheduling once running item
            if (regItem == null)
            {
                task.Triggers.Add(new TimeTrigger(item.TriggeringTime));
            }
            else
            {
                // by default; Client must provide, that user can't place task on scheduling without running days
                DaysOfTheWeek dayOfWeek = DaysOfTheWeek.AllDays;
                for (int i = 0; i < DAYS_COUNT; i++)
                {
                    if (regItem.RunningDays[i])
                    {
                        // If there is the first marked day then dayOfWeek = _days[i]
                        // else add one more day
                        dayOfWeek = dayOfWeek == DaysOfTheWeek.AllDays ?
                            _days[i] :
                            dayOfWeek | _days[i];
                    }
                }
                task.Triggers.Add(new WeeklyTrigger 
                    { StartBoundary = regItem.TriggeringTime, DaysOfWeek = dayOfWeek });
            }
            _taskService.RootFolder.RegisterTaskDefinition(item.Id.ToString(), task);
        }

        public bool RemoveFromScheduling(OnceRunningScheduleItem item)
        {
            // if no such task, then returns null
            Task task = _taskService.GetTask(item.Id.ToString());
            if (task == null)
            {
                Console.WriteLine("No such task"); // log
                return false;
            }
            _taskService.RootFolder.DeleteTask(item.Id.ToString());
            return true;
        }

        /// <summary>
        /// Releases all resources taken from Windows System Scheduler interface
        /// </summary>
        public void Dispose()
        {
            _taskService.Dispose();
        }
    }
}