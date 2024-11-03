using System.Security.Cryptography;
using Chuan.Core;
using KasTools.Models;
using Newtonsoft.Json;
using Serilog;
using System.Text;

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
        var index = 10;

        while (index-- >= 0)
        {
            try
            {
                var key = await GenerateHashAsync();
                var res = await ClientUtils.SafeGetStringOnce(
                    "https://api-v2-do.kas.fyi/token/krc20/tokens",
                    new Dictionary<string, string>()
                    {
                        {"x-api-key",key}
                    });

                if (string.IsNullOrEmpty(res))
                    continue;
                
                var obj = JsonConvert.DeserializeObject<TokensObject>(res);

                return obj?.Data;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        return null;
    }

    static async Task<string> GenerateHashAsync()
    {
        var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        var uuid = "8305b25a-dbbd-426d-8e21-9833b91e35d1";
        var input = $"{currentTime}:{uuid}";

        var hash = await Task.Run(() => ComputeSha256Hash(input));
        return $"{currentTime}:{hash}";
    }

    static string ComputeSha256Hash(string rawData)
    {
        using SHA256 sha256Hash = SHA256.Create();

        var bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // 转换字节数组为十六进制字符串
        var builder = new StringBuilder();
        foreach (var b in bytes)
            builder.Append(b.ToString("x2")); // x2 代表用两位十六进制格式表示

        return builder.ToString();
    }
}