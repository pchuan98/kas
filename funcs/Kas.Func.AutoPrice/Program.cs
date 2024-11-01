using Chuan.Core;
using Chuan.Core.Models;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateLogger();

// зЂВс command
var commands = new List<RegisterModel>()
{
    new("price","http://122.152.227.199:5098/api/price")
};

commands.ForEach(command =>
{
    var response = ClientUtils.ClientInstance.PostAsJsonAsync("http://122.152.227.199:5099/api/callback/register", command.ToString()).Result;

    if (response.IsSuccessStatusCode)
        Log.Logger.Information("Registing {name}", command.Name);
    else
        Log.Logger.Error("Registing {name}", command.Name);
});

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();