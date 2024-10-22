using Newtonsoft.Json;

namespace Gewechat.Models;

public class LoginQr : BaseResponse
{
    [JsonProperty("data")]

    public QrObject? Data { get; set; }

    public class QrObject
    {
        [JsonProperty("appId")]
        public string? Id { get; set; }

        [JsonProperty("qrData")]
        public string? Qr { get; set; }

        [JsonProperty("qrImgBase64")]
        public string? Image { get; set; }

        [JsonProperty("uuid")]
        public string? Uuid { get; set; }
    }
}
