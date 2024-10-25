namespace WeChat.Api.Shell;

public static class ValueBox
{
    /// <summary>
    /// 微信登录的静态信息
    /// </summary>
    public const string WE_CHAT_CONFIG_PATH = "WECHAT_CONFIG.json";

    /// <summary>
    /// 将命令广播出去的基础地址
    /// </summary>
    public static string BroadcastUrl = "http://localhost:5098/api";

    /// <summary>
    /// 
    /// </summary>
    public static string LocalUrl = "http://localhost:5099/api";
}