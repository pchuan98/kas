using Chuan.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace WeChat.Api.Shell.Controllers;

/// <summary>
/// 发送的二次封装接口
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class SendController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> SimpleSendValue([FromBody] SendModel body)
    {
        // default is string
        if (body.Receiver is null || body.Content is null) return Ok(false);

        Serilog.Log.Information("Send A Command Message:\n{msg}", body);

        var rect = await WeChatUtil.Instance.SendFriendStringMsg(body.Receiver, body.Content);
        return Ok(rect);
    }
}