using Chuan.Core.Models;
using Microsoft.AspNetCore.Mvc;
using WeChat.Api.Shell.Models;

namespace WeChat.Api.Shell.Controllers;

/// <summary>
/// 权限管理
///
/// 这里的权限只是一级权限，即是否有进一步广播到其他处理程序中的权限
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PermissionsController : ControllerBase
{
    /// <summary>
    /// 
    /// </summary>
    public const string HasPermissionString = "{\"HasPermission\":true}";

    /// <summary>
    /// 
    /// </summary>
    public const string NoPermissionString = "{\"HasPermission\":false}";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost("check")]
    public async Task<IActionResult> CheckPermission([FromBody] CallbackModel request)
    {
        // 如果是我的账号，无脑通过
        if (request.Sender == "wxid_xwmbszzoarry21")
            return Ok(HasPermissionString);

        return Ok(HasPermissionString);

        var level = await GetPermissionLevel(request);

        // admin权限最高，可以在任何时候广播
        if (level == PermissionLevel.Admin) 
            return Ok(HasPermissionString);
        // 判断是不是有权限的群，如果不是不再广播
        if (level == PermissionLevel.GroupAdmin)
        {

        }

        // 判断用户是否有权限，没有没有也不广播
        return Ok(HasPermissionString);
    }

    /// <summary>
    /// todo 判断发送人是不是管理员
    /// </summary>
    /// <returns></returns>
    public async Task<bool> IsAdmin(CallbackModel model)
    {
        await Task.Delay(10);
        return true;
    }

    /// <summary>
    /// 判断这个群有没有命令权限
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<bool> IsPermissionGroup(CallbackModel model)
    {
        await Task.Delay(10);
        return true;
    }

    /// <summary>
    /// 判断这个用户有没有命令权限
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    public async Task<bool> IsPermissionUser(CallbackModel model)
    {
        await Task.Delay(10);
        return true;
    }

    public async Task<PermissionLevel> GetPermissionLevel(CallbackModel model)
    {
        await Task.Delay(10);
        return PermissionLevel.GroupAdmin;
    }
}
