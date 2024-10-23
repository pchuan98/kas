using KasTools.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Wechat.Shell.Commands;

namespace Wechat.Shell.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GewechatCallbackController : ControllerBase
    {
        public CommandManager Manager = new();

        public GewechatCallbackController()
        {
            Task.Run(async () =>
            {
                Thread.Sleep(10000);

                await Manager.StartTimer();
            });
        }

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

            if (string.IsNullOrEmpty(wxid) || string.IsNullOrEmpty(msg)) return;

            Manager.DumpMessage(WeChatGlobal.ParseCallback(json));
        }
    }
}
