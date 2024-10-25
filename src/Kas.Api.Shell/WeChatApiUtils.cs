using Chuan.Core;
using Chuan.Core.Models;

namespace Kas.Api.Shell;

public static class WeChatApiUtils
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="wxid"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static async Task<string?> SendMessage(string wxid, string content)
    {
        var result = await ClientUtils.Post($"{ValueBox.WeChatApiBaseUrl}/send", new SendModel()
        {
            Receiver = wxid,
            Content = content
        });

        return result;
    }

    /// <summary>
    /// 快速发消息
    /// </summary>
    /// <param name="model"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static async Task<string?> SendMessage(this CallbackModel model, string content)
        => await SendMessage(model.IsGroup ? model.Group : model.Sender, content);

}