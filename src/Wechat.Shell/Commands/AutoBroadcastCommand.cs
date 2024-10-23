using System.Text.RegularExpressions;
using KasTools.Utils;

namespace Wechat.Shell.Commands;

/// <summary>
/// 
/// </summary>
public class AutoBroadcastCommand : IInteractiveCommand
{
    /// <summary>
    /// 需要ticker的类型
    /// </summary>
    internal static readonly List<string> Tickers = new();

    /// <summary>
    /// 
    /// </summary>
    private static readonly Regex ArgsPattern = new(@"^(?:wx.*\n)?(.*)");

    /// <summary>
    /// 分割若个空格的单词
    /// </summary>
    private static readonly Regex SplitNamePattern = new(@"\w+");

    /// <inheritdoc />
    public string Wxid { get; set; } = "";

    /// <inheritdoc />
    public string Key { get; private set; } = "";

    /// <inheritdoc />
    public string Args { get; set; } = "";

    /// <inheritdoc />
    public Action Executor { get; }

    /// <summary>
    /// 
    /// </summary>
    public AutoBroadcastCommand()
    {
        Executor = async () =>
        {
            if (Wxid is null) return;

            if (Tickers.Count > 9)
            {
                await WeChatGlobal.Send(Wxid, "The automatic brodcast queue is full.");
                return;
            }

            if (string.IsNullOrWhiteSpace(Wxid)) return;

            var args = Args?.Trim();

            if (string.IsNullOrEmpty(args))
                await WeChatGlobal.Send(Wxid, "add or remove Command Args Error.");

            var match = ArgsPattern.Match(Args!);
            if (!match.Success)
            {
                await WeChatGlobal.Send(Wxid, "add or remove Command Args Error.");
                return;
            }

            if (match.Groups[1].Value.ToLower().Contains("\add"))
                Key = "/add";
            else if (match.Groups[1].Value.ToLower().Contains("\remove"))
                Key = "/remove";
            else
                return;

            var splitArgs = SplitNamePattern.Matches(
                    match.Groups[1].Value
                        .ToLower()
                        .Replace("/add", "")
                        .Replace("/remove", "")
                        .Replace(",", " ")
                        .Replace("，", " ")
                        .Trim()
                        .ToUpper())
                .Select(item => item.Value)
                .ToHashSet()
                .ToList();

            var names = (await TokenUtil.QueryAll())
                ?.Select(item => item.Ticker?.ToLower());

            var valid = splitArgs
                .Where(item => names?.Contains(item) is true);

            var array = valid as string[] ?? valid.ToArray();

            if (Tickers.Count + array.Length > 10)
            {
                await WeChatGlobal.Send(Wxid, "The automatic brodcast queue is full.");
                return;
            }

            if (Key?.ToLower() == "/add") await Add(array);
            else if (Key?.ToLower() == "/remove") await Remove(array);
            else await WeChatGlobal.Send(Wxid, "Command must /add or /remove.");
        };
    }

    // note 可能有线程安全问题
    internal async Task Add(IEnumerable<string> valid)
    {
        foreach (var v in valid)
            Tickers.Add(v);

        await WeChatGlobal.Send(Wxid, $"Current broadcast tickers: {string.Join(" ", Tickers)}");
    }

    internal async Task Remove(IEnumerable<string> valid)
    {
        foreach (var v in valid)
            Tickers.Remove(v);

        if (Tickers.Count != 0)
            await WeChatGlobal.Send(Wxid, $"Current broadcast tickers: {string.Join(" ", Tickers)}");
    }
}