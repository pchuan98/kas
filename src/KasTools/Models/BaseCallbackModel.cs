using Newtonsoft.Json;

namespace KasTools.Models;

public class BaseCallbackModel<T>
{
    /// <summary>
    /// 消息类型
    /// </summary>
    [JsonProperty("TypeName")]
    public string? TypeName { get; set; }

    /// <summary>
    /// 设备ID
    /// </summary>
    [JsonProperty("Appid")]
    public string? AppId { get; set; }

    /// <summary>
    /// 所属微信的wxid
    /// </summary>
    [JsonProperty("Wxid")]
    public string? WxId { get; set; }

    /// <summary>
    /// 消息实体
    /// </summary>
    [JsonProperty("Data")]
    public T? Data { get; set; }

    public override string ToString()
        => JsonConvert.SerializeObject(this, Formatting.Indented);
}