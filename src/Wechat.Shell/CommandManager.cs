using KasTools.Utils;
using Wechat.Shell.Commands;
using Wechat.Shell.Models;

namespace Wechat.Shell;

/// <summary>
/// 1、1min的自动定时器工具
/// 2、验证器
///
/// todo 个人命令验证器、定时任务验证动态增减
/// </summary>
public class CommandManager
{
    private static readonly string[] WhiteListGroup = [
        "53473027910@chatroom"
    ];

    private static readonly Dictionary<string, IInteractiveCommand> Commands = new()
    {
        {"/price",new PriceCommand()},
        {"/add",new AutoBroadcastCommand()},
        {"/remove",new AutoBroadcastCommand()},
    };

    private static readonly Timer MinuteTimer = new Timer(MiniteTimerCallback, null, Timeout.Infinite, Timeout.Infinite);

    private static async void MiniteTimerCallback(object? state)
    {
        // 整点运行
        if (AutoBroadcastCommand.Tickers.Count != 0)
        {
            var msg = $"[TIME] {DateTime.Now:MM-dd HH:mm:ss}\n\n";
            var tokens = await TokenUtil.QueryAll();

            foreach (var name in AutoBroadcastCommand.Tickers)
            {
                var token = tokens?.FirstOrDefault(token => token.Ticker?.ToUpper().Trim() == $"{name.Trim()}");
                if (token is null) continue;

                //msg += $"{name.Trim(),-10}:  {token?.Price?.FloorPrice:F8} KAS\n";
                msg += $"{name.Trim(),-10}:  {token?.Price?.FloorPrice:F8} KAS\n";
            }

            foreach (var group in WhiteListGroup)
                await WeChatGlobal.Send(group, msg);
        }

        //await WeChatGlobal.Send(WhiteListGroup[0], DateTime.Now.Second.ToString());
    }

    public void DumpMessage(ReceiveMessageModel message)
    {
        if (!message.IsGroup) return;
        if (!WhiteListGroup.Contains(message.Group)) return;

        // content to command and args
        foreach (var command in
                 from commandName in Commands.Keys
                 where message.Content!.ToLower().Contains(commandName)
                 select Commands[commandName])
        {
            command.Args = message.Content;
            command.Wxid = message.Group!;
            command.Executor?.Invoke();

            break;
        }
    }

    public async Task StopTimer()
    {
        MinuteTimer.Change(Timeout.Infinite, Timeout.Infinite);

        foreach (var group in WhiteListGroup)
            await WeChatGlobal.Send(group, "## The hourly time chime function has been turned off! ##");
    }

    public async Task StartTimer()
    {
        MinuteTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(61));

        foreach (var group in WhiteListGroup)
            await WeChatGlobal.Send(group, "[庆祝] The hourly time chime function has been turned on! [庆祝]");
    }

    public static readonly CommandManager Instance = new();
}