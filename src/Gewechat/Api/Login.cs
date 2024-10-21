using Newtonsoft.Json.Linq;
using Utils = Gewechat.Utils;

namespace Gewechat.Api;


/// <summary>
/// 登录模块
/// </summary>
public static class Login
{
    /// <summary>
    /// 获取tokenId，将tokenId配置到Utils类中的token属性
    /// </summary>
    /// <returns></returns>
    public static async Task<JObject> GetToken()
    {
        return await Utils.PostJsonAsync("/tools/getTokenId", new JObject());
    }

    ///// <summary>
    ///// 设置微信消息的回调地址
    ///// </summary>
    ///// <param name="token"></param>
    ///// <param name="callbackUrl"></param>
    ///// <returns></returns>
    //public static JObject SetCallback(string token, string callbackUrl)
    //{
    //    var param = new JObject
    //    {
    //        ["token"] = token,
    //        ["callbackUrl"] = callbackUrl
    //    };
    //    return Utils.PostJSON("/tools/setCallback", param);
    //}

    ///// <summary>
    ///// 获取登录二维码
    ///// </summary>
    ///// <param name="appId">设备id，首次登录传空，后续登录传返回的appid</param>
    ///// <returns></returns>
    //public static JObject GetQr(string appId)
    //{
    //    JObject param = new JObject
    //    {
    //        ["appId"] = appId
    //    };
    //    return Utils.PostJSON("/login/getLoginQrCode", param);
    //}

    ///// <summary>
    ///// 确认登陆
    ///// </summary>
    ///// <param name="appId"></param>
    ///// <param name="uuid">取码返回的uuid</param>
    ///// <param name="captchCode">登录验证码（跨省登录会出现此提示，使用同省代理ip能避免此问题，也能使账号更加稳定）</param>
    ///// <returns></returns>
    //public static JObject CheckQr(string appId, string uuid, string captchCode)
    //{
    //    JObject param = new JObject
    //    {
    //        ["appId"] = appId,
    //        ["uuid"] = uuid,
    //        ["captchCode"] = captchCode
    //    };
    //    return Utils.PostJSON("/login/checkLogin", param);
    //}

    ///// <summary>
    ///// 退出微信
    ///// </summary>
    ///// <param name="appId"></param>
    ///// <returns></returns>
    //public static JObject LogOut(string appId)
    //{
    //    JObject param = new JObject
    //    {
    //        ["appId"] = appId
    //    };
    //    return Utils.PostJSON("/login/logout", param);
    //}

    ///// <summary>
    ///// 弹框登录
    ///// </summary>
    ///// <param name="appId"></param>
    ///// <returns></returns>
    //public static JObject DialogLogin(string appId)
    //{
    //    JObject param = new JObject
    //    {
    //        ["appId"] = appId
    //    };
    //    return Utils.PostJSON("/login/dialogLogin", param);
    //}

    ///// <summary>
    ///// 检查是否在线
    ///// </summary>
    ///// <param name="appId"></param>
    ///// <returns></returns>
    //public static JObject CheckOnline(string appId)
    //{
    //    JObject param = new JObject
    //    {
    //        ["appId"] = appId
    //    };
    //    return Utils.PostJSON("/login/checkOnline", param);
    //}

    ///// <summary>
    ///// 退出
    ///// </summary>
    ///// <param name="appId"></param>
    ///// <returns></returns>
    //public static JObject Logout(string appId)
    //{
    //    JObject param = new JObject
    //    {
    //        ["appId"] = appId
    //    };
    //    return Utils.PostJSON("/login/logout", param);
    //}
}
