using Chuan.Core;
using Chuan.Core.Models;
using Microsoft.AspNetCore.Mvc;
using PChuan.Core;

namespace Kas.Api.Shell.Controllers;

/// <summary>
/// 自动定时任务
/// </summary>
/// <param name="Registrant">发起任务的人 只能是个人</param>
/// <param name="Receiver">接受者，可以是人，可以是群</param>
/// <param name="Interval"></param>
/// <param name="Alias"></param>
/// <param name="Task"></param>
/// <param name="Args"></param>
public record TaskTimer(
    string Registrant,
    string Receiver,
    TimeSpan Interval,
    string Alias,
    string Task,
    string[]? Args);

public record TaskCell(DateTime Last, TaskTimer Task);

/// <summary>
/// 统计价格相关
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class TimerController : ControllerBase
{
    //internal static readonly HttpClient Client = new();

    /// <summary>
    /// 
    /// </summary>
    internal static string? BaseUrl = null;

    /// <summary>
    /// 每分钟自动运行
    /// </summary>
    private static readonly Timer MinuteTimer
        = new(MiniteTimerCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(59));

    /// <summary>
    /// TaskTimerCollection类型合集
    /// </summary>
    private static List<(DateTime Last, TaskTimer Task)> TaskTimerCollection = new();

    private static readonly object _lock = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="state"></param>
    /// <exception cref="NotImplementedException"></exception>
    private static void MiniteTimerCallback(object? state)
    {
        lock (_lock)
        {
            var current = DateTime.Now;

            for (var i = 0; i < TaskTimerCollection.Count; i++)
            {
                var task = TaskTimerCollection[i];
                if (current - task.Last < task.Task.Interval) continue;

                try
                {
                    var back = new CallbackModel()
                    {
                        IsGroup = task.Task.Receiver.Contains("@chatroom"),
                        Group = task.Task.Receiver,
                        Sender = task.Task.Registrant,
                        CommandName = task.Task.Task,
                        Args = task.Task.Args
                    };

                    TaskTimerCollection[i] = new ValueTuple<DateTime, TaskTimer>(current, task.Task);

                    var url = $"{BaseUrl}/{task.Task.Task}";
                    Task.Run(async () => await ClientUtils.ClientInstance.PostAsJsonAsync(url, back));
                }
                catch (Exception e)
                {
                    Serilog.Log.Error(e.Message);
                }
            }
        }
    }

    /// <summary>
    /// 验证函数
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<bool> CheckPermission(CallbackModel request)
    {
        await Task.Delay(10);


        return true;
        // todo 添加其他的东西
        return request.IsSuperAdmin();
    }

    [HttpGet]
    public string Main()
    {
        return $"{Request.Scheme}://{Request.Host}{Request.Path}/{Request.PathBase}";
    }

    /// <summary>
    /// 1. /timer add alias [minutes] [task] [args]
    /// 2. /timer remove alias
    /// 3. /timer start alias
    /// 4. /timer stop alias
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task TimerPost([FromBody] CallbackModel request)
    {
        BaseUrl ??= $"{Request.Scheme}://{Request.Host}/api";

        var permission = await CheckPermission(request);
        if (!permission) return;

        if (request.Args is null || request.Args.Length == 0)
        {
            if (request.IsSuperAdmin())
            {
                await request.SendMessage("/timer add alias [minutes] [task] [args]");
            }
            return;
        }

        var subcommand = request.Args[0];

        _ = subcommand.Trim().ToLower() switch
        {
            "add" => AddTimer(request),
            "remove" => RemoveTimer(request),
            "start" => StartTimer(request),
            "stop" => StopTimer(request),
            _ => request.SendMessage(request.ToString())
        };
    }

    // todo Collection的线程安全没有解决

    internal async Task AddTimer(CallbackModel model)
    {
        lock (_lock)
        {
            if (model.Args is null
                || model.Args?.Length < 4) return;

            var alias = model.Args![1];
            if (!double.TryParse(model.Args[2], out var minute)) return;
            var task = model.Args[3];
            var args = model.Args.Length > 4 ? model.Args[4..] : null;

            TaskTimerCollection.Add(new(DateTime.Now
                , new TaskTimer(
                    model.Sender,
                    model.IsGroup ? model.Group : model.Sender,
                    TimeSpan.FromMinutes(minute),
                    alias,
                    task,
                    args
                )));
        }
    }

    internal async Task RemoveTimer(CallbackModel model)
    {
        //lock (_lock)
        //{
        //    if (model.Args is null
        //        || model.Args?.Length < 4) return;

        //    var alias = model.Args![1];
        //    if (!double.TryParse(model.Args[2], out var minute)) return;
        //    var task = model.Args[3];
        //    var args = model.Args.Length > 4 ? model.Args[4..] : null;


        //}
    }

    internal async Task StartTimer(CallbackModel model)
    {
        await Task.Delay(10);
    }

    internal async Task StopTimer(CallbackModel model)
    {
        await Task.Delay(10);

    }
}