namespace Gewechat;

public partial class WeChat
{
    /// <summary>
    /// 执行程序的 CaptchCode 文件里面
    /// </summary>
    public const string CAPTCH_CODE_PATH = "CaptchCode";
}

/// <summary>
/// 
/// </summary>
/// <param name="url"></param>
/// <param name="token"></param>
/// <param name="appid"></param>
/// <param name="uuid"></param>
public partial class WeChat(
    string url,
    string? token = null,
    string? appid = null,
    string? uuid = null)
{
    /// <summary>
    /// Session Token
    ///
    /// such as 23e1b0e5f20848388739ffd37300b2eb
    /// </summary>
    public string? Token { get; set; } = token;

    /// <summary>
    /// 设备ID，尽量保证使用整个Id去登录
    /// 
    /// sunch as wx_Y0N3Sm-XH1CeDm4h5VCf1
    /// </summary>
    public string? AppId { get; set; } = appid;

    /// <summary>
    /// 唯一标识
    ///
    /// such as Qd8ifWDRorYhzp7MdXXU
    /// </summary>
    public string? Uuid { get; set; } = uuid;

    /// <summary>
    /// 当前登录账号的唯一标识
    /// </summary>
    public string? Wxid { get; set; }

    /// <summary>
    /// api地址前缀
    /// http://xx.xx.xx.xx:xx/v2/api
    /// </summary>
    public string? ApiUrl { get; set; } = url;
}