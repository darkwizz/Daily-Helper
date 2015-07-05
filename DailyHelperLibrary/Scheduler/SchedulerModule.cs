using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DailyHelperLibrary.Entities;
using DailyHelperLibrary.Faults;
using DailyHelperLibrary.Savers;

namespace DailyHelperLibrary.Scheduler
{
    public class SchedulerModule
    {
        private IScheduler _scheduler;
        private IScheduleItemSaver _saver;

        public SchedulerModule(IScheduler scheduler, IScheduleItemSaver saver)
        {
            _scheduler = scheduler;
            _saver = saver;
        }

        public EventResult OnOnceRunningSelected(SchedulerModuleEventArgs<OnceRunningScheduleItem> e)
        {
            try
            {
                _saver.SaveScheduleItem(e.User, e.ScheduleItem, OnceRunningScheduleItem.GetMachineId);
                PlaceOnScheduling(e.User, e.ScheduleItem);
                return new EventResult(true);
            }
            catch (FaultException<DatabaseConnectionFault> ex)
            {
                Console.WriteLine(ex.Detail.FullDescription); // logging
                return new EventResult(false, ex.Detail.ErrorMessage);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Unknown server error: " + ex.Message); // logging
                return new EventResult(false, ex.Message);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message); // logging
                return new EventResult(false, "Can't connect to server. Connection timeout is over");
            }
        }

        public EventResult OnRegularlyRunningSelected(SchedulerModuleEventArgs<RegularlyRunningScheduleItem> e)
        {
            try
            {
                _saver.SaveScheduleItem(e.User, e.ScheduleItem, OnceRunningScheduleItem.GetMachineId);
                PlaceOnScheduling(e.User, e.ScheduleItem);
                return new EventResult(true);
            }
            catch (FaultException<DatabaseConnectionFault> ex)
            {
                Console.WriteLine(ex.Detail.FullDescription); // logging
                return new EventResult(false, ex.Detail.ErrorMessage);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Unknown server error: " + ex.Message); // logging
                return new EventResult(false, ex.Message);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message); // logging
                return new EventResult(false, "Can't connect to server. Connection timeout is over");
            }
        }

        private void PlaceOnScheduling(User user, OnceRunningScheduleItem item)
        {
            user.ScheduleItems.Add(item.Id, item);
            _scheduler.PlaceOnScheduling(item); // this schedule item is placed on schedule immediately
        }

        /// <summary>
        /// Removes <code>ScheduleItem</code> from scheduler. If no such task, then it'll be removed from user's list and
        /// it'll show error message
        /// </summary>
        /// <param name="e">Deleted <code>ScheduleItem</code> info</param>
        /// <returns>true <code>EventResult</code>, if already added task is removed</returns>
        public EventResult OnDeletedScheduleItem(SchedulerModuleEventArgs<OnceRunningScheduleItem> e)
        {
            try
            {
                OnceRunningScheduleItem item = e.ScheduleItem;
                User user = e.User;

                bool success = _scheduler.RemoveFromScheduling(item);
                if (!success)
                {
                    return new EventResult(false, "No such task on scheduling. Please, place it on scheduler");
                }

                _saver.DeleteScheduleItem(item);
                user.ScheduleItems.Remove(item.Id);
                return new EventResult(true);
            }
            catch (FaultException<DatabaseConnectionFault> ex)
            {
                Console.WriteLine(ex.Detail.FullDescription); // logging
                return new EventResult(false, ex.Detail.ErrorMessage);
            }
            catch (FaultException ex)
            {
                Console.WriteLine("Unknown server error: " + ex.Message); // logging
                return new EventResult(false, ex.Message);
            }
            catch (CommunicationException ex)
            {
                string message = "Connection with server has been failed. " + ex.Message;
                Console.WriteLine(message); // logging
                return new EventResult(false, message);
            }
            catch (TimeoutException ex)
            {
                Console.WriteLine(ex.Message); // logging
                return new EventResult(false, "Can't connect to server. Connection timeout is over");
            }
        }
    }
}
