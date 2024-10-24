using Gewechat.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Wechat.Shell.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GewechatCallbackController : ControllerBase
    {
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

            CommandManager.Instance.DumpMessage(WeChatGlobal.ParseCallback(json));
        }
    }
}
