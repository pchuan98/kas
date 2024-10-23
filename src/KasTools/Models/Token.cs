using KasTools.Converters;
using KasTools.Models.Common;
using Newtonsoft.Json;

namespace KasTools.Models;

public class Token
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
    /// 币的icon
    /// </summary>
    [JsonProperty("iconUrl")]
    public string IconUrl { get; set; } = "";

    /// <summary>
    /// Minted -> 实际显示少了8个0
    /// </summary>
    [JsonProperty("maxSupply")]
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
    [JsonProperty("opScoreCreated")]
    public decimal OpscoreCreated { get; set; }

    /// <summary>
    /// Unknown
    /// </summary>
    [JsonProperty("opScoreUpdated")]
    public decimal OpscoreUpdated { get; set; }

    /// <summary>
    /// Mint Count
    /// </summary>
    [JsonProperty("preMint")]
    public decimal PreMint { get; set; }

    /// <summary>
    /// 价格相关参数
    /// </summary>
    [JsonProperty("price")]
    public PriceModel? Price { get; set; }

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
    /// 当前tiker的状态
    ///
    /// finished        -       Minted
    /// deployed        -       Minting
    /// </summary>
    [JsonProperty("status")]
    public string? Status { get; set; }

    /// <summary>
    /// 当前tiker的状态
    /// </summary>
    [JsonProperty("ticker")]
    public string? Ticker { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("totalMinted")]
    public decimal TotalMinted { get; set; }

    /// <summary>
    /// 24h 统计结果
    /// </summary>
    [JsonProperty("tradeVolume")]
    public TradeVolume? TradeVolume24H { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("transferTotal")]
    public long TransferTotal { get; set; }

    /// <summary>
    /// Mint的百分比 (0 - 1)
    /// </summary>
    public double MintPersent
        => Status == "deployed" ? (double)(TotalMinted / MaxSupply) : 1;

    /// <inheritdoc />
    public override string ToString()
        => $"{Ticker,-10}\t" +
           $"{((Price?.FloorPrice) ?? 0),12:F8}KAS\t\t" +
           $"{Price?.Change24H,5:0.##}%\t\t" +
           $"${TradeVolume24H?.AmountInUsd,-10:N0}\t" +
           $"{(DateTime.Now - DeployedAt).ToHumanDateString()}\t" +
           $"{MintPersent * 100,4:0.##}%\t" +
           $"{(MaxSupply / 100000000).ToLargeNumberSuffix(),10}\t" +
           $"{HolderTotal}";
}