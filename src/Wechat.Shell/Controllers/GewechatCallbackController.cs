using KasTools.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Wechat.Shell.Commands;

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
            var msg = callback?.Data?.Content?.MessageContent;

            Serilog.Log.Verbose("{wxid} : {msg}", wxid, msg);

            //if (msg.Contains("[旺柴]")  ) await WeChatGlobal.Send(wxid, "[旺柴]");

            if (string.IsNullOrEmpty(wxid) || string.IsNullOrEmpty(msg)) return;
            if (!msg.Contains("/price")) return;

            IInteractiveCommand priceCommand = new PriceCommand()
            {
                Args = msg,
                Wxid = wxid
            };
            priceCommand.Executor?.Invoke();
        }
    }
}
