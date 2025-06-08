using System;

namespace FitnessTracker.RazorComponents.Utilities
{
    public static class DateTimeExtensions
    {
        // Extension method to get the start of the month
        public static DateTime StartOfMonth(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, 1);
        }
    }
}
