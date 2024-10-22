using Newtonsoft.Json;

namespace Gewechat.Models;

public class LoginCheck : BaseResponse
{
    [JsonProperty("data")]
    public LoginCheckData? Data { get; set; }

    public class LoginCheckData
    {
        [JsonProperty("uuid")]
        public string? Uuid { get; set; }

        [JsonProperty("headImgUrl")]
        public string? HeadIconUrl { get; set; }

        [JsonProperty("nickName")]
        public string? NickName { get; set; }

        [JsonProperty("expiredTime")]
        public int ExpiredTime { get; set; }

        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("loginInfo")]
        public LoginInfo? Info { get; set; }
    }

    public class LoginInfo
    {
        [JsonProperty("uin")]
        public long Uin { get; set; }

        [JsonProperty("wxid")]
        public string? Wxid { get; set; }

        [JsonProperty("nickName")]
        public string? NickName { get; set; }

        [JsonProperty("mobile")]
        public string? Mobile { get; set; }

        [JsonProperty("alias")]
        public string? Alias { get; set; }
    }
}