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

        var sender = request.IsGroup ? request.Group : request.Sender;

        LastArgs.TryGetValue(sender, out var last);
        var args = (request.Args ?? last)?.ToHashSet();

        if (!LastArgs.Keys.Contains(sender))
            LastArgs.TryAdd(sender, args?.ToArray());
        else
            LastArgs[sender] = args?.ToArray();

        if (args is null)
        {
            await request.SendMessage("请输入正确的Ticker Name");
            return;
        }

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

        var msg = $"[TIME] {time:MM-dd HH:mm:ss}\n\n";
        foreach (var name in args)
        {
            var token = tokens?.FirstOrDefault(token => token.Ticker?.ToUpper().Trim() == $"{name.ToUpper().Trim()}");
            if (token is null) continue;

            msg += $"[烟花] {name.Trim(),-12} : {token?.Price?.FloorPrice:F8} KAS\n";
        }

        await request.SendMessage(msg);
    }
}