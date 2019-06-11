using Bazirano.Models.News;
using System;

namespace Bazirano.Infrastructure
{
    public static class DateTimeExtensions
    {
        public static bool LessThanHourElapsed(this DateTime time)
        {
            return (DateTime.Now - time).Hours == 0;
        }

        public static string Elapsed(this DateTime time)
        {
            var elapsed = DateTime.Now - time;
            return elapsed.Minutes.ToString();
        }
    }
}
