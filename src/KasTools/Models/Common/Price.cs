using KasTools.Converters;
using Newtonsoft.Json;

namespace KasTools.Models;

/// <summary>
/// 价格记录，对象为简略模式
/// </summary>
public class PriceHistory
{
    /// <summary>
    /// 价格
    /// </summary>
    [JsonProperty("p")]
    public double Price { get; set; }

    /// <summary>
    /// 时间点
    /// </summary>
    [JsonProperty("t")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime Time { get; set; }
}


public class PriceChart
{

    /// <summary>
    /// 价格趋势集合
    /// </summary>
    [JsonProperty("priceHistory")]
    public List<PriceHistory>? PriceHistories { get; set; }
}

/// <summary>
/// 
/// </summary>
public class PriceModel
{
    /// <summary>
    /// Floor KAS Price
    /// </summary>
    [JsonProperty("floorPrice")]
    public double FloorPrice { get; set; }

    /// <summary>
    /// 市值
    /// </summary>
    [JsonProperty("marketCapInUsd")]
    public double MarketcapInUsd { get; set; }

    /// <summary>
    /// 24h的改变值
    /// </summary>
    [JsonProperty("change24h")]
    public double Change24H { get; set; }
}