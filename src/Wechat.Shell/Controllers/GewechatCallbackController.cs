using Microsoft.AspNetCore.Mvc;

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
        public void WeChatCallback(object content)
        {
            Console.WriteLine(content);
        }
    }
}
