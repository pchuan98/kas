using Gewechat.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Chuan.Core.Models;
using Newtonsoft.Json.Linq;

namespace WeChat.Api.Shell.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CallbackController : ControllerBase
{
    /// <summary>
    /// 切割命令和参数
    /// </summary>
    private static readonly Regex MessagePattern = new(@"^/(\w+)(?:\s+([^\s,，、]+(?:[,\s，、][^\s,，、]+)*))?$");

    /// <summary>
    /// 识别是否为命令
    /// </summary>
    private static readonly Regex CommandValidPattern = new(@"^/\w+.*?");

    [HttpGet]
    public string Main()
    {
        return $"{Request.Scheme}://{Request.Host}{Request.Path}/{Request.PathBase}";
    }

    [HttpPost]
    public async void WeChatCallback([FromBody] object content)
    {
        var json = content.ToString() ?? "";

        var obj = JsonConvert.DeserializeObject<TextCallbackModel>(json);

        var fromWxid = obj?.Data?.FromUserName?.WxId;
        var toWxid = obj?.Data?.ToUserName?.WxId;

        var msg = obj?.Data?.Content?.MessageContent;

        Serilog.Log.Verbose("{from} -> {to} : {msg}", fromWxid, toWxid, msg);

        if (obj?.Data?.MsgType != 1
            || string.IsNullOrEmpty(fromWxid)
            || string.IsNullOrEmpty(msg)) return;

        // 群名称，发送人名称，命令，参数
        var callback = new CallbackModel
        {
            IsGroup = fromWxid?.Contains("@chatroom") ?? false,
            Group = fromWxid ?? "",
        };

        if (callback.IsGroup)
        {
            var words = msg.Split(":\n", 2, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length != 2)
            {
                Serilog.Log.Error("Group Message Parse Error. {msg}", msg);
                return;
            }

            callback.Sender = words[0];
            msg = words[1];
        }
        else callback.Sender = fromWxid ?? "";


        if (!CommandValidPattern.IsMatch(msg.Trim())) return;

        var match = MessagePattern.Match(msg.Trim());
        if (!match.Success)
        {

            Serilog.Log.Error("Command Parse Error. {msg}", msg);
            return;
        }

        callback.CommandName = match.Groups[1].Value;
        callback.Args = match.Groups[2].Success
            ? match.Groups[2].Value.Split(new[] { ' ', ',', '，', '、' }
                , StringSplitOptions.RemoveEmptyEntries)
            : null;

        Serilog.Log.Information("Send Command :\n{callback}", callback);

        // 授权
        var isPermission = await CheckPermissionAsync(callback);

        if (!isPermission) return;

        // 广播
        try
        {
            if (callback.CommandName == "wechat")
            {
                if (callback.Args?.Length >= 1)
                {
                    callback.CommandName = callback.Args[0];
                    callback.Args = callback.Args[1..];
                }
                else // todo 输出wechat所有的命令
                {
                    return;
                }
            }

            var baseUrl = callback.CommandName == "wechat"
                ? $"{Request.Scheme}://{Request.Host}/api"
                : ValueBox.BroadcastUrl;

            var broadcastUrl = $"{baseUrl}/{callback.CommandName.ToLower()}";

            await Client.PostAsJsonAsync(broadcastUrl, callback);
        }
        catch (Exception e)
        {
            Serilog.Log.Error(e.Message);
        }
    }

    private static readonly HttpClient Client = new();

    private async Task<bool> CheckPermissionAsync(CallbackModel callback)
    {
        var url = $"{Request.Scheme}://{Request.Host}/api/permissions/check";

        var response = await Client.PostAsJsonAsync(url, callback);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var ret = JObject.Parse(result)["HasPermission"]?.ToString();

            bool.TryParse(ret, out var per);

            return per;
        }

        Serilog.Log.Error("Failed to check permission.");
        return false;
    }
}
