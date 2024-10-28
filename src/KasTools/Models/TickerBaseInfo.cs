using KasTools.Converters;
using Newtonsoft.Json;

namespace KasTools.Models;

public class TickerBaseInfo
{
    /// <summary>
    /// 名字
    /// </summary>
    public string Ticker { get; set; } = "";

    /// <summary>
    /// 被谁部署
    /// </summary>
    public string? DeployedBy { get; set; } = "";

    /// <summary>
    /// 部署时间
    /// </summary>
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? DeployedAt { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}