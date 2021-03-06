﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using DailyHelperLibrary.Entities;
using Microsoft.Win32.TaskScheduler;

namespace DailyHelperLibrary.Scheduler
{
    public class WindowsScheduler: IScheduler
    {
        private const int DAYS_COUNT = 7;
        private const string CONFIG_DIRECTORY = "ScheduleConfigs";
        private static DataContractSerializer _serializer;
        private static DaysOfTheWeek[] _days;
        private TaskService _taskService;

        public WindowsScheduler()
        {
            _taskService = new TaskService();
            if (_serializer == null)
            {
                _serializer = new DataContractSerializer(typeof(Dictionary<Guid, InnerOnceRunningScheduleItem>), 
                    new Type[] { typeof(InnerRegularlyRunningScheduleItem) });
            }
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

        public void SaveUserConfig(User user)
        {
            string userDirectory = user.Email.Substring(0, user.Email.LastIndexOf('@')) + "Settings";
            if (!Directory.Exists(CONFIG_DIRECTORY + "/" + userDirectory))
            {
                Directory.CreateDirectory(CONFIG_DIRECTORY + "/" + userDirectory);
            }
            string configFile = CONFIG_DIRECTORY + "/" + userDirectory + "/Scheduler.xml";
            Dictionary<Guid, InnerOnceRunningScheduleItem> scheduleItems = new Dictionary<Guid, InnerOnceRunningScheduleItem>();
            foreach (var item in user.ScheduleItems.Values)
            {
                scheduleItems.Add(item.Id, item.InnerScheduleItem);
            }
            using (XmlWriter writer = XmlWriter.Create(configFile))
            {
                _serializer.WriteObject(writer, scheduleItems);
            }
        }

        static WindowsScheduler()
        {
            if (!Directory.Exists(CONFIG_DIRECTORY))
            {
                Directory.CreateDirectory(CONFIG_DIRECTORY);
            }
        }

        public Dictionary<Guid, OnceRunningScheduleItem> LoadUserConfig(string login)
        {
            string userDirectory = login.Substring(0, login.LastIndexOf('@')) + "Settings";
            Dictionary<Guid, OnceRunningScheduleItem> scheduleItems = new Dictionary<Guid,OnceRunningScheduleItem>();
            if (!Directory.Exists(CONFIG_DIRECTORY + "/" + userDirectory))
            {
                return scheduleItems;
            }
            string configFile = CONFIG_DIRECTORY + "/" + userDirectory + "/Scheduler.xml";
            if (!File.Exists(configFile))
            {
                return scheduleItems;
            }
            using (XmlReader reader = XmlReader.Create(configFile))
            {
                Dictionary<Guid, InnerOnceRunningScheduleItem> items = 
                    (Dictionary<Guid, InnerOnceRunningScheduleItem>)_serializer.ReadObject(reader);
                foreach (var item in items.Values)
                {
                    if (item.TriggeringTime <= DateTime.Now)
                    {
                        continue;
                    }
                    scheduleItems.Add(item.Id, item.ScheduleItem);
                }
            }
            return scheduleItems;
        }

        public void PlaceOnScheduling(OnceRunningScheduleItem item)
        {
            TaskDefinition task = _taskService.NewTask();
            if (item.Message != null)
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
                int i;
                for (i = 0; i < DAYS_COUNT; i++)
                {
                    if (regItem.RunningDays[i])
                    {
                        dayOfWeek = _days[i];
                        break;
                    }
                }
                for (; i < DAYS_COUNT; i++)
                {
                    if (regItem.RunningDays[i])
                    {
                        dayOfWeek = _days[i];
                    }
                }
                task.Triggers.Add(new MonthlyDOWTrigger { WeeksOfMonth = WhichWeek.AllWeeks, StartBoundary = regItem.TriggeringTime, DaysOfWeek = dayOfWeek });
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