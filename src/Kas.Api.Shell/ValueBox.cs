using Kas.Api.Shell.Models;

namespace Kas.Api.Shell;

public static class ValueBox
{
    /// <summary>
    /// WeChat的地址
    /// </summary>
    public static string WeChatApiBaseUrl = "http://122.152.227.199:5099/api";

    /// <summary>
    /// 
    /// </summary>
    public static readonly AliveInfo AliveKasInfo = new();
}