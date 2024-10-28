using Gewechat.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Chuan.Core;
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

    [HttpGet("commands")]
    public IActionResult GetCommands()
    {
        return Ok();
        throw new Exception();
    }


    [HttpPost("register")]
    public IActionResult RegisterCommand([FromBody] object command)
    {
        var obj = JsonConvert.DeserializeObject<RegisterModel>(command.ToString() ?? "");

        // todo valid
        if (string.IsNullOrWhiteSpace(obj?.Name) || string.IsNullOrWhiteSpace(obj?.Url))
            return BadRequest("Command name is required.");

        Serilog.Log.Information("Register command: {name} -> {url}", obj.Name, obj.Url);

        var current = ValueBox.CommandCollection.FirstOrDefault(item => item.Name.ToLower() == obj.Name.ToLower());
        if (current is null)
            ValueBox.CommandCollection.Add(new(obj.Name, obj.Url));
        else
            current.Url = obj.Url;
        return Ok("Command registered successfully.");
    }

    [HttpPut("register")]
    public IActionResult UpdateCommand([FromBody] (string CommandName, string CommandUrl) command)
    {
        var existingCommand = ValueBox.CommandCollection.FirstOrDefault(c => c.Name == command.CommandName);

        if (existingCommand == default)
        {
            return NotFound("Command not found.");
        }

        ValueBox.CommandCollection.TryTake(out existingCommand);
        ValueBox.CommandCollection.Add(new(command.CommandName, command.CommandUrl));

        return Ok("Command updated successfully.");
    }

    [HttpDelete("register/{commandName}")]
    public IActionResult DeleteCommand(string commandName)
    {
        var existingCommand = ValueBox.CommandCollection.FirstOrDefault(c => c.Name == commandName);

        if (existingCommand == default)
        {
            return NotFound("Command not found.");
        }

        ValueBox.CommandCollection.TryTake(out existingCommand);
        return Ok("Command deleted successfully.");
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
            ? match.Groups[2].Value.Split(new char[] { ' ', ',', '，', '、' }  // Linux必须这样写
                , StringSplitOptions.RemoveEmptyEntries)
            : null;

        Serilog.Log.Information("Send Command :\n{callback}", callback);

        // 授权
        var isPermission = await CheckPermissionAsync(callback);

        Serilog.Log.Information("Permission Status :\n{status}", isPermission);

        if (!isPermission) return;

        // 广播
        try
        {
            var command = ValueBox.CommandCollection
                .FirstOrDefault(item
                    => item.Name.ToLower() == callback.CommandName.ToLower());

            if (command is null) return;

            Serilog.Log.Information("Start a command: {command}", callback.ToString());

            await ClientUtils.ClientInstance.PostAsJsonAsync(command.Url, callback);

        }
        catch (Exception e)
        {
            Serilog.Log.Error(e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    private async Task<bool> CheckPermissionAsync(CallbackModel callback)
    {
        return true;
        var url = $"{ValueBox.LocalUrl}/permissions/check";

        var response = await ClientUtils.ClientInstance.PostAsJsonAsync(url, callback);
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
