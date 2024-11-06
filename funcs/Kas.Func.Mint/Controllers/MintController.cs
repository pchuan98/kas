using Chuan.Core;
using Chuan.Core.Models;
using KasTools.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Kas.Func.Mint.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MintController : ControllerBase
{
    [HttpGet]
    public IActionResult Main()
    {
        //var mints = MintUtil.QueryAllMint();


        return Ok("okk");
    }

    private static string ?_mintString = null;

    private static DateTime _mintTime = DateTime.Now;

    /// <summary>
    /// 处理函数
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task MintStart([FromBody] CallbackModel request)
    {
        if (_mintString is not null && (DateTime.Now - _mintTime) < TimeSpan.FromMinutes(30))
        {
            await request.SendMessage(_mintString);

            return;
        }

        // trending 
        var trending = await TrendingUtils.QueryAll();
        var trendingTickers = trending
            ?.MostMinted
            ?.Select(item => item.Ticker!)
            ?.ToHashSet();

        // tokens
        var tokens = await TokenUtil.QueryAll();
        var tokensTickers = tokens
            ?.Where(item => item.Status?.ToLower() == "deployed")
            .Where(item => (DateTime.Now - item.DeployedAt) < TimeSpan.FromHours(3))
            .Select(item => item.Ticker!)
            .ToHashSet();

        trendingTickers ??= new HashSet<string>();
        var tickers = new HashSet<string>(tokensTickers
            ?.Union(trendingTickers) ?? Array.Empty<string>());

        var infos = new List<string>();

        foreach (var ticker in tickers)
        {
            try
            {
                var info = await MintManager.ParseMint(ticker, tokens: tokens);

                infos.Add(info.ToString());
                Log.Debug(info.ToString());
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        var msg = string.Join("\n=======================\n", infos);
        _mintString = msg;
        await request.SendMessage(msg);
    }

}

public static class WeChatApiUtils
{
    public static string WeChatApiBaseUrl = "http://122.152.227.199:5099/api";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="wxid"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static async Task<string?> SendMessage(string wxid, string content)
    {
        var result = await ClientUtils.Post($"{WeChatApiBaseUrl}/send", new SendModel()
        {
            Receiver = wxid,
            Content = content
        });

        return result;
    }

    /// <summary>
    /// 快速发消息
    /// </summary>
    /// <param name="model"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public static async Task<string?> SendMessage(this CallbackModel model, string content)
        => await SendMessage(model.IsGroup ? model.Group : model.Sender, content);

}