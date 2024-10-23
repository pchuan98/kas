namespace Wechat.Shell.Commands;

public interface IInteractiveCommand : ICommandBase
{
    /// <summary>
    /// 唤醒关键字
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// 参数列表
    /// </summary>
    public string Args { get; set; }

    /// <summary>
    /// 任务执行人
    /// </summary>
    public Action Executor { get; }
}