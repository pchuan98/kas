using Gewechat;
using Serilog;
using Serilog.Core;
using Wechat.Shell;

// todo Login wechat detect

Task.Run(async () =>
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Console()
        .CreateLogger();

    await Task.Delay(2000);

    await WeChatGlobal.SetCallbackUrl();
});

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();