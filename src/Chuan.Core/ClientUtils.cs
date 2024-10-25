using System.Net.Http.Json;

namespace Chuan.Core;

public static class ClientUtils
{
    private static readonly HttpClient Client = new();

    public static async Task<string?> Post(string url, object content)
    {
        var response = await Client.PostAsJsonAsync(url, content);

        if (!response.IsSuccessStatusCode) return null;
        
        var result = await response.Content.ReadAsStringAsync();
        return result;

    }
}