using Newtonsoft.Json;

namespace KasTools.Models;

public class BaseTrendingObject
{
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("ticker")]
    public string? Ticker { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("count")]
    public long Count { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);

    }
}

/// <summary>
/// Live data from last 15 minutes
///
/// https://api-v2-do.kas.fyi/token/krc20/trending
/// </summary>
public class Trending
{
    /// <summary>
    /// 
    /// </summary>
    public BaseTrendingObject[]? MostMinted { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public BaseTrendingObject[]? MostTransferred { get; set; }
}