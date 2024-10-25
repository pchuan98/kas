using Newtonsoft.Json;

namespace Chuan.Core.Models;

public class CallbackModel
{
    /// <summary>
    /// 是否组消息
    /// </summary>
    public bool IsGroup { get; set; }

    /// <summary>
    /// 组名
    /// </summary>
    public string Group { get; set; } = "";

    /// <summary>
    /// 发信人
    /// </summary>
    public string Sender { get; set; } = "";

    /// <summary>
    /// 命令
    /// </summary>
    public string CommandName { get; set; } = "";

    /// <summary>
    /// 命令集合
    /// </summary>
    public string[]? Args { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}