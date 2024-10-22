using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Gewechat.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gewechat;

public static class Utils
{
    /// <summary>
    /// 
    /// </summary>
    private static readonly string BaseUrl = "http://127.0.0.1:2531/v2/api";

    /// <summary>
    /// todo 这里改成线程池一样的东西，或者干脆就用一个静态的工具
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
            Timeout = TimeSpan.FromSeconds(60),
        };

        return client;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="route"></param>
    /// <param name="headers"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    public static async Task<string> PostRequestAsync(string route, Dictionary<string, string>? headers = null, string? body = null)
    {
        var client = GetHttpClient();

        var url = BaseUrl + route;

        var request = new HttpRequestMessage(HttpMethod.Post, url);

        if (headers is not null)
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    public static async Task<string> PostRequest2Async(string url, Dictionary<string, string>? headers = null, string? body = null)
    {
        var client = GetHttpClient();

        var request = new HttpRequestMessage(HttpMethod.Post, url);

        if (headers is not null)
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);


        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }


    ///// <summary>
    ///// 
    ///// </summary>
    ///// <param name="route"></param>
    ///// <param name="param"></param>
    ///// <returns></returns>
    ///// <exception cref="Exception"></exception>
    //public static async Task<T> PostJsonAsync<T>(string route, JObject param)
    //{
    //    var headers = new Dictionary<string, string>()
    //    {

    //    };

    //    // append the token when 
    //    if (!string.IsNullOrEmpty(Token))
    //        headers.Add("X-GEWE-TOKEN", Token);

    //    try
    //    {
    //        if (string.IsNullOrEmpty(BaseUrl))
    //            throw new Exception("baseUrl 未配置");

    //        var response = await PostJsonInternalAsync(
    //            BaseUrl + route,
    //            headers,
    //            param.ToString(),
    //            GetHttpClient());

    //        return JsonConvert.DeserializeObject<T>(response)!;
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine("url=" + BaseUrl + route);
    //        throw new Exception(ex.Message);
    //    }

    //}

    //public static async Task<JObject> PostJsonAsync(string route, JObject param)
    //{
    //    var headers = new Dictionary<string, string>();

    //    if (!string.IsNullOrEmpty(Token))
    //    {
    //        headers.Add("X-GEWE-TOKEN", Token);
    //    }

    //    try
    //    {
    //        if (string.IsNullOrEmpty(BaseUrl))
    //            throw new Exception("baseUrl 未配置");

    //        var response = await PostJsonInternalAsync(BaseUrl + route, headers, param.ToString(), GetHttpClient());

    //        Console.WriteLine(response);

    //        var jsonResponse = JObject.Parse(response);
    //        if ((int)(jsonResponse["ret"] ?? -1) == 200)
    //            return jsonResponse;

    //        throw new Exception(response);
    //    }
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine("url=" + BaseUrl + route);
    //        throw new Exception(ex.Message);
    //    }
    //}

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
