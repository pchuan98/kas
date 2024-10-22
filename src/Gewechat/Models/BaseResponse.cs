using Newtonsoft.Json;

namespace Gewechat.Models;

public class BaseResponse
{
    [JsonProperty("ret")]
    public int ReturnCode { get; set; }

    [JsonProperty("msg")]
    public string? Message { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}