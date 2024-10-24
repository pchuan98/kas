using Newtonsoft.Json;

namespace WeChat.Api.Shell.Models;

[Serializable]
public class WeChatInfo
{
    [JsonProperty("url")]
    public string? Url { get; set; }

    [JsonProperty("token")]
    public string? Token { get; set; }

    [JsonProperty("appid")]
    public string? AppId { get; set; }

    [JsonProperty("uuid")]
    public string? Uuid { get; set; }
}