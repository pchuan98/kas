using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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


            var res = await GlobalUtils.Client
                .GetStringAsync(
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
}
