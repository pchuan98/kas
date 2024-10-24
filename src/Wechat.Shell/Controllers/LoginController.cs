using Microsoft.AspNetCore.Mvc;

namespace Wechat.Shell.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    [HttpGet]
    public void GetLoginStatus()
    {
        // todo login relogin logout online


    }
}