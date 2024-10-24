using System.Text.RegularExpressions;
using Gewechat;
using Gewechat.Models;
using Newtonsoft.Json;
using Wechat.Shell.Models;

namespace Wechat.Shell;

public static class WeChatGlobal
{
    internal static string Url => "http://127.0.0.1:2531/v2/api";

    internal static string Token => "23e1b0e5f20848388739ffd37300b2eb";

    internal static string AppId => "wx_Y0N3Sm-XH1CeDm4h5VCf1";

    internal static string Uuid => "QcfvrfGZ3nFYkxUl1BTZ";

    public static readonly WeChat WechatObject = new(Url, Token, AppId, Uuid);

    public static async Task SetCallbackUrl()
    {
        Serilog.Log.Information("Wechat is online: {online}", WechatObject.IsOnline);
        Serilog.Log.Information("Wechat set callback: {callback}", await WechatObject.SetCallbackUrl("http://100.64.78.26:5099/api/GewechatCallback"));
    }

    public static async Task Send(string wxid, string content)
    {
        var rect = await WechatObject.SendFriendStringMsg(wxid, content);

        if (!rect)
            Serilog.Log.Error("Send message error. {wxid} -> {msg}", wxid, content);
    }

    public static ReceiveMessageModel ParseCallback(string content)
    {
        var callback = JsonConvert.DeserializeObject<TextCallbackModel>(content);

        var sender = callback?.Data?.FromUserName?.WxId;
        var receiver = callback?.Data?.ToUserName?.WxId;

        if (callback?.Data?.MsgType != 1) return new ReceiveMessageModel();

        var wxid = callback?.Data?.FromUserName?.WxId;
        var msg = callback?.Data?.Content?.MessageContent;

        Serilog.Log.Verbose("{wxid} : {msg}", wxid, msg);

        if (string.IsNullOrEmpty(wxid) || string.IsNullOrEmpty(msg)) return new ReceiveMessageModel();

        var match = new Regex(@"^(.*:\n)?(.*)").Match(msg);

        if (!match.Success) return new ReceiveMessageModel();

        var user = match.Groups.Count == 3 ? match.Groups[1].Value.Replace(":\n", "") : null;
        var args = match.Groups[^1].Value;

        return new ReceiveMessageModel()
        {
            IsGroup = sender?.Contains("chatroom") is true,
            Group = sender,
            User = string.IsNullOrEmpty(user) ? sender ?? "" : user,
            Content = args
        };
    }

    public static async Task Login()
    {
        await WeChatGlobal.WechatObject.RequireToken();
        Console.WriteLine(await WeChatGlobal.WechatObject.RequireLoginQr());
        await WeChatGlobal.WechatObject.RequireLogin();
    }
}