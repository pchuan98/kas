using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WeChat.Api.Shell.Models;

namespace WeChat.Api.Shell.Controllers;

// todo  这里之后写一些静态网页，现在写不管了

/// <summary>
/// 登录设置
///
/// 1、配置文件相关的：文件的查看、重新生成、修改
/// 2、微信登录相关：获取token
/// </summary>
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
    {
        var routes = new List<string>
        {
            $"{Request.Scheme}://{Request.Host}/api/login/file - 查看配置文件",
            $"{Request.Scheme}://{Request.Host}/api/login/file/set - 设置/登录微信参数",
            $"{Request.Scheme}://{Request.Host}/api/login/isonline - 检查在线状态",
            $"{Request.Scheme}://{Request.Host}/api/login/callback - 设置当前回调URL"
        };

        var htmlContent = $@"
            <!DOCTYPE html>
            <html lang=""zh-CN"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>API Routes</title>
                <style>
                    ol {{ font-size: 24px; list-style-position: inside; }}
                    li {{ color: blue; }}
                    li:hover {{ color: red; }}
                    a {{ font-size: 24px; color: blue; text-decoration: none; }}
                    a:hover {{ color: red; }}
                </style>
            </head>
            <body>
                <h2>登录控制器API导航</h2>
                <ol>
                    {string.Join("", routes.Select(route => $"<li><a href=\"{route.Split(' ')[0]}\">{route}</a></li>"))}
                </ol>
            </body>
            </html>";

        return Content(htmlContent, "text/html");
    }

    [HttpGet("file")]
    public async Task<IActionResult> GetFile() =>
        !System.IO.File.Exists(ValueBox.WE_CHAT_CONFIG_PATH)
            ? Ok("File not exist.")
            : Ok(await System.IO.File.ReadAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH));

    [HttpGet("file/reset")]
    public async Task<IActionResult> ResetFile()
    {
        var info = JsonConvert.SerializeObject(new WeChatInfo(), Formatting.Indented);
        await System.IO.File.WriteAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH, info);

        return Ok("New Create File");
    }

    [HttpGet("file/set")]
    public async Task<IActionResult> SettingsPage()
    {
        var weChatInfo = new WeChatInfo();
        if (System.IO.File.Exists(ValueBox.WE_CHAT_CONFIG_PATH))
        {
            var configContent = await System.IO.File.ReadAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH);
            weChatInfo = JsonConvert.DeserializeObject<WeChatInfo>(configContent) ?? new WeChatInfo();
        }

        var htmlContent = $@"
            <!DOCTYPE html>
            <html lang=""zh-CN"">
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>Settings</title>
            <style>
                    label, input, button {{ font-size: 24px; margin: 10px 0; display: block; }}
                    input[type='text'] {{ width: 100%; padding: 8px; font-size: 18px; }}
                    button {{ padding: 8px 12px; }}
                    #qr-image {{ display: none; margin-top: 20px; }}
                    #qr-text {{ font-family: monospace; font-size: 18px; line-height: 1em; 
                                background-color: #f9f9f9; padding: 10px; border: 1px solid #ddd; }}
                </style>
            </head>
            <body>
                <h2>设置 URL、Token、AppId 和 UUID</h2>
                
                <label for=""url"">URL:</label>
                <input type=""text"" id=""url"" value=""{weChatInfo.Url ?? ""}"" placeholder=""请输入URL"">
                <button onclick=""submitSingleSetting('url')"">提交URL</button>

                <label for=""token"">Token:</label>
                <input type=""text"" id=""token"" value=""{weChatInfo.Token ?? ""}"" placeholder=""点击获取Token"">
                <button onclick=""fetchToken()"">获取Token</button>
                <button onclick=""submitSingleSetting('token')"">提交Token</button>

                <label for=""appid"">AppId:</label>
                <input type=""text"" id=""appid"" value=""{weChatInfo.AppId ?? ""}"" placeholder=""请输入AppId"">
                <button onclick=""submitSingleSetting('appid')"">提交AppId</button>

                <label for=""uuid"">UUID:</label>
                <input type=""text"" id=""uuid"" value=""{weChatInfo.Uuid ?? ""}"" placeholder=""请输入UUID"">
                <button onclick=""submitSingleSetting('uuid')"">提交UUID</button>

                <button onclick=""syncInfo()"">同步</button>

                <button onclick=""fetchQR()"">获取QR</button>
                
                <img id=""qr-image"" alt=""QR Code"" />

                <script>
                    async function fetchToken() {{
                        try {{
                            let response = await fetch('/api/login/gettoken');
                            let data = await response.text();
                            document.getElementById('token').value = data;
                        }} catch (error) {{
                            alert('获取Token失败: ' + error);
                        }}
                    }}

                    async function fetchQR() {{
                        try {{
                            let response = await fetch('/api/login/qr64');
                            let base64Data = await response.text();
                            const qrImage = document.getElementById('qr-image');
                            qrImage.src = base64Data;  // Assuming it's a PNG image
                            qrImage.style.display = 'block';  // Show the image
                        }} catch (error) {{
                            alert('获取QR失败: ' + error);
                        }}
                    }}

                    async function syncInfo() {{
                        try {{
                            let response = await fetch('/api/login/sync');
                            let data = await response.json();
                            // Update the input fields with the received data
                            document.getElementById('url').value = data.url;
                            document.getElementById('token').value = data.token;
                            document.getElementById('appid').value = data.appid;
                            document.getElementById('uuid').value = data.uuid;
                        }} catch (error) {{
                            alert('同步失败: ' + error);
                        }} 
                    }}

                    async function submitSingleSetting(field) {{
                        const value = document.getElementById(field).value;
                        const setting = {{ field: field, value: value }};
                        
                        try {{
                            let response = await fetch(`/api/login/setSingleSetting`, {{
                                method: 'POST',
                                headers: {{ 'Content-Type': 'application/json' }},
                                body: JSON.stringify(setting)
                            }});
                            alert(await response.text());
                        }} catch (error) {{
                            alert(`提交${{field}}失败: ` + error);
                        }}
                    }}
                </script>
            </body>
            </html>";

        return Content(htmlContent, "text/html");
    }

    [HttpPost("setSingleSetting")]
    public async Task<IActionResult> SetSingleSetting([FromBody] Dictionary<string, string> setting)
    {
        if (setting == null || !setting.ContainsKey("field") || !setting.ContainsKey("value"))
            return BadRequest("无效的设置请求。");

        var weChatInfo = new WeChatInfo();
        if (System.IO.File.Exists(ValueBox.WE_CHAT_CONFIG_PATH))
        {
            var configContent = await System.IO.File.ReadAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH);
            weChatInfo = JsonConvert.DeserializeObject<WeChatInfo>(configContent) ?? new WeChatInfo();
        }

        var field = setting["field"];
        var value = setting["value"];
        switch (field.ToLower())
        {
            case "url":
                weChatInfo.Url = value;
                break;
            case "token":
                weChatInfo.Token = value;
                break;
            case "appid":
                weChatInfo.AppId = value;
                break;
            case "uuid":
                weChatInfo.Uuid = value;
                break;
            default:
                return BadRequest("未知的设置字段。");
        }

        await System.IO.File.WriteAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH, weChatInfo.ToString());
        return Ok($"{field} 已更新为：{value}");
    }


    /// <summary>
    /// get old / new wechat token
    /// </summary>
    /// <returns></returns>
    [HttpGet("gettoken")]
    public async Task<IActionResult> GetToken()
    {
        var configContent = await System.IO.File.ReadAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH);
        var info = JsonConvert.DeserializeObject<WeChatInfo>(configContent) ?? new WeChatInfo();

        WeChatUtil.SetWeChat(new Gewechat.WeChat(info.Url ?? ""));

        var token = await WeChatUtil.Instance.RequireToken();
        return Ok(token);
    }

    [HttpGet("sync")]
    public IActionResult SyncWechatInfo()
    {
        return Ok((new WeChatInfo()
        {
            Url = WeChatUtil.Instance.ApiUrl,
            Token = WeChatUtil.Instance.Token,
            AppId = WeChatUtil.Instance.AppId,
            Uuid = WeChatUtil.Instance.Uuid
        }).ToString());
    }


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
        var configContent = await System.IO.File.ReadAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH);
        var info = JsonConvert.DeserializeObject<WeChatInfo>(configContent) ?? new WeChatInfo();

        WeChatUtil.SetWeChat(new Gewechat.WeChat(info.Url ?? "", info.Token));

        var qr = await WeChatUtil.Instance.RequireLoginQr();

        return Ok(qr);
    }

    [HttpGet("qr64")]
    public async Task<IActionResult> QrLogin64()
    {
        var configContent = await System.IO.File.ReadAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH);
        var info = JsonConvert.DeserializeObject<WeChatInfo>(configContent) ?? new WeChatInfo();

        WeChatUtil.SetWeChat(new Gewechat.WeChat(info.Url ?? "", info.Token));

        var qr = await WeChatUtil.Instance.RequireLoginQrBases64();

        return Ok(qr);
    }

    [HttpGet("reset")]
    public async Task<IActionResult> ResetLogin()
    {
        await WeChatUtil.SetWeChatWithFile();
        return Ok();
    }

    [HttpGet("isonline")]
    public IActionResult CheckOnline()
    {
        return Ok(WeChatUtil.Instance.IsOnline);
    }

    [HttpGet("callback")]
    public async Task<IActionResult> SetCallback()
    {
#if DEBUG
        return Ok(await WeChatUtil.Instance.SetCallbackUrl("http://100.77.255.127:5099/api/login/callback"));
#endif

        var url = $"{Request.Scheme}://{Request.Host}/api/callback";
        var result = await WeChatUtil.Instance.SetCallbackUrl(url);

        return Ok(result);
    }
}
