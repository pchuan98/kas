using Gewechat.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gewechat;

public partial class WeChat
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="wxid"></param>
    /// <param name="content"></param>
    /// <returns></returns>
    public async Task<bool> SendFriendStringMsg(string wxid, string content)
    {
        var body = new JObject
        {
            ["appId"] = AppId,
            ["toWxid"] = wxid,
            ["ats"] = content,
            ["content"] = content,
        };

        var recall = await PostJsonRequestWithHeaderAsync("/message/postText", body.ToString());

        return JsonConvert.DeserializeObject<BaseResponse>(recall)?.ReturnCode == 200;
    }
}