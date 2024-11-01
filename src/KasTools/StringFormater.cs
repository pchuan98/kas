using System.Globalization;

namespace KasTools;

public static class StringFormater
{
    public static string ToLargeNumberSuffix(this decimal num)
    {
        return num switch
        {
            >= 1_000_000_000 => $"{num / 1000000000:0.###}B",
            >= 1_000_000 => $"{num / 1000000:0.###}M",
            >= 1_000 => $"{num / 1000:0.###}K",
            _ => num.ToString(CultureInfo.InvariantCulture)
        };
    }

    public static string ToLargeNumberSuffix(this double num)
    {
        return num switch
        {
            >= 1_000_000_000 => $"{num / 1000000000:0.###}B",
            >= 1_000_000 => $"{num / 1000000:0.###}M",
            >= 1_000 => $"{num / 1000:0.###}K",
            _ => num.ToString(CultureInfo.InvariantCulture)
        };
    }

    public static string ToHumanDateString(this TimeSpan time)
    {
        if (time.TotalMinutes < 60)
            return $"{time.TotalHours:.###}h";
        if (time.TotalHours < 24)
            return $"{time.TotalHours:.###}h";
        if (time.TotalDays < 100)
            return $"{time.TotalDays:.###}d";

        // todo add year
        var months = (int)(time.TotalDays / 30);
        return $"{months}m{(int)(time.TotalDays % 30)}d";
    }

    public static string ToHumanDateString(this DateTime t)
        => ToHumanDateString(DateTime.Now - t);
}