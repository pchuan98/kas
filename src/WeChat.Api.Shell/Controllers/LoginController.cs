using Microsoft.AspNetCore.Mvc;

namespace WeChat.Api.Shell.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    [HttpGet]
    public string Test()
    {
        return "hello world";
    }

    [HttpGet("set/{content}")]
    public async Task<string?> ValueSet(string content)
    {

        return content;
    }

    [HttpGet("relogin")]
    public IActionResult ReLogin()
    {
        return Redirect("https://www.example.com");

    }

    [HttpGet("{info}")]
    public async Task<string?> Login(string info)
    {
        var str = await Task.Run(() =>
        {
            return info.ToLower() switch
            {
                "url" => "null",
                _ => ""
            };
        });

        return str;
    }
}
