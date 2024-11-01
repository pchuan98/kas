using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chuan.Core;
using KasTools;
using KasTools.Models;
using KasTools.Utils;
using Microsoft.Extensions.Configuration.EnvironmentVariables;

namespace Kas.Func.Mint;

public class TimeObject<T>
{
    /// <summary>
    /// 最后更新时间
    /// </summary>
    public DateTime LastUpdate { get; init; }

    /// <summary>
    /// 实体
    /// </summary>
    public T? Content { get; set; }
}

public class MintInfo
{
    /// <summary>
    /// Ticker名字
    /// </summary>
    public string Ticker { get; set; } = "";

    /// <summary>
    /// 当前mint百分比
    /// </summary>
    public double? Percent { get; set; }

    /// <summary>
    /// 2h 以内的增长趋势
    /// </summary>
    public double? Slope2H { get; set; }

    /// <summary>
    /// 24h增加mint数量
    /// </summary>
    public long Increase { get; set; }

    /// <summary>
    /// 时间差
    /// </summary>
    public TimeSpan Span { get; set; }

    /// <summary>
    /// 每分钟增长count
    /// </summary>
    public double AverageCount { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime DeployedAt { get; set; }

    /// <summary>
    /// 交叉持有
    /// </summary>
    public string[] CrossHolders { get; set; } = [];

    public override string ToString()
        => $"{Ticker.ToUpper(),-12} : {DeployedAt.ToHumanDateString()}\n" +
           $"{new string('-', 32)}\n" +
           $"{"Percent",-12} : {(Percent ?? 0) * 100:0.#####}%\n" +
           $"{"Slope2H",-12} : {Slope2H}\n" +
           $"{"Increase",-12} : {Increase} / {Span.ToHumanDateString()}\n" +
           $"{"Average(c/m)",-12} : {AverageCount:F2}\n" +
           $"{"Cross H",-12} : {(CrossHolders.Length == 0 ? "NONE" : string.Join(",", CrossHolders))}";
}

public static class MintManager
{
    public static async Task TrendingMint()
    {
        //var trending = await TrendingUtils.QueryAll();
        //if (trending?.MostTransferred is null
        //    || trending.MostMinted is null) return;

        //// top 20
        //var infos = GetKasInfos(trending.MostTransferred
        //    .Take(20)
        //    .Select(item => item.Ticker), 16);

        //// mints
        //var mints = trending.MostMinted;
    }

    public static async Task<MintInfo> ParseMint(
        string ticker,
        int topTickerCount = 20,
        int topTickerHolderCount = 20,
        Token[]? tokens = null,
        KasInfo? detail = null)
    {
        var result = new MintInfo();

        tokens ??= await TokenUtil.QueryAll();

        if (tokens is null) return result;

        // top count
        var topInfos = await GetKasInfos(tokens
                .Take(topTickerCount)
                .Select(item => item.Ticker),
            16);

        // info
        detail ??= await TickerUtils.QueryChart(ticker.ToUpper(), ChartInterval.D1);
        result.DeployedAt = detail!.DeployedAt;
        result.Ticker = ticker.ToUpper();
        result.Percent = detail.TotalMinted / detail.MaxSupply;

        var histories = detail
            .StatsHistories
            .OrderByDescending(item => item.Time.Ticks)
            .ToArray();

        var minutes = (histories[0].Time - histories[^1].Time).TotalMinutes;
        var increase = histories[0].MintCount - histories[^1].MintCount;

        result.Increase = increase;
        result.Span = histories[0].Time - histories[^1].Time;

        result.AverageCount = increase / minutes;

        var current = DateTime.Now;
        if (!result.DeployedAt.LessThan1Hour())
        {
            // time
            var time = histories
                .Select(item => (current - item.Time).TotalHours)
                .Where(item => item < 2)
                .ToArray();

            // values
            var vals = histories.Select(item =>
                (double)item.MintCount).ToArray()[..time.Length];
            for (var i = 0; i < vals.Length - 1; i++)
                vals[i] = vals[i] - vals[i + 1];
            vals[^1] = 0;

            var x = time.Reverse().ToArray();
            var y = vals.Reverse().Take(time.Length).Select(item => (double)item).ToArray();

            result.Slope2H = MathUtils.LinearRegression(x, y).sloope;
        }

        var holders = detail!.Holders
            ?.Select(item => item.Address)
            .Where(item => !string.IsNullOrEmpty(item));

        try
        {
            // cross valid
            if (holders is not null)
                result.CrossHolders = topInfos
                    .Where(item => item!.Holders
                        !.OrderByDescending(holder => holder.Amount)
                        .Take(topTickerHolderCount <= item!.Holders!.Count
                            ? topTickerHolderCount
                            : item!.Holders.Count)
                        !.Select(holder => holder.Address)
                        .Intersect(holders)
                        .Any())
                    .Select(item => item.Ticker)
                    .ToArray();
        }
        catch (Exception e)
        {
            Serilog.Log.Error(e.Message);
        }

        return result;
    }


    #region KasInfos

    /// <summary>
    /// 当前内容
    /// </summary>
    public static readonly ConcurrentDictionary<string, TimeObject<KasInfo>> Infos = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static async Task UpdateKasInfo(string name)
    {
        try
        {
            name = name.ToUpper();

            if (Infos.TryGetValue(name, out var value)
                  && value.LastUpdate.LessThan3Minutes()) return;

            var info = await TickerUtils.QueryChart(name, ChartInterval.Y1);

            if (info == null) return;

            Infos.TryRemove(name, out _);
            Infos.TryAdd(name, new TimeObject<KasInfo>()
            {
                LastUpdate = DateTime.Now,
                Content = info!
            });
        }
        catch (Exception e)
        {
            Serilog.Log.Error(e.Message);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="names"></param>
    /// <param name="threadCount"></param>
    /// <returns></returns>
    internal static async Task UpdateKasInfos(IEnumerable<string> names, int threadCount = 8)
    {
        var nameQueue = new BlockingCollection<string>();
        var array = names as string[] ?? names.ToArray();
        foreach (var name in array)
            nameQueue.Add(name.ToUpper());

        var tasks = new Task[threadCount];

        for (var i = 0; i < threadCount; i++)
            tasks[i] = Task.Run(async () =>
            {
                while (nameQueue.TryTake(out var name))
                {
                    if (string.IsNullOrEmpty(name)) break;

                    await UpdateKasInfo(name);
                }
            });

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="names"></param>
    /// <param name="threadCount"></param>
    /// <returns></returns>
    internal static async Task<KasInfo[]> GetKasInfos(IEnumerable<string?> names, int threadCount = 8)
    {
        var array = names as string[] ?? names.ToArray();

        await UpdateKasInfos(array!, threadCount);

        return array.Select(item =>
        {
            var rec = Infos.TryGetValue(item!.ToUpper(), out var value);
            return value?.Content ?? new KasInfo();
        }).ToArray();
    }

    #endregion
}