using Chuan.Core;
using KasTools.Models;
using Newtonsoft.Json;

namespace KasTools.Utils;

public static class MarketUtils
{
    public static async Task<Market?> Query()
    {
        var url = "https://api-v2-do.kas.fyi/market";

        try
        {
            var res = await ClientUtils.SafeGetString(url);

            var obj = JsonConvert.DeserializeObject<Market>(res);
            return obj;
        }
        catch (Exception e)
        {
            Serilog.Log.Error(e.Message);
        }

        return null;
    }
}