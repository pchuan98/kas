using Chuan.Core.Models;

namespace PChuan.Core;


/// <summary>
/// 提供常用的检测手段
/// </summary>
public static class PermissionUtils
{
    public static bool IsSuperAdmin(this CallbackModel model)
        => model.Sender == "wxid_xwmbszzoarry21";
}