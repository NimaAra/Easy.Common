namespace Easy.Common.Extensions;

using System;

/// <summary>
/// Extensions for DateTime
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Epoch represented as DateTime
    /// </summary>
    internal static readonly DateTime Epoch;

    static DateTimeExtensions() => Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Converts a given <see cref="DateTime"/> to milliseconds from Epoch.
    /// </summary>
    /// <param name="dateTime">A given <see cref="DateTime"/></param>
    /// <returns>Milliseconds since Epoch</returns>
    public static long ToEpochMilliseconds(this DateTime dateTime) => 
        (long)dateTime.ToUniversalTime().Subtract(Epoch).TotalMilliseconds;

    /// <summary>
    /// Converts a given <see cref="DateTime"/> to seconds from Epoch.
    /// </summary>
    /// <param name="dateTime">A given <see cref="DateTime"/></param>
    /// <returns>The Unix time stamp</returns>
    public static long ToEpochSeconds(this DateTime dateTime) => 
        dateTime.ToEpochMilliseconds() / 1000;

    /// <summary>
    /// Checks the given date is between the two provided dates
    /// </summary>
    public static bool IsBetween(this DateTime date, DateTime startDate, DateTime endDate, bool compareTime = false) => 
        compareTime ? date >= startDate && date <= endDate : date.Date >= startDate.Date && date.Date <= endDate.Date;

    /// <summary>
    /// Returns whether the given date is the last day of the month
    /// </summary>
    public static bool IsLastDayOfTheMonth(this DateTime dateTime) => 
        dateTime == new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(1).AddDays(-1);

    /// <summary>
    /// Returns whether the given date falls in a weekend
    /// </summary>
    public static bool IsWeekend(this DateTime value) => 
        value.DayOfWeek == DayOfWeek.Sunday || value.DayOfWeek == DayOfWeek.Saturday;

    /// <summary>
    /// Determines if a given year is a LeapYear or not.
    /// </summary>
    public static bool IsLeapYear(this DateTime value) => 
        DateTime.DaysInMonth(value.Year, 2) == 29;

    /// <summary>
    /// Returns the age based on <paramref name="birthDay"/>.
    /// </summary>
    /// <param name="birthDay">The birthday for which age should be calculated</param>
    public static int Age(this DateTime birthDay)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDay.Year;

        if (birthDay > today.AddYears(-age)) { age--; }
        return age;
    }

    /// <summary>
    /// Convert DateTime to Shamsi Date (YYYY/MM/DD)
    /// </summary>
    public static string ToShamsiDateYmd(this DateTime date)
    {
        var persianCalendar = new System.Globalization.PersianCalendar();
        var intYear = persianCalendar.GetYear(date);
        var intMonth = persianCalendar.GetMonth(date);
        var intDay = persianCalendar.GetDayOfMonth(date);

        return intYear.ToString() + "/" + intMonth.ToString() + "/" + intDay.ToString();
    }

    /// <summary>
    /// Convert DateTime to Shamsi Date (DD/MM/YYYY)
    /// </summary>
    public static string ToShamsiDateDmy(this DateTime date)
    {
        var persianCalendar = new System.Globalization.PersianCalendar();
        var intYear = persianCalendar.GetYear(date);
        var intMonth = persianCalendar.GetMonth(date);
        var intDay = persianCalendar.GetDayOfMonth(date);

        return intDay.ToString() + "/" + intMonth.ToString() + "/" + intYear.ToString();
    }

    /// <summary>
    /// Convert DateTime to Shamsi String
    /// </summary>
    public static string ToShamsiDate(this DateTime date)
    {
        var persianCalendar = new System.Globalization.PersianCalendar();
        var intYear = persianCalendar.GetYear(date);
        var intMonth = persianCalendar.GetMonth(date);
        var intDayOfMonth = persianCalendar.GetDayOfMonth(date);
        var enDayOfWeek = persianCalendar.GetDayOfWeek(date);

        string strMonthName, strDayName;
        switch (intMonth)
        {
            case 1:
                strMonthName = "فروردین";
                break;
            case 2:
                strMonthName = "اردیبهشت";
                break;
            case 3:
                strMonthName = "خرداد";
                break;
            case 4:
                strMonthName = "تیر";
                break;
            case 5:
                strMonthName = "مرداد";
                break;
            case 6:
                strMonthName = "شهریور";
                break;
            case 7:
                strMonthName = "مهر";
                break;
            case 8:
                strMonthName = "آبان";
                break;
            case 9:
                strMonthName = "آذر";
                break;
            case 10:
                strMonthName = "دی";
                break;
            case 11:
                strMonthName = "بهمن";
                break;
            case 12:
                strMonthName = "اسفند";
                break;
            default:
                strMonthName = "";
                break;
        }

        switch (enDayOfWeek)
        {
            case DayOfWeek.Friday:
                strDayName = "جمعه";
                break;
            case DayOfWeek.Monday:
                strDayName = "دوشنبه";
                break;
            case DayOfWeek.Saturday:
                strDayName = "شنبه";
                break;
            case DayOfWeek.Sunday:
                strDayName = "یکشنبه";
                break;
            case DayOfWeek.Thursday:
                strDayName = "پنجشنبه";
                break;
            case DayOfWeek.Tuesday:
                strDayName = "سه شنبه";
                break;
            case DayOfWeek.Wednesday:
                strDayName = "چهارشنبه";
                break;
            default:
                strDayName = "";
                break;
        }

        return $"{strDayName} {intDayOfMonth.ToString()} {strMonthName} {intYear.ToString()}";
    }
}