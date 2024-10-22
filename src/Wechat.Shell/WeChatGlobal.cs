using Gewechat;

namespace Wechat.Shell;

public static class WeChatGlobal
{
    internal static string Url => "http://127.0.0.1:2531/v2/api";

    internal static string Token => "23e1b0e5f20848388739ffd37300b2eb";

    internal static string AppId => "wx_Y0N3Sm-XH1CeDm4h5VCf1";

    internal static string Uuid => "Yf_c-xWmHJLHGyJShSMk";

    public static readonly WeChat WechatObject = new WeChat(Url, Token, AppId, Uuid);

    public static async Task SetCallbackUrl()
    {
        Console.WriteLine(WechatObject.IsOnline);
        Console.WriteLine(await WechatObject.SetCallbackUrl("http://100.64.4.199:5099/api/GewechatCallback"));
    }
}