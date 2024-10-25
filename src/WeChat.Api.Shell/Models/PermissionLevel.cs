namespace WeChat.Api.Shell.Models;


/// <summary>
/// 权限等级
/// </summary>
[Flags]
public enum PermissionLevel
{
    /// <summary>
    /// 无权限
    /// </summary>
    None = 0,

    /// <summary>
    /// 组管理员模式，组成员都可以用
    /// </summary>
    GroupAdmin = 1,

    /// <summary>
    /// 组用户模式，允许的用户才能用
    /// </summary>
    GroupUser = 2,

    /// <summary>
    /// 用户模式，判断单独发是否可用
    /// </summary>
    User = 4,

    /// <summary>
    /// 管理员权限
    /// </summary>
    Admin = 8,
}