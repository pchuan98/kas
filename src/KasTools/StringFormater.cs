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

    public static string ToHumanDateString(this TimeSpan time)
    {
        if (time.TotalMinutes < 60)
            return "{0:6}h";
        if (time.TotalHours < 24)
            return $"{(int)time.TotalHours,6}h";
        if (time.TotalDays < 100)
            return $"{(int)time.TotalDays,6}d";

        // todo add year
        var months = (int)(time.TotalDays / 30);
        return $"{$"{months}m{(int)(time.TotalDays % 30)}d",6}";
    }

    public static string ToHumanDateString(this DateTime t)
    {
        var time = DateTime.Now - t;

        if (time.TotalMinutes < 60)
            return "<1h";
        if (time.TotalHours < 24)
            return $"{(int)time.TotalHours,6}h";
        if (time.TotalDays < 100)
            return $"{(int)time.TotalDays,6}d";

        // todo add year
        var months = (int)(time.TotalDays / 30);
        return $"{$"{months}m{(int)(time.TotalDays % 30)}d",6}";
    }
}