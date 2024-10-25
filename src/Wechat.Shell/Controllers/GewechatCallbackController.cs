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

      
    }
}
