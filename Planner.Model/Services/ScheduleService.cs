﻿using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planner.Model.Services
{
    public class ScheduleService : IScheduleService
    {
        public async Task AddEventAsync(Event @event)
        {
            //TODO: Add event through API
        }

        public async Task RemoveEventAsync(Event @event)
        {
            //TODO: Remove event through API
        }

        public async Task CompleteEventAsync(Event @event, int displayedDay)
        {
            //TODO: Update event through API
            //Code from viewmodel:
            /*
             * var events = unitOfWork.EventRepository.Find(x => x.Id == @event.Id).ToList();

                if (events.Any())
                {
                    var actualEvent = events[0];

                    if (actualEvent.RecurrencePattern == null)
                    {
                        actualEvent.CompleteEvent(User);
                        await RemoveEventAsync(actualEvent);
                    }
                    else
                    {
                        var day = Schedule.ElementAt(displayedDay).Key;
                        actualEvent.CompleteEvent(User, day);
                        unitOfWork.SaveChanges();
                        await LoadScheduleAsync();
                    }

                }*/
        }

        public Event BuildEvent(string name, int eventType, int eventDifficulty, DateTime startDateTime, DateTime? endDateTime, bool allDay,
                int recurrenceType, int interval, List<Microsoft.Graph.DayOfWeek> daysOfWeek, int index, int month, int? occurrences)
        {
            RecurrencePattern recurrencePattern = null;

            switch (recurrenceType)
            {
                case -1:
                    break;
                case 0:
                    {
                        recurrencePattern = new RecurrencePattern();
                        recurrencePattern.Interval = interval;
                        recurrencePattern.Type = RecurrencePatternType.Daily;
                    }
                    break;
                case 1:
                    {
                        recurrencePattern = new RecurrencePattern();
                        recurrencePattern.Type = RecurrencePatternType.Weekly;
                        recurrencePattern.Interval = interval;
                        recurrencePattern.DaysOfWeek = daysOfWeek;
                    }
                    break;
                case 2:
                    {
                        recurrencePattern = new RecurrencePattern();
                        recurrencePattern.Interval = interval;

                        if (index == -1)
                        {
                            recurrencePattern.Type = RecurrencePatternType.AbsoluteMonthly;
                        }
                        else
                        {
                            recurrencePattern.Type = RecurrencePatternType.RelativeMonthly;
                            recurrencePattern.Index = (WeekIndex)index;
                            recurrencePattern.DaysOfWeek = daysOfWeek;
                        }
                    }
                    break;
                case 3:
                    {
                        recurrencePattern = new RecurrencePattern();
                        recurrencePattern.Interval = interval;

                        if (index == -1)
                        {
                            recurrencePattern.Type = RecurrencePatternType.AbsoluteYearly;
                        }
                        else
                        {
                            recurrencePattern.Type = RecurrencePatternType.RelativeYearly;
                            recurrencePattern.Index = (WeekIndex)index;
                            recurrencePattern.DaysOfWeek = daysOfWeek;
                            recurrencePattern.Month = month + 1;
                        }
                    }
                    break;
            }

            if (endDateTime == null && recurrencePattern == null)
            {
                return new Event(name, (EventType)eventType, (EventDifficulty)eventDifficulty, startDateTime, allDay);
            }
            else if (endDateTime.HasValue && recurrencePattern == null)
            {
                return new Event(name, (EventType)eventType, (EventDifficulty)eventDifficulty, startDateTime, endDateTime.Value);
            }
            else if (endDateTime == null && recurrencePattern != null && occurrences == null)
            {
                return new Event(name, (EventType)eventType, (EventDifficulty)eventDifficulty, startDateTime, allDay, recurrencePattern);
            }
            else if (endDateTime.HasValue && recurrencePattern != null && occurrences == null)
            {
                return new Event(name, (EventType)eventType, (EventDifficulty)eventDifficulty, startDateTime, endDateTime.Value, recurrencePattern);
            }
            else if (endDateTime == null && recurrencePattern != null && occurrences.HasValue)
            {
                return new Event(name, (EventType)eventType, (EventDifficulty)eventDifficulty, startDateTime, allDay, recurrencePattern, occurrences.Value);
            }
            else if (endDateTime.HasValue && recurrencePattern != null && occurrences.HasValue)
            {
                return new Event(name, (EventType)eventType, (EventDifficulty)eventDifficulty, startDateTime, endDateTime.Value, recurrencePattern, occurrences.Value);
            }
            else return null;
        }

        public async Task<Dictionary<DateTime, List<Event>>> GetScheduleAsync(DateTime CurrentlyDisplayedDate)
        {
            var Schedule = new Dictionary<DateTime, List<Event>>();
            var days = await Task.Run(() => GetCurrentlyDisplayedDays(CurrentlyDisplayedDate));
            var events = new List<Event>();

            //Load Events from API


            var repetetiveEvents = await Task.Run(() => events.Where(x => x.RecurrencePattern != null).ToList());

            foreach (var t in days)
            {
                var listOfEvents = new List<Event>();

                //Finds all repetetive events that are going to happen on the day t
                foreach (var k in repetetiveEvents)
                {
                    if (k.IsDateTimeMatchingRecurrencePattern(t) && k.DaysCompleted.Find(x => x.Date.Equals(t.Date)) == default(DateTime)) listOfEvents.Add(k);
                }

                // Finds all disposable events that are going to happen on the day t
                listOfEvents.AddRange(await Task.Run(() => events.Where(x => x.StartDateTime.Value.Date == t.Date && x.RecurrencePattern == null).ToList()));

                Schedule.Add(t, listOfEvents);
            }

            return Schedule;
        }

        private List<DateTime> GetCurrentlyDisplayedDays(DateTime CurrentlyDisplayedDate)
        {
            var listOfDays = new List<DateTime>();
            var date = new DateTime(CurrentlyDisplayedDate.Year, CurrentlyDisplayedDate.Month, 1);

            // Generate days from previous months to fill a gap at the begining of the calendar
            int numberOfDaysFromPreviousMonth;

            // Sunday is 0
            if ((int)date.DayOfWeek == 0) numberOfDaysFromPreviousMonth = 6;
            else numberOfDaysFromPreviousMonth = (int)date.DayOfWeek - 1;

            date = date.AddDays(-numberOfDaysFromPreviousMonth);

            for (int i = 0; i < numberOfDaysFromPreviousMonth; i++)
            {
                listOfDays.Add(date);
                date = date.AddDays(1);
            }

            // Generate list of days from currently displayed month
            listOfDays.AddRange(Enumerable.Range(1, DateTime.DaysInMonth(date.Year, date.Month))
                .Select(day => new DateTime(date.Year, date.Month, day))
                .ToList());

            // Generate days from next month to fill a gap at the end of the calendar
            date = date.AddMonths(1);

            var numberOfDaysFromNextMonth = 35 - listOfDays.Count;

            for (int i = 0; i < numberOfDaysFromNextMonth; i++)
            {
                listOfDays.Add(date);
                date = date.AddDays(1);
            }

            return listOfDays;
        }
    }
}
