using KasTools.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Xml.Linq;
using Kas.Test.Console;
using System.Text.RegularExpressions;

namespace Wechat.Shell.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GewechatCallbackController : ControllerBase
    {
        public static HttpClient WebClient = new HttpClient();

        [HttpGet]
        public string SimpleGet()
        {
            return "hello world";
        }

        [HttpPost]
        public async void WeChatCallback([FromBody] object content)
        {
            var json = content.ToString() ?? "";

            var callback = JsonConvert.DeserializeObject<TextCallbackModel>(json);

            if (callback?.Data?.MsgType != 1) return;

            var wxid = callback?.Data?.FromUserName?.WxId;
            var command = callback?.Data?.Content?.MessageContent;

            if (string.IsNullOrEmpty(wxid) || string.IsNullOrEmpty(command)) return;
            if (!command.Contains("/price")) return;

            var pattern = @"^wx.*\n(.*)";
            var match = Regex.Match(command, pattern);
            if (match.Success)
                command = match.Groups[1].Value;
            

            var coinName = command.Replace("/price", "").Trim().ToUpper();
            var url = $"https://api-v2-do.kas.fyi/token/krc20/{coinName}/info?includeCharts=true";
            var coinResponse = await WebClient.GetStringAsync(url);

            var info = JsonConvert.DeserializeObject<KasInfo>(coinResponse)!;

            var price = $"{info.Price?.FloorPrice:F8}";
            var usd = $"{info.Price?.MarketcapInUsd:F8}";

            var msg = $"{coinName} {DateTime.Now:MM-dd HH:mm:ss}\n\n" +
                      $"Float Price: {price}\n";

            var rec = await WeChatGlobal.WechatObject!.SendFriendStringMsg(wxid, msg);
            Console.WriteLine(rec);
        }
    }
}
