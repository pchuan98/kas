using Newtonsoft.Json.Linq;
using Gewechat.Models;
using Newtonsoft.Json;
using QRCoder;

namespace Gewechat;

// todo 所有和登录已经不登陆有关的内容
partial class WeChat
{
    public bool IsOnline
    {
        get
        {
            if (string.IsNullOrEmpty(Token) || string.IsNullOrEmpty(AppId)) return false;

            var body = new JObject
            {
                ["appId"] = AppId ?? "",
            };

            var response = PostJsonRequestWithHeaderAsync("/login/checkOnline", body.ToString()).Result;
            var result = JObject.Parse(response);
            return result["data"]?.Value<bool?>() ?? false;
        }
    }

    /// <summary>
    /// Step 1
    ///
    /// You can require a token string. (Must holder)
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<string?> RequireToken()
    {
        var tokenResponse = await PostJsonRequestAsync("/tools/getTokenId");
        var token = JsonConvert.DeserializeObject<LoginToken>(tokenResponse);

        if (token is null || token.ReturnCode != 200)
            throw new Exception("Get Token Error.");

        Token = token.Data;
        Serilog.Log.Information("Login token is {token}", Token);

        return Token;
    }

    /// <summary>
    /// Step 2
    ///
    /// Will return a qr-img-as-str
    /// </summary>
    /// <returns>
    /// 二维码字符串形式
    /// </returns>
    /// <exception cref="Exception"></exception>
    public async Task<string?> RequireLoginQr()
    {
        var body = new JObject
        {
            ["appId"] = AppId ?? ""
        };

        var qrResponse = await PostJsonRequestWithHeaderAsync("/login/getLoginQrCode", body.ToString());
        var qr = JsonConvert.DeserializeObject<LoginQr>(qrResponse);

        if (qr!.ReturnCode == 500)
        {
            Serilog.Log.Error("Qr Get Error 500. Message is {msg}", qr.Message);
            throw new Exception(qr.Message);
        }

        AppId = qr.Data?.Id;
        Uuid = qr.Data?.Uuid;

        Serilog.Log.Information("Login Qr appid is {id}", AppId);
        Serilog.Log.Information("Login Qr uuid is {uuid}", Uuid);

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
    private async Task<LoginCheck?> LiveCheck(string? code = null)
    {
        var body = new JObject
        {
            ["appId"] = AppId,
            ["uuid"] = Uuid
        };
        if (!string.IsNullOrWhiteSpace(code))
            body["captchCode"] = code;

        var checkResponse = await PostJsonRequestWithHeaderAsync("/login/checkLogin", body.ToString());
        var check = JsonConvert.DeserializeObject<LoginCheck>(checkResponse);

        return check;
    }

    /// <summary>
    /// Step 3
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool?> RequireLogin()
    {
        var startTime = DateTime.Now;
        var code = "";

        var check = await LiveCheck();

        while (check?.Data?.Info?.Wxid is null)
        {
            if (DateTime.Now - startTime > TimeSpan.FromMinutes(3)) break;

            if (File.Exists(CAPTCH_CODE_PATH))
                code = await File.ReadAllTextAsync(CAPTCH_CODE_PATH);

            code = code.Replace("\r", "").Replace("\n", "").Replace(" ", "").Trim();
            check = await LiveCheck(code);

            await Task.Delay(5000);
        }

        if (check?.Data?.Info?.Wxid is null) return false;
        Wxid = check?.Data?.Info?.Wxid;

        Serilog.Log.Information("Login wxid is {wxid}", Wxid);
        return true;
    }

    public async Task<bool?> RequireReLogin()
    {
        var body = new JObject
        {
            ["appId"] = $"{AppId}"
        };

        var callback = await PostJsonRequestWithHeaderAsync("/login/dialogLogin", body.ToString());
        var response = JsonConvert.DeserializeObject<BaseResponse>(callback);

        if (response?.ReturnCode == 200)
            return await RequireLogin();

        return false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<bool?> RequireLoginout()
    {
        var body = new JObject
        {
            ["appId"] = $"{AppId}"
        };

        var callback = await PostJsonRequestWithHeaderAsync("/login/logout", body.ToString());
        var response = JsonConvert.DeserializeObject<BaseResponse>(callback);

        return response?.ReturnCode == 200;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task<bool> SetCallbackUrl(string url)
    {
        var body = new JObject
        {
            ["token"] = $"{Token}",
            ["callbackUrl"] = url,
        };

        var callback = await PostJsonRequestWithHeaderAsync("/tools/setCallback", body.ToString());
        var response = JsonConvert.DeserializeObject<BaseResponse>(callback);

        return response?.ReturnCode == 200;
    }
}