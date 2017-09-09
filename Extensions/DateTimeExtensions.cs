using System;

namespace RequestSimulation.Extensions
{
    public static class DateTimeExtensions 
    {
        public static DateTime Normalize(this DateTime date) 
        {
            return new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second);
        }
    }

}
