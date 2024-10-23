using KasTools.Models;

namespace KasTools.Utils;

public static class MintUtil
{
    /// <summary>
    /// 找到所有正在Mint的项目
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<List<Token>> QueryAllMint(IEnumerable<Token>? tokens = null)
    {
        tokens ??= await TokenUtil.QueryAll();
        if (tokens is null)
            throw new Exception();

        var deployed = tokens
            .Where(item => item.Status == "deployed")
            .ToList();

        return deployed;
    }

    /// <summary>
    /// 获取一个小时内刚开始的mint
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<List<Token>> QueryNewMint(IEnumerable<Token>? tokens = null)
    {
        tokens ??= await QueryAllMint();

        var current = DateTime.Now;

        return tokens.Where(mint => (current - mint.DeployedAt).TotalHours <= 1)
            .ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="span"></param>
    /// <returns></returns>
    public static async Task<List<Token>> QueryPercentRangeMint(
        IEnumerable<Token>? tokens = null,
        double min = 0,
        double max = 1,
        TimeSpan? span = null)
    {
        tokens ??= await QueryAllMint();
        span ??= TimeSpan.FromHours(24);

        var current = DateTime.Now;

        return tokens
            .Where(mint => (current - mint.DeployedAt) <= span)
            .Where(mint => mint.MintPersent >= min && mint.MintPersent <= max)
            .ToList();
    }

    /// <summary>
    /// 快一半了
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="span"></param>
    /// <returns></returns>
    public static async Task<List<Token>> QueryHalfRangeMint(IEnumerable<Token>? tokens = null, TimeSpan? span = null)
        => await QueryPercentRangeMint(tokens, 0.45, 0.55, span);

    /// <summary>
    /// 快结束了
    /// </summary>
    /// <param name="tokens"></param>
    /// <param name="span"></param>
    /// <returns></returns>
    public static async Task<List<Token>> QueryFinishRangeMint(IEnumerable<Token>? tokens = null, TimeSpan? span = null)
        => await QueryPercentRangeMint(tokens, 0.95, 1, span);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mints"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<List<Token>> QueryTrendingMint(
        IEnumerable<BaseTrendingObject>? mints = null,
        IEnumerable<Token>? tokens = null)
    {
        mints ??= await TrendingUtils.QueryMint();
        tokens ??= await QueryAllMint();

        if (mints is null)
            throw new Exception();

        return mints
            .OrderByDescending(item => item.Count)
            .Select(b => tokens.FirstOrDefault(t => t.Ticker == b.Ticker))
            .Where(t => t != null)
            .Select(item => item!)
            .ToList();
    }
}