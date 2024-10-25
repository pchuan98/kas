using KasTools.Models;
using KasTools.Utils;

namespace Kas.Api.Shell.Models;

/// <summary>
/// 自动拉取 + 后期查找的kas信息
/// </summary>
public class AliveInfo
{
    public DateTime Time { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public (DateTime Time, Token[]? Data) Tokens { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task UpdateTokens()
        => Tokens = new(DateTime.Now, await TokenUtil.QueryAll());
}