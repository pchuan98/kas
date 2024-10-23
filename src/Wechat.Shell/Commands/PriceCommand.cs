using System.Text.RegularExpressions;
using KasTools.Utils;

namespace Wechat.Shell.Commands;


public class PriceCommand : IInteractiveCommand
{
    /// <summary>
    /// 
    /// </summary>
    private static readonly Regex ArgsPattern = new(@"^(?:wx.*\n)?(.*)");

    /// <summary>
    /// 分割若个空格的单词
    /// </summary>
    private static readonly Regex SplitNamePattern = new(@"\w+");

    public PriceCommand()
    {
        Executor = () =>
        {
            Task.Run(async () =>
            {
                var args = Args.Trim();

                if (string.IsNullOrEmpty(args))
                    await WeChatGlobal.Send(Wxid, "Price Command Args Error.");

                var match = ArgsPattern.Match(Args);
                if (!match.Success)
                {
                    await WeChatGlobal.Send(Wxid, "Price Command Args Error.");
                    return;
                }

                var names = SplitNamePattern.Matches(
                    match.Groups[1].Value
                        .ToLower()
                        .Replace("/price", "")
                        .Replace(",", " ")
                        .Replace("，", " ")
                        .Trim()
                        .ToUpper())
                    .Select(item => item.Value)
                    .ToHashSet();

                if (names.Count == 0)
                    await WeChatGlobal.Send(Wxid, "Price Command Args Is 0.");

                var tokens = await TokenUtil.QueryAll();

                var msg = $"[TIME] {DateTime.Now:MM-dd HH:mm:ss}\n\n";
                foreach (var name in names)
                {
                    var token = tokens?.FirstOrDefault(token => token.Ticker?.ToUpper().Trim() == $"{name.Trim()}");
                    if (token is null) continue;

                    //msg += $"{name.Trim(),-10}:  {token?.Price?.FloorPrice:F8} KAS\n";
                    msg += $"{name.Trim(),10}:  {token?.Price?.FloorPrice:F8} KAS\n";

                }

                await WeChatGlobal.Send(Wxid, msg);
            });
        };
    }

    /// <inheritdoc />
    public string Key => "/price";

    /// <inheritdoc />
    public string Args { get; set; } = "";

    /// <inheritdoc />
    public Action Executor { get; }

    /// <inheritdoc />
    public string Wxid { get; set; } = "";
}