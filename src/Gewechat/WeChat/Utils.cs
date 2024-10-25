using System.Net.Http.Headers;
using System.Reflection.PortableExecutable;
using System.Text;
using Chuan.Core;
using Newtonsoft.Json;
using Serilog.Debugging;

namespace Gewechat;

public partial class WeChat
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="headers"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    internal async Task<string> PostJsonRequestAsync(
        string controller,
        Dictionary<string, string>? headers = null,
        string? body = null)
    {
        var url = ApiUrl + controller;

        var request = new HttpRequestMessage(HttpMethod.Post, url);

        if (headers is not null)
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");

        Serilog.Log.Verbose("Post Json Requests. \n" +
                            "Url: {url}.\n" +
                            "Headers:\n{headers}\n" +
                            "Body:\n{body}", url, JsonConvert.SerializeObject(headers, Formatting.Indented), body);

        var response = await ClientUtils.ClientInstance.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    /// <summary>
    /// with header X-GEWE-TOKEN
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    internal async Task<string> PostJsonRequestWithHeaderAsync(
        string controller,
        string? body = null)
    {
        var url = ApiUrl + controller;

        var request = new HttpRequestMessage(HttpMethod.Post, url);

        request.Headers.Add("X-GEWE-TOKEN", $"{Token}");
        //request.Headers.Add("User-Agent", "Apifox/1.0.0 (https://apifox.com)");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Content = new StringContent(body ?? "{}", Encoding.UTF8, "application/json");

        Serilog.Log.Verbose("Post Json Requests With Header. \n" +
                            "Url: {url}.\n" +
                            "Body:\n{body}", url, body);

        var response = await ClientUtils.ClientInstance.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        Serilog.Log.Verbose("Result: {result}", JsonConvert.SerializeObject(result, Formatting.Indented));
        return result;
    }
}