using Newtonsoft.Json;

namespace Kas.Test.Console;

public class UnixTimestampConverter : JsonConverter<DateTime>
{
    public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
        => writer.WriteValue(((DateTimeOffset)value).ToUnixTimeMilliseconds());

    public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
        => DateTimeOffset.FromUnixTimeMilliseconds((long)(reader.Value ?? 0)).LocalDateTime;
}

public class KasInfo
{
    /// <summary>
    /// Unknown
    /// </summary>
    [JsonProperty("decimal")]
    public int Decimal { get; set; }

    /// <summary>
    /// 部署时间
    /// </summary>
    [JsonProperty("deployedAt")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime DeployedAt { get; set; }

    /// <summary>
    /// 持有的总人数
    /// </summary>
    [JsonProperty("holderTotal")]
    public long HolderTotal { get; set; }

    /// <summary>
    /// 前50个持有者
    /// </summary>
    [JsonProperty("holders")]
    public List<Holder>? Holders { get; set; }

    /// <summary>
    /// 币的icon
    /// </summary>
    [JsonProperty("iconurl")]
    public string IconUrl { get; set; } = "";

    /// <summary>
    /// 一般就一个
    /// </summary>
    [JsonProperty("marketsData")]
    public List<MarketsData>? MarketsData { get; set; }

    /// <summary>
    /// Minted -> 实际显示少了8个0
    /// </summary>
    [JsonProperty("maxsupply")]
    public decimal MaxSupply { get; set; }

    /// <summary>
    /// Unknown
    /// </summary>
    [JsonProperty("mintLimit")]
    public decimal MintLimit { get; set; }

    /// <summary>
    /// Mint Count
    /// </summary>
    [JsonProperty("mintTotal")]
    public decimal MintTotal { get; set; }

    /// <summary>
    /// Unknown
    /// </summary>
    [JsonProperty("opscorecreated")]
    public decimal OpscoreCreated { get; set; }

    /// <summary>
    /// Unknown
    /// </summary>
    [JsonProperty("opscoreUpdated")]
    public decimal OpscoreUpdated { get; set; }

    /// <summary>
    /// Premint = value / TotalMinted
    /// </summary>
    [JsonProperty("preMint")]
    public decimal PreMint { get; set; }

    /// <summary>
    /// 价格相关参数
    /// </summary>
    [JsonProperty("price")]
    public Price? Price { get; set; }

    /// <summary>
    /// 价格趋势集合
    /// </summary>
    [JsonProperty("priceHistory")]
    public List<PriceHistory>? PriceHistories { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("revealHash")]
    public string RevealHash { get; set; } = "";

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("socialLinks")]
    public List<SocialLink> SocialLinks { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("statsHistory")]
    public List<StatsHistory> StatsHistories { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("ticker")]
    public string Ticker { get; set; }

    [JsonProperty("totalMinted")]
    public decimal TotalMinted { get; set; }

    [JsonProperty("tradeVolume")]
    public TradeVolume TradeVolume { get; set; }

    [JsonProperty("transferTotal")]
    public int TransferTotal { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}

/// <summary>
/// 
/// </summary>
public class Holder
{
    /// <summary>
    /// 持有者钱包地址
    /// </summary>
    [JsonProperty("address")]
    public string Address { get; set; } = "";

    /// <summary>
    /// 持有者持有数量
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }
}

public class MarketsData
{
    /// <summary>
    /// KSPR Bot     不知道什么意思
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// 贸易数据
    /// </summary>
    [JsonProperty("marketData")]
    public MarketData MarketData { get; set; }

    /// <summary>
    /// 贸易元数据
    /// </summary>
    [JsonProperty("metadata")]
    public MarketsDataMetadata Metadata { get; set; }
}

/// <summary>
/// 
/// </summary>
public class MarketData
{
    /// <summary>
    /// 浮动价格的美元价格
    /// </summary>
    [JsonProperty("priceInUsd")]
    public double PriceInUsd { get; set; }

    /// <summary>
    /// Volume (24h)
    /// </summary>
    [JsonProperty("volumeInUsd")]
    public double VolumeInUsd { get; set; }
}

/// <summary>
/// 
/// </summary>
public class MarketsDataMetadata
{
    /// <summary>
    /// KSPR Bot
    /// </summary>
    [JsonProperty("iconUrl")]
    public string IconUrl { get; set; }

    /// <summary>
    /// kspr.jpg
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; }

    /// <summary>
    /// Bot 地址
    /// </summary>
    [JsonProperty("url")]
    public string Url { get; set; }
}

/// <summary>
/// 
/// </summary>
public class Price
{
    /// <summary>
    /// Float KAS Price
    /// </summary>
    [JsonProperty("floorPrice")]
    public double FloorPrice { get; set; }

    /// <summary>
    /// todo 待检验
    /// </summary>
    [JsonProperty("marketcapInUsd")]
    public double MarketcapInUsd { get; set; }

    /// <summary>
    /// Unknown
    /// </summary>
    [JsonProperty("change24h")]
    public double Change24h { get; set; }
}

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

public class SocialLink
{
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("url")]
    public string Url { get; set; }
}

public class StatsHistory
{
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("t")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime Time { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("h")]
    public int HolderCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("m")]
    public int MintCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("tf")]
    public int TransferCount { get; set; }
}

public class TradeVolume
{
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    [JsonProperty("amountInKas")]
    public decimal AmountInKas { get; set; }

    [JsonProperty("amountInUsd")]
    public decimal AmountInUsd { get; set; }
}