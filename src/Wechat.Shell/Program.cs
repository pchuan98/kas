using Gewechat;

Task.Run(async () =>
{
    await Task.Delay(2000);
    var wechat = new WeChat(
        "http://127.0.0.1:2531/v2/api",
        "23e1b0e5f20848388739ffd37300b2eb",
        "wx_Y0N3Sm-XH1CeDm4h5VCf1",
        "Yf_c-xWmHJLHGyJShSMk");

    //Console.WriteLine(await wechat.RequireToken());
    //Console.WriteLine(await wechat.RequireLoginQr());
    //Console.WriteLine(await wechat.RequireLogin());
    //Thread.Sleep(4000);
    Console.WriteLine(wechat.IsOnline);
    Console.WriteLine(await wechat.SetCallbackUrl("http://100.64.4.199:5099/api/GewechatCallback"));
    Console.Read();

});

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

var app = builder.Build();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();