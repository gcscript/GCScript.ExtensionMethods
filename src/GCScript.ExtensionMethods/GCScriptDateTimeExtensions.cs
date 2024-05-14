using System;

namespace GCScript.ExtensionMethods;

public static class GCScriptDateTimeExtensions
{
    /// <summary>
    /// Converts a given date-time to yyyy-MM-ddTHH:mm:ssZ. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>Returns string in the format of yyyy-MM-ddTHH:mm:ssZ</returns>
    public static string ToISO8601(this DateTime dateTime) => dateTime.ToString("yyyy-MM-ddTHH:mm:ssZ");

    /// <summary>
    /// Converts a given date-time to yyyy-MM-ddTHH:mm:ssZ. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTimeOffset">The date time.</param>
    /// <returns>Returns string in the format of yyyy-MM-ddTHH:mm:ssZ</returns>
    public static string ToISO8601(this DateTimeOffset dateTimeOffset) => dateTimeOffset.ToString("yyyy-MM-ddTHH:mm:ssZ");

    /// <summary>
    /// Converts a given date-time to yyyy-MM-ddTHH:mm:ssZ. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <returns>Returns string in the format of yyyy-MM-ddTHH:mm:ssZ</returns>
    public static string ToISO8601(this DateTime? dateTime)
    {
        if (dateTime == null) { return ""; }
        return dateTime.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    /// <summary>
    /// Converts a given date-time to yyyy-MM-ddTHH:mm:ssZ. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTimeOffset">The date time.</param>
    /// <returns>Returns string in the format of yyyy-MM-ddTHH:mm:ssZ</returns>
    public static string ToISO8601(this DateTimeOffset? dateTimeOffset)
    {
        if (dateTimeOffset == null) { return ""; }
        return dateTimeOffset.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }

    /// <summary>
    /// Converts a given date-time to dd-MM-yyyy. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of dd-MM-yyyy</returns>
    public static string ToDDMMYYYY(this DateTime dateTime, char separator = '-') => dateTime.ToString($"dd{separator}MM{separator}yyyy");

    /// <summary>
    /// Converts a given date-time to dd-MM-yyyy. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of dd-MM-yyyy</returns>
    public static string ToDDMMYYYY(this DateTimeOffset dateTimeOffset, char separator = '-') => dateTimeOffset.ToString($"dd{separator}MM{separator}yyyy");

    /// <summary>
    /// Converts a given date-time to dd-MM-yyyy. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of dd-MM-yyyy</returns>
    public static string ToDDMMYYYY(this DateTime? dateTime, char separator = '-')
    {
        if (dateTime == null) { return ""; }
        return dateTime.Value.ToString($"dd{separator}MM{separator}yyyy");
    }

    /// <summary>
    /// Converts a given date-time to dd-MM-yyyy. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of dd-MM-yyyy</returns>
    public static string ToDDMMYYYY(this DateTimeOffset? dateTimeOffset, char separator = '-')
    {
        if (dateTimeOffset == null) { return ""; }
        return dateTimeOffset.Value.ToString($"dd{separator}MM{separator}yyyy");
    }

    /// <summary>
    /// Converts a given date-time to yyyy-MM-dd. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of yyyy-MM-dd</returns>
    public static string ToYYYYMMDD(this DateTime dateTime, char separator = '-') => dateTime.ToString($"yyyy{separator}MM{separator}dd");

    /// <summary>
    /// Converts a given date-time to yyyy-MM-dd. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of yyyy-MM-dd</returns>
    public static string ToYYYYMMDD(this DateTimeOffset dateTimeOffset, char separator = '-') => dateTimeOffset.ToString($"yyyy{separator}MM{separator}dd");

    /// <summary>
    /// Converts a given date-time to yyyy-MM-dd. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of yyyy-MM-dd</returns>
    public static string ToYYYYMMDD(this DateTime? dateTime, char separator = '-')
    {
        if (dateTime == null) { return ""; }
        return dateTime.Value.ToString($"yyyy{separator}MM{separator}dd");
    }

    /// <summary>
    /// Converts a given date-time to yyyy-MM-dd. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of yyyy-MM-dd</returns>
    public static string ToYYYYMMDD(this DateTimeOffset? dateTimeOffset, char separator = '-')
    {
        if (dateTimeOffset == null) { return ""; }
        return dateTimeOffset.Value.ToString($"yyyy{separator}MM{separator}dd");
    }

    /// <summary>
    /// Converts a given date-time to MM/dd/yyyy. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of MM/dd/yyyy</returns>
    public static string ToMMDDYYYY(this DateTime dateTime, char separator = '-') => dateTime.ToString($"MM{separator}dd{separator}yyyy");

    /// <summary>
    /// Converts a given date-time to MM/dd/yyyy. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of MM/dd/yyyy</returns>
    public static string ToMMDDYYYY(this DateTimeOffset dateTimeOffset, char separator = '-') => dateTimeOffset.ToString($"MM{separator}dd{separator}yyyy");

    /// <summary>
    /// Converts a given date-time to MM/dd/yyyy. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of MM/dd/yyyy</returns>
    public static string ToMMDDYYYY(this DateTime? dateTime, char separator = '-')
    {
        if (dateTime == null) { return ""; }
        return dateTime.Value.ToString($"MM{separator}dd{separator}yyyy");
    }

    /// <summary>
    /// Converts a given date-time to MM/dd/yyyy. You can also specify the separator. 
    /// </summary>
    /// <param name="dateTime">The date time.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>Returns string in the format of MM/dd/yyyy</returns>
    public static string ToMMDDYYYY(this DateTimeOffset? dateTimeOffset, char separator = '-')
    {
        if (dateTimeOffset == null) { return ""; }
        return dateTimeOffset.Value.ToString($"MM{separator}dd{separator}yyyy");
    }

    /// <summary>
    /// Converts a given date-time to HH:mm. You can also specify the separator.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="separator"></param>
    /// <returns>Returns string in the format of HH:mm</returns>
    public static string ToHHMM(this DateTime dateTime, char separator = ':') => dateTime.ToString($"HH{separator}mm");

    /// <summary>
    /// Converts a given date-time to HH:mm. You can also specify the separator.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="separator"></param>
    /// <returns>Returns string in the format of HH:mm</returns>
    public static string ToHHMM(this DateTimeOffset dateTimeOffset, char separator = ':') => dateTimeOffset.ToString($"HH{separator}mm");

    /// <summary>
    /// Converts a given date-time to HH:mm. You can also specify the separator.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="separator"></param>
    /// <returns>Returns string in the format of HH:mm</returns>
    public static string ToHHMM(this DateTime? dateTime, char separator = ':')
    {
        if (dateTime == null) { return ""; }
        return dateTime.Value.ToString($"HH{separator}mm");
    }

    /// <summary>
    /// Converts a given date-time to HH:mm. You can also specify the separator.
    /// </summary>
    /// <param name="dateTimeOffset"></param>
    /// <param name="separator"></param>
    /// <returns>Returns string in the format of HH:mm</returns>
    public static string ToHHMM(this DateTimeOffset? dateTimeOffset, char separator = ':')
    {
        if (dateTimeOffset == null) { return ""; }
        return dateTimeOffset.Value.ToString($"HH{separator}mm");
    }

    /// <summary>
    /// Determines whether the date is Today's date or Not.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>
    ///   <c>true</c> if the specified date is today; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsToday(this DateTime date)
    {
        return date.Date == DateTime.Today.Date;
    }

    /// <summary>
    /// Determines whether the date is Today's date or Not.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>
    ///   <c>true</c> if the specified date is today; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsToday(this DateTime? date)
    {
        if (date == null) { return false; }
        return date.Value.Date == DateTime.Today.Date;
    }

    /// <summary>
    /// Determines whether the date is Today's date or Not.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>
    ///   <c>true</c> if the specified date is today; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsToday(this DateTimeOffset date)
    {
        return date.Date == DateTime.Today.Date;
    }

    /// <summary>
    /// Determines whether the date is Today's date or Not.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>
    ///   <c>true</c> if the specified date is today; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsToday(this DateTimeOffset? date)
    {
        if (date == null) { return false; }
        return date.Value.Date == DateTime.Today.Date;
    }
}
