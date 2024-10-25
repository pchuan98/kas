using Newtonsoft.Json;

namespace Chuan.Core.Models;

public class SendModel
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public int TypeId { get; set; } = 0;

    /// <summary>
    /// 接收人
    /// </summary>
    public string? Receiver { get; set; }

    /// <summary>
    /// 发送的内容
    ///
    /// todo 后期这里要改成object，用来发送任意类型的数据
    /// </summary>
    public string? Content { get; set; }


    public override string ToString()
        => JsonConvert.SerializeObject(this, Formatting.Indented);

}