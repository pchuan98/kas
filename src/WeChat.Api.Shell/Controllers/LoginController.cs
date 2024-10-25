using Microsoft.AspNetCore.Mvc;

namespace WeChat.Api.Shell.Controllers;

// todo  这里之后写一些静态网页，现在写不管了

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    /// <summary>
    /// 入口，包含所有login的get方法
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public IActionResult BaseContent()
        => Content("""
                   <!DOCTYPE html>
                   <html lang="zh-CN">
                   <head>
                       <meta charset="UTF-8">
                       <meta name="viewport" content="width=device-width, initial-scale=1.0">
                       <title>WeChat Login</title>
                       <style>
                           ol {
                                  font-size: 48px; /* 设置整个有序列表的字体大小 */
                                  list-style-position: inside; /* 让数字在列表项内 */
                              }
                              li {
                                  color: blue; /* 设置列表项的字体颜色 */
                              }
                              li:hover {
                                  color: red; /* 设置鼠标悬停时的字体颜色 */
                              }
                           a {
                               font-size: 48px;
                               color: blue;
                               text-decoration: none;
                           }
                           a:hover {
                               color: red;
                           }
                           
                       </style>
                   </head>
                   <body>
                       <ol>
                       <li><a href="https://www.example.com">点击跳转到Example网站</a></li>
                       </ol>
                   </body>
                   </html>
                   """, "text/html");

    /// <summary>
    /// 
    /// </summary>
    /// <param name="content"></param>
    /// <returns></returns>
    [HttpGet("get/{content}")]
    public async Task<string?> GetValue(string content)
    {

        return content;
    }

    [HttpPost("set")]
    public async Task<string?> SetValue(string info)
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

    [HttpGet("qr")]
    public async Task<IActionResult> QrLogin()
    {
        await WeChatUtil.Login();

        return Ok();
    }

    [HttpGet("relogin")]
    public IActionResult ReLogin()
    {
        return Redirect("https://www.example.com");

    }

   
}
