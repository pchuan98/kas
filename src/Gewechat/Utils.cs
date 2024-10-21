using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Gewechat;

public static class Utils
{
    /// <summary>
    /// 
    /// </summary>
    private static readonly string BaseUrl = "http://100.77.255.127:2531/v2/api";

    /// <summary>
    /// 
    /// </summary>
    private static readonly string Token = "";

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static HttpClient GetHttpClient()
    {
        var handler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };

        var client = new HttpClient(handler)
        {
            Timeout = TimeSpan.FromSeconds(60)
        };

        return client;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="route"></param>
    /// <param name="param"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<JObject> PostJsonAsync(string route, JObject param)
    {
        var headers = new Dictionary<string, string>();

        if (!string.IsNullOrEmpty(Token))
        {
            headers.Add("X-GEWE-TOKEN", Token);
        }

        try
        {
            if (string.IsNullOrEmpty(BaseUrl))
                throw new Exception("baseUrl 未配置");

            var response = await PostJsonInternalAsync(BaseUrl + route, headers, param.ToString(), GetHttpClient());

            Console.WriteLine(response);

            var jsonResponse = JObject.Parse(response);
            if ((int)(jsonResponse["ret"] ?? -1) == 200)
                return jsonResponse;

            throw new Exception(response);
        }
        catch (Exception ex)
        {
            Console.WriteLine("url=" + BaseUrl + route);
            throw new Exception(ex.Message);
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="url"></param>
    /// <param name="headers"></param>
    /// <param name="jsonContent"></param>
    /// <param name="client"></param>
    /// <returns></returns>
    private static async Task<string> PostJsonInternalAsync(string url, Dictionary<string, string> headers, string jsonContent, HttpClient client)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url);

        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }
}
