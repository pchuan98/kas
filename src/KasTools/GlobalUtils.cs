namespace KasTools;

public static class GlobalUtils
{
    /// <summary>
    /// 通用的基础client
    /// </summary>
    internal static readonly HttpClient Client
        = new(new HttpClientHandler()
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        })
        {
            Timeout = TimeSpan.FromSeconds(60)
        };

}