using Gewechat;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateLogger();

var wechat = new WeChat(
    "http://cn.pchuan.site:2531/v2/api",
    "d1b49bfb3f3d4af6b6d094ba65acb8c0",
    "wx_fx1s4DdgDkJj9aAihgeuN",
    "gdbMUbP0qpXfzojo18Dh");

Console.WriteLine(await wechat.RequireLoginQr());
Console.WriteLine(await wechat.RequireLogin());