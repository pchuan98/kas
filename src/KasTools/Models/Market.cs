using Newtonsoft.Json;

namespace KasTools.Models;

public class Market
{
    /// <summary>
    /// /
    /// </summary>
    [JsonProperty("marketCap")]
    public double MarketCap { get; set; }

    /// <summary>
    /// 当前对应的美元价格
    /// </summary>
    [JsonProperty("price")]
    public double Price { get; set; }

    [JsonProperty("priceChange24h")]
    public double PriceChange24H { get; set; }

    [JsonProperty("rank")]
    public int Rank { get; set; }

    [JsonProperty("volume24h")]
    public double Volume24H { get; set; }
}