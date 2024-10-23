using Newtonsoft.Json;

namespace KasTools.Models.Common;

/// <summary>
/// 24H内
/// </summary>
public class TradeVolume
{
    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("amountInKas")]
    public decimal AmountInKas { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [JsonProperty("amountInUsd")]
    public decimal AmountInUsd { get; set; }
}