namespace Wechat.Shell.Models;

/// <summary>
/// 接受后的微信消息解析到这里
/// </summary>
public class ReceiveMessageModel
{
    /// <summary>
    /// 是否为群组id
    /// </summary>
    public bool IsGroup { get; set; } = false;

    /// <summary>
    /// 群组的id
    /// </summary>
    public string? Group { get; set; }

    /// <summary>
    /// 发送消息的用户id
    /// </summary>
    public string User { get; set; } = "";

    /// <summary>
    /// 发送的内容
    /// </summary>
    public string Content { get; set; } = "";
}