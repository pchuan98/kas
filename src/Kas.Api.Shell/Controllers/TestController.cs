using Chuan.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kas.Api.Shell.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TestController : ControllerBase
{
    [HttpPost]
    public async Task Test([FromBody] CallbackModel request)
    {
        if (request.Sender != "wxid_xwmbszzoarry21") return;

        await WeChatApiUtils.SendMessage(request.IsGroup ? request.Group : request.Sender, request.ToString());
    }
}