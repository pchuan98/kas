using Chuan.Core;
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

        //if (AliveKasInfo.Tokens.Data is null
        //    || !AliveKasInfo.Tokens.Time.LessThan3Minutes())
        //    await AliveKasInfo.UpdateTokens();

        //if (AliveKasInfo.Tokens.Data is null)
        //{
        //    Serilog.Log.Error("AliveKasInfo Get Tokens Error.");
        //    return;
        //}

        //var tokens = AliveKasInfo.Tokens.Data;

        var msg = $"[TIME] {DateTime.Now:MM-dd HH:mm:ss}\n\n";

        var charts = await TickerUtils.QueryCharts(args!);

        //var marketInfo = await MarketUtils.Query();

        if (!System.IO.File.Exists("TokensIds"))
            await System.IO.File.WriteAllTextAsync("TokensIds", "");

        var oks = (await System.IO.File.ReadAllLinesAsync("TokensIds"))
            .SkipWhile(string.IsNullOrEmpty)
            .Where(item => item.Contains(","))
            .Select(item => (item.Split([','])[0], double.Parse(item.Split([','])[1])))
            .ToHashSet();

        var oksStr = oks.Select(item => item.Item1.ToUpper()).ToHashSet();
        var argsStr = args.Select(item => item.ToUpper());

        var diff = argsStr.Except(oksStr).ToList();
        if (diff.Any())
        {
            await request.SendMessage("有新数据待加载，请稍等");
            var result = await TransferUtils.QueryPerMintCounts(diff);

            foreach (var item in result)
                await System.IO.File.AppendAllTextAsync("TokensIds", $"{item.Key.ToUpper()},{item.Value}\n");
        }

        oks = (await System.IO.File.ReadAllLinesAsync("TokensIds"))
            .SkipWhile(string.IsNullOrEmpty)
            .Where(item => item.Contains(","))
            .Select(item => (item.Split([','])[0], double.Parse(item.Split([','])[1])))
            .ToHashSet();

        foreach (var arg in args!)
        {
            try
            {
                var chart = charts.FirstOrDefault(item => string.Equals(arg.Trim(), item.Ticker, StringComparison.OrdinalIgnoreCase));

                if (chart is null) continue;

                var count = oks.FirstOrDefault(item => item.Item1 == arg.ToUpper()).Item2;
                var mintPrice = 1.1 / count;

                var price = chart?.PriceHistories?[^1]?.Price;

                if (price is null) continue;

                var ticker = chart?.Ticker?.ToUpper()?.Trim();

                msg += mintPrice > 0.00000000000001
                    ? $"[烟花] {ticker,-10} ({price / mintPrice:F2}) : {price:F8} KAS\n"
                    : $"[烟花] {ticker,-10} : {price:F8} KAS\n";

            }
            catch (Exception e)
            {
                Serilog.Log.Error(e.Message);
            }
        }

        await request.SendMessage(msg);
    }
}
