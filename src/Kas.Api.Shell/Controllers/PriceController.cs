using Chuan.Core;
using Chuan.Core.Models;
using Microsoft.AspNetCore.Mvc;
using PChuan.Core;

namespace Kas.Api.Shell.Controllers;

/// <summary>
/// 统计价格相关
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class PriceController : ControllerBase
{
    /// <summary>
    /// 持久化技术
    /// </summary>
    internal static readonly Dictionary<string, string[]?> LastArgs = new();

    /// <summary>
    /// 验证函数
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<bool> CheckPermission(CallbackModel request)
    {
        await Task.Delay(10);

        // todo 添加其他的东西
        return true;
        return request.IsSuperAdmin();
    }

    /// <summary>
    /// 处理函数
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task Price([FromBody] CallbackModel request)
    {
        var permission = await CheckPermission(request);
        if (!permission) return;

        var receiver = request.Receiver;

        // args有，last=args，args没有，看last有么，没有返回错误
        LastArgs.TryGetValue(receiver, out var last);
        var args = request.Args?.ToHashSet();

        // args x && last x
        if ((args is null || args.Count == 0)
            && (last is null || last.Length == 0))
        {
            await request.SendMessage("请输入正确的Ticker Name");
            return;
        }

        if (args?.Count > 0) // args ok
        {
            // 设置last
            if (!LastArgs.Keys.Contains(receiver))
                LastArgs.TryAdd(receiver, args?.ToArray());
            else
                LastArgs[receiver] = args?.ToArray();
        }
        else args = last!.ToHashSet();


        if (ValueBox.AliveKasInfo.Tokens.Data is null)
            await ValueBox.AliveKasInfo.UpdateTokens();

        if (ValueBox.AliveKasInfo.Tokens.Data is null)
        {
            Serilog.Log.Error("AliveKasInfo Get Tokens Error.");
            return;
        }

        if (!ValueBox.AliveKasInfo.Tokens.Time.LessThanHalfHour())
            await ValueBox.AliveKasInfo.UpdateTokens();

        var tokens = ValueBox.AliveKasInfo.Tokens.Data;
        var time = ValueBox.AliveKasInfo.Tokens.Time;

        var msg = $"[TIME] {DateTime.Now:MM-dd HH:mm:ss}\n\n";
        foreach (var name in args!)
        {
            var token = tokens?.FirstOrDefault(token => token.Ticker?.ToUpper().Trim() == $"{name.ToUpper().Trim()}");
            if (token is null) continue;

            var price = token.PriceHistories?.OrderByDescending(item => item.Time)?.FirstOrDefault()?.Price;

            msg += $"[烟花] {name.ToUpper().Trim(),-10} : {price:F8} KAS\n";
        }

        await request.SendMessage(msg);
    }
}