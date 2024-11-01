using Chuan.Core;
using KasTools.Models;
using Newtonsoft.Json;
using Serilog;

namespace KasTools.Utils;

file class TokensObject
{
    [JsonProperty("results")]
    public Token[]? Data { get; set; }
}

public static class TokenUtil
{
    public static async Task<Token[]?> QueryAll()
    {
        try
        {
            var res = await ClientUtils.SafeGetString("https://api-v2-do.kas.fyi/token/krc20/tokens");

            if (string.IsNullOrEmpty(res))
            {
                Log.Error("TokenUtil QueryAll Error.");
                return null;
            }

            var obj = JsonConvert.DeserializeObject<TokensObject>(res);

            return obj?.Data;
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }

        return null;
    }
}