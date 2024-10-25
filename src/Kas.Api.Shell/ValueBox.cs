using Kas.Api.Shell.Models;

namespace Kas.Api.Shell;

public static class ValueBox
{
    /// <summary>
    /// WeChat的地址
    /// </summary>
    public static string WeChatApiBaseUrl = "http://localhost:5099/api";

    /// <summary>
    /// 
    /// </summary>
    public static readonly AliveInfo AliveKasInfo = new();
}