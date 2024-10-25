using Serilog;
using WeChat.Api.Shell;

Log.Logger = new LoggerConfiguration()
    //.MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateLogger();

Task.Run(async () =>
{
    await Task.Delay(1000);
    //await WeChatUtil.Login();
    Log.Information("The wechat is online :{online}", WeChatUtil.Instance.IsOnline);
    Log.Information("Set callback url : {rec}", await WeChatUtil.Instance.SetCallbackUrl("http://100.64.114.107:5099/api/callback"));
});

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();
