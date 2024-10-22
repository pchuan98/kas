using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;
using Serilog.Debugging;

namespace Gewechat.Api;

public class BaseResponseApi
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

public class LoginToken : BaseResponseApi
{
    [JsonProperty("data")]
    public string? Data { get; set; }
}

public class LoginQr : BaseResponseApi
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

public class LoginCheck : BaseResponseApi
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


/// <summary>
/// 登录模块
/// </summary>
public class Login
{
    /// <summary>
    /// 
    /// </summary>
    private const string TokenRoute = "/tools/getTokenId";

    /// <summary>
    /// 
    /// </summary>
    private const string QrRoute = "/login/getLoginQrCode";

    /// <summary>
    /// 
    /// </summary>
    private const string CheckRoute = "/login/checkLogin";

    /// <summary>
    /// 
    /// </summary>
    public LoginToken? Token { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public LoginQr? Qr { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public LoginCheck? Check { get; private set; }

    /// <summary>
    /// 返回登录结果
    /// </summary>
    /// <param name="appid"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string?> LoginWithQr(string? appid = null)
    {
        var tokenResponse = await Utils.PostRequestAsync(TokenRoute);
        var token = JsonConvert.DeserializeObject<LoginToken>(tokenResponse);

        if (token is null || token.ReturnCode != 200)
            throw new Exception("Get Token Error.");

        Token = token;

        Serilog.Log.Information("Login token is {token}", token.Data);

        var headers = new Dictionary<string, string>
        {
            {"X-GEWE-TOKEN",$"{token.Data}"},
        };
        var body = new JObject
        {
            ["appId"] = appid ?? "",
        };

        Serilog.Log.Verbose($"Login Headers:\n{JsonConvert.SerializeObject(headers, Formatting.Indented)}");
        Serilog.Log.Verbose($"Login Body:\n{JsonConvert.SerializeObject(body, Formatting.Indented)}");

        var qrResponse = await Utils.PostRequestAsync(QrRoute, headers, body.ToString());
        var qr = JsonConvert.DeserializeObject<LoginQr>(qrResponse);

        Qr = qr;

        Serilog.Log.Information("Login Qr appid is {id}", qr?.Data?.Id);

        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(qr?.Data?.Qr ?? "", QRCodeGenerator.ECCLevel.M);
        var qrCode = new AsciiQRCode(qrCodeData);
        var qrCodeAsAsciiArt = qrCode.GetGraphic(1);

        return qrCodeAsAsciiArt;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <exception cref="Exception"></exception>
    private async Task<bool> LiveCheck(string? code = null)
    {
        if (Token is null || string.IsNullOrWhiteSpace(Token.Data))
            throw new Exception("Token not exists.");

        if (Qr?.Data is null || string.IsNullOrWhiteSpace(Qr.Data.Id))
            throw new Exception("Login id not exists.");

        var headers = new Dictionary<string, string>
        {
            {"X-GEWE-TOKEN",$"{Token.Data}"},
        };
        var body = new JObject
        {
            ["appId"] = Qr.Data.Id,
            ["uuid"] = Qr.Data.Uuid,
        };
        if (!string.IsNullOrWhiteSpace(code))
            body["captchCode"] = code;

        Serilog.Log.Verbose($"Login Headers:\n{JsonConvert.SerializeObject(headers, Formatting.Indented)}");
        Serilog.Log.Verbose($"Login Body:\n{JsonConvert.SerializeObject(body, Formatting.Indented)}");

        var checkResponse = await Utils.PostRequestAsync(CheckRoute, headers, body.ToString());
        var check = JsonConvert.DeserializeObject<LoginCheck>(checkResponse);

        Check = check;

        return check?.Data?.Info?.Wxid != null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task AutoLiveCheck()
    {
        var startTime = DateTime.Now;
        var code = "";

        do
        {   
            if (Check is not null) await Task.Delay(5000);                     // 5s触发一次

            if (File.Exists("ValidCode"))
                code = await File.ReadAllTextAsync("ValidCode");

            code = code.Replace("\r", "").Replace("\n", "");

            if (DateTime.Now - startTime <= TimeSpan.FromMinutes(3)) continue;

            Serilog.Log.Warning("Login timeout");
            break;

        } while (!await LiveCheck(code));

        if (Check?.Data?.Info?.Wxid is null) return;

        Serilog.Log.Information("Login wxid is {wxid}", Check.Data.Info.Wxid);
    }
}
