using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Serilog;

namespace Chuan.Core;

public static class ClientUtils
{
    private static HttpClient? _client;

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
                Timeout = TimeSpan.FromSeconds(20)
            };

            _client.DefaultRequestVersion = HttpVersion.Version20;

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

    public static async Task<string> SafeGetString(string url, Dictionary<string, string>? headers = null)
    {
        var content = "";
        var index = 10;

        if (headers is not null)
        {
            ClientInstance.DefaultRequestHeaders.Clear();

            ClientInstance.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            ClientInstance.DefaultRequestHeaders.Connection.Add("keep-alive");

            foreach (var key in headers.Keys)
                ClientInstance.DefaultRequestHeaders.Add(key, headers[key]);
        }

        Log.Verbose(ClientInstance.DefaultRequestHeaders.Aggregate("",
            (current, header) =>
                current + $"\n{header.Key}: {string.Join(", ", header.Value)}"));

        Log.Verbose("Get String Url: {url}", url);

        while (index-- != 0)
        {
            try
            {
                var response = await ClientInstance.GetAsync(url);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();

                Log.Warning(response.ToString());
                await Task.Delay(1000);
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        return content;
    }

    public static async Task<string?> SafeGetStringOnce(string url, Dictionary<string, string>? headers = null)
    {
        if (headers is not null)
        {
            ClientInstance.DefaultRequestHeaders.Clear();

            ClientInstance.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            ClientInstance.DefaultRequestHeaders.Connection.Add("keep-alive");

            foreach (var key in headers.Keys)
                ClientInstance.DefaultRequestHeaders.Add(key, headers[key]);
        }

        Log.Verbose(ClientInstance.DefaultRequestHeaders.Aggregate("",
            (current, header) =>
                current + $"\n{header.Key}: {string.Join(", ", header.Value)}"));

        Log.Verbose("Get String Url: {url}", url);

        try
        {
            var response = await ClientInstance.GetAsync(url);

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();

            Log.Warning(response.ToString());
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return null;
        }
        return null;
    }
}