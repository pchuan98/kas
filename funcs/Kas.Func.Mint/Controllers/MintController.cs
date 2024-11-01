using Chuan.Core.Models;
using KasTools.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kas.Func.Mint.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MintController : ControllerBase
{
    [HttpGet]
    public IActionResult Main()
    {
        var mints = MintUtil.QueryAllMint();
        

        return Ok();
    }

    /// <summary>
    /// 处理函数
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task MintStart([FromBody] CallbackModel request)
    {
        // todo 开启自动mint追踪功能
    }

}