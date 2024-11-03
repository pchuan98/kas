using System.Collections.Concurrent;
using Chuan.Core.Models;

namespace WeChat.Api.Shell;

public static class ValueBox
{
    /// <summary>
    /// 微信登录的静态信息
    /// </summary>
    public const string WE_CHAT_CONFIG_PATH = "WECHAT_CONFIG.json";

    /// <summary>
    /// 
    /// </summary>
    public static string LocalUrl = "http://localhost:5099/api";

    /// <summary>
    /// 
    /// </summary>
    public static ConcurrentBag<RegisterModel> CommandCollection = new();
}