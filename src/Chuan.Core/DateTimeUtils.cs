namespace Chuan.Core;

public static class DateTimeUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static bool LessThan1Minutes(this DateTime time)
        => DateTime.Now - time <= TimeSpan.FromMinutes(1);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static bool LessThan2Minutes(this DateTime time)
        => DateTime.Now - time <= TimeSpan.FromMinutes(2);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static bool LessThan3Minutes(this DateTime time)
        => DateTime.Now - time <= TimeSpan.FromMinutes(3);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static bool LessThan4Minutes(this DateTime time)
        => DateTime.Now - time <= TimeSpan.FromMinutes(4);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static bool LessThan5Minutes(this DateTime time)
        => DateTime.Now - time <= TimeSpan.FromMinutes(5);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static bool LessThanHalfHour(this DateTime time)
        => DateTime.Now - time <= TimeSpan.FromMinutes(30);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static bool LessThan1Hour(this DateTime time)
        => DateTime.Now - time <= TimeSpan.FromHours(1);
}