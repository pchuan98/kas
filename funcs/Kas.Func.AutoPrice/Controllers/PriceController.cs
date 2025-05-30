﻿using Chuan.Core;
using Chuan.Core.Models;
using KasTools.Models;
using KasTools.Utils;
using Microsoft.AspNetCore.Mvc;
using PChuan.Core;

namespace Kas.Func.AutoPrice.Controllers;

/// <summary>
/// 自动拉取 + 后期查找的kas信息
/// </summary>
public class AliveInfo
{
    /// <summary>
    /// 
    /// </summary>
    public (DateTime Time, Token[]? Data) Tokens { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task UpdateTokens()
        => Tokens = new(DateTime.Now, await TokenUtil.QueryAll());
}

[Route("api/[controller]")]
[ApiController]
public class PriceController : ControllerBase
{
    /// <summary>
    /// 持久化技术
    /// </summary>
    internal static readonly Dictionary<string, string[]?> LastArgs = new();

    /// <summary>
    /// 查找信息持久化
    /// </summary>
    public static readonly AliveInfo AliveKasInfo = new();

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
        Serilog.Log.Verbose("Load request:\n{r}", request);

        var permission = await CheckPermission(request);
        if (!permission) return;

        var receiver = request.Receiver;

        // args有，last=args，args没有，看last有么，没有返回错误

        LastArgs.TryGetValue(receiver, out var last);
        var args = request.Args?.Select(item => item.ToUpper())?.ToHashSet();

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

        if (AliveKasInfo.Tokens.Data is null
            || !AliveKasInfo.Tokens.Time.LessThan3Minutes())
            await AliveKasInfo.UpdateTokens();



        if (AliveKasInfo.Tokens.Data is null)
        {
            Serilog.Log.Error("AliveKasInfo Get Tokens Error.");
            return;
        }

        var tokens = AliveKasInfo.Tokens.Data;

        var msg = $"[TIME] {AliveKasInfo.Tokens.Time:MM-dd HH:mm:ss}\n\n";
        foreach (var name in args!)
        {
            var token = tokens?.FirstOrDefault(token => token.Ticker?.ToUpper().Trim() == $"{name.ToUpper().Trim()}");
            if (token is null) continue;

            msg += $"[烟花] {name.ToUpper().Trim(),-10} : {token?.Price?.FloorPrice:F8} KAS\n";
        }

        await request.SendMessage(msg);
    }
}
