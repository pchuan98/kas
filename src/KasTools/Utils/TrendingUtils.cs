using Chuan.Core;
using KasTools.Models;
using Newtonsoft.Json;

namespace KasTools.Utils;

public static class TrendingUtils
{
    public static async Task<Trending?> QueryAll()
    {
        var res = await ClientUtils.SafeGetString("https://api-v2-do.kas.fyi/token/krc20/trending");

        var obj = JsonConvert.DeserializeObject<Trending>(res);

        return obj;
    }

    public static async Task<IEnumerable<BaseTrendingObject>?> QueryMint()
    {
        var trending = await QueryAll();

        return trending?.MostMinted;
    }

    public static async Task<IEnumerable<BaseTrendingObject>?> QueryTransfer()
    {
        var trending = await QueryAll();

        return trending?.MostTransferred;
    }
}