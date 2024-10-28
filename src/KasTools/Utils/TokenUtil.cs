using Chuan.Core;
using KasTools.Models;
using Newtonsoft.Json;

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
        var res = await ClientUtils.ClientInstance
            .GetStringAsync("https://api-v2-do.kas.fyi/token/krc20/tokens");

        var obj = JsonConvert.DeserializeObject<TokensObject>(res);

        return obj?.Data;
    }
}