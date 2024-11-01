using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chuan.Core;
using KasTools.Models;
using Newtonsoft.Json;

namespace KasTools.Utils;

public enum ChartInterval
{
    D1,
    D7,
    M1,
    Y1
}

public static class TickerUtils
{
    public static async Task<KasInfo?> QueryChart(string ticker, ChartInterval interval = ChartInterval.D1)
    {
        try
        {
            var t = interval switch
            {
                ChartInterval.D1 => "1d",
                ChartInterval.D7 => "7d",
                ChartInterval.M1 => "1m",
                ChartInterval.Y1 => "1y",
                _ => throw new Exception()
            };


            var res = await ClientUtils.SafeGetString(
                $"https://api-v2-do.kas.fyi/token/krc20/{ticker.ToUpper()}/info?includeCharts=true&interval={t}");

            var obj = JsonConvert.DeserializeObject<KasInfo>(res);
            return obj;
        }
        catch (Exception e)
        {
            Serilog.Log.Error($"{ticker.ToUpper()} -> " + e.Message);
        }

        return null;
    }

    public static async Task<KasInfo[]> QueryCharts(
        IEnumerable<string> tickers,
        ChartInterval interval = ChartInterval.D1,
        int threadCount = 8)
    {
        var names = tickers as string[] ?? tickers.ToArray();
        var hash = new ConcurrentDictionary<string, KasInfo>();

        var nameQueue = new BlockingCollection<string>();
        foreach (var name in names)
            nameQueue.Add(name);

        var tasks = new Task[threadCount];

        for (var i = 0; i < threadCount; i++)
            tasks[i] = Task.Run(async () =>
            {
                while (nameQueue.TryTake(out var name))
                {
                    if (string.IsNullOrEmpty(name)) break;

                    var chart = await QueryChart(name.ToUpper(), interval);

                    hash.TryAdd(name, chart ?? new KasInfo());
                }
            });

        await Task.WhenAll(tasks);

        return hash.Values.ToArray();

        //Parallel.For(0, names.Length, new ParallelOptions()
        //{
        //    MaxDegreeOfParallelism = threadCount
        //}, i =>
        //{
        //    var chart = QueryChart(names[i], interval).Result;

        //    infos[i] = chart ?? new KasInfo();
        //});

        //return infos;
    }
}
