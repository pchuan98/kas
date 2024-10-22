using System.Net;
using System.Text;
using Gewechat;
using Gewechat.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

Console.WriteLine(new string('=', 20));

CallbackServer.RunTCP();

var login = new Login();

var qr = await login.LoginWithQr("");
Console.WriteLine(qr);

await login.AutoLiveCheck();




while (true)
{
    await Task.Delay(5000);
    Console.WriteLine("Start mint");
    var headers = new Dictionary<string, string>
    {
        {"X-GEWE-TOKEN",$"{login.Token.Data}"},
    };
    var body = new JObject
    {
        ["token"] = $"{login.Token.Data}",
        ["callbackUrl"] = "http://192.168.2.51:5135/api/Demo/",
    };

    Console.WriteLine(JsonConvert.SerializeObject(headers));
    Console.WriteLine(body);

    Console.WriteLine(await Utils.PostRequestAsync("/tools/setCallback", headers, body.ToString()));


}