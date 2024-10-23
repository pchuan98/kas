using Newtonsoft.Json;

namespace Gewechat.Models;

/// <summary>
/// 消息发送人
/// </summary>
public class FromUserNameModel
{
    /// <summary>
    /// 消息发送人的wxid
    /// </summary>
    [JsonProperty("string")]
    public string? WxId { get; set; }
}

/// <summary>
/// 消息接收人
/// </summary>
public class ToUserNameModel
{
    /// <summary>
    /// 消息接收人的wxid
    /// </summary>
    [JsonProperty("string")]
    public string? WxId { get; set; }
}

/// <summary>
/// 消息内容模型
/// </summary>
public class TextContentModel
{
    /// <summary>
    /// 消息内容
    /// </summary>
    [JsonProperty("string")]
    public string? MessageContent { get; set; }
}

/// <summary>
/// 图片缓冲区模型
/// </summary>
public class ImgBufModel
{
    /// <summary>
    /// 图片长度（字节）
    /// </summary>
    [JsonProperty("iLen")]
    public int Length { get; set; }
}

/// <summary>
/// 文本消息回调数据模型
/// </summary>
public class TextCallbackDataModel
{
    /// <summary>
    /// 消息ID
    /// </summary>
    [JsonProperty("MsgId")]
    public long MsgId { get; set; }

    /// <summary>
    /// 消息发送人
    /// </summary>
    [JsonProperty("FromUserName")]
    public FromUserNameModel? FromUserName { get; set; }

    /// <summary>
    /// 消息接收人
    /// </summary>
    [JsonProperty("ToUserName")]
    public ToUserNameModel? ToUserName { get; set; }

    /// <summary>
    /// 1、文本
    /// </summary>
    [JsonProperty("MsgType")]
    public int MsgType { get; set; }

    /// <summary>
    /// 消息内容
    /// </summary>
    [JsonProperty("Content")]
    public TextContentModel? Content { get; set; }

    /// <summary>
    /// 消息状态
    /// </summary>
    [JsonProperty("Status")]
    public int Status { get; set; }

    /// <summary>
    /// 图片状态
    /// </summary>
    [JsonProperty("ImgStatus")]
    public int ImgStatus { get; set; }

    /// <summary>
    /// 图片缓冲区
    /// </summary>
    [JsonProperty("ImgBuf")]
    public ImgBufModel? ImgBuf { get; set; }

    /// <summary>
    /// 消息创建时间（时间戳）
    /// </summary>
    [JsonProperty("CreateTime")]
    public long CreateTime { get; set; }

    /// <summary>
    /// 消息来源，包含 XML 格式的元数据
    /// </summary>
    [JsonProperty("MsgSource")]
    public string? MsgSource { get; set; }

    /// <summary>
    /// 消息推送内容，通常为简短的通知内容
    /// </summary>
    [JsonProperty("PushContent")]
    public string? PushContent { get; set; }

    /// <summary>
    /// 新消息ID
    /// </summary>
    [JsonProperty("NewMsgId")]
    public long NewMsgId { get; set; }

    /// <summary>
    /// 消息序列号
    /// </summary>
    [JsonProperty("MsgSeq")]
    public long MsgSeq { get; set; }
}

/// <summary>
/// 文本消息回调模型（空类，后续可扩展）
/// </summary>
public class TextCallbackModel : BaseCallbackModel<TextCallbackDataModel>;
