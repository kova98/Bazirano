using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bazirano.Models.DataAccess;
using Bazirano.Models.News;
using Microsoft.EntityFrameworkCore;

namespace Bazirano.Infrastructure
{
    public static class TimeHelper
    {
        public static TimeDisplay GetTimeDisplayFromTimeElapsed(TimeSpan elapsed)
        {
            int timeNumber;
            string timeText;

            if (elapsed.Days > 0)
            {
                timeNumber = elapsed.Days;
                timeText = GetDayNoun(elapsed.Days);
            }
            else if (elapsed.Hours > 0)
            {
                timeNumber = elapsed.Hours;
                timeText = GetHourNoun(elapsed.Hours);
            }
            else
            {
                timeNumber = elapsed.Minutes;
                timeText = GetMinuteNoun(elapsed.Minutes);
            }

            return new TimeDisplay { Number = timeNumber, Text = timeText };
        }

        private static string GetHourNoun(int hours)
        {
            char lastDigit = hours.ToString().Last();

            if (lastDigit == '1' && hours != 11)
            {
                return "sat";
            }
            if ((lastDigit == '2' || lastDigit == '3' || lastDigit == '4') && (hours > 14 || hours < 10))
            {
                return "sata";
            }

            return "sati";
        }

        private static string GetDayNoun(int days)
        {
            char lastDigit = days.ToString().Last();
            return (lastDigit == '1' && days != 11) ? "dan" : "dana";
        }

        private static string GetMinuteNoun(int minutes)
        {
            return "min";
        }

    }
}
