using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public static class DateTimeExtensions
    {
        public static string ToEuTimeFormat(this DateTime date)
        {
            return date.ToString("dd.MM.yyyy. HH:mm");
        }
    }
}
