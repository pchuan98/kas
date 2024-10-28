using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Chuan.Core;

public static class ClientUtils
{
    private static HttpClient? _client = new HttpClient();

    public static HttpClient ClientInstance
    {
        get
        {
            if (_client is not null) return _client;

            _client = new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            })
            {
                Timeout = TimeSpan.FromSeconds(60) // todo 多次获取结果
            };

            _client.DefaultRequestVersion = HttpVersion.Version20;
            _client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _client.DefaultRequestHeaders.Connection.Add("keep-alive");

            return _client;
        }
    }

    public static async Task<string?> Post(string url, object content)
    {
        var response = await ClientInstance.PostAsJsonAsync(url, content);

        if (!response.IsSuccessStatusCode) return null;

        var result = await response.Content.ReadAsStringAsync();
        return result;

    }
}