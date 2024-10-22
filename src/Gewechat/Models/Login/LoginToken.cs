using Newtonsoft.Json;

namespace Gewechat.Models;

public class LoginToken : BaseResponse
{
    [JsonProperty("data")]
    public string? Data { get; set; }
}