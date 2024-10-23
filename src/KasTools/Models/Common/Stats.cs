using KasTools.Converters;
using Newtonsoft.Json;

namespace KasTools.Models.Common;

public class StatsHistory
{
    /// <summary>
    /// time
    /// </summary>
    [JsonProperty("t")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime Time { get; set; }

    /// <summary>
    /// holder
    /// </summary>
    [JsonProperty("h")]
    public int HolderCount { get; set; }

    /// <summary>
    /// mint
    /// </summary>
    [JsonProperty("m")]
    public int MintCount { get; set; }

    /// <summary>
    /// tranfer
    /// </summary>
    [JsonProperty("tf")]
    public int TransferCount { get; set; }
}

public class StatsChart
{
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("statsHistory")]
    public List<StatsHistory>? StatsHistories { get; set; }
}