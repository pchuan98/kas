using Newtonsoft.Json;
using WeChat.Api.Shell.Models;

namespace WeChat.Api.Shell;

public static class WeChatUtil
{
    private static Gewechat.WeChat? _wechat = null;

    /// <summary>
    /// 
    /// </summary>
    public static Gewechat.WeChat Instance
    {
        get
        {
            if (_wechat is not null) return _wechat;

            if (ResetWeChat().Result 
                && _wechat is not null) return _wechat;

            throw new Exception();
        }
    }

    /// <summary>
    /// 重新设置 WeChat 信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<bool> ResetWeChat()
    {
        if (!File.Exists(ValueBox.WE_CHAT_CONFIG_PATH))
        {
            Serilog.Log.Warning($"The File {ValueBox.WE_CHAT_CONFIG_PATH} not create.");

            var info = JsonConvert.SerializeObject(new WeChatInfo(), Formatting.Indented);
            await File.WriteAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH, info);
        }

        var text = await File.ReadAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH);
        var obj = JsonConvert.DeserializeObject<WeChatInfo>(text);

        if (obj is null)
        {
            Serilog.Log.Error("WeChat config file read failed.");
            throw new Exception();
        }

        if (obj.Url is null)
        {
            Serilog.Log.Error("Must set WeChat Url Path!");
            Console.Write("Please input the WeCaht API URL :");

            obj.Url = Console.ReadLine();
        }

        Serilog.Log.Information("WeChat Config : {config}", obj);

        _wechat = new Gewechat.WeChat(obj.Url!, obj.Token, obj.AppId, obj.Uuid);

        await Save();
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static async Task Login()
    {
        // todo 这里的形式要改一下了

        await Instance.RequireToken();
        Console.WriteLine(await Instance.RequireLoginQr());
        await Instance.RequireLogin();

        await Save();
    }

    /// <summary>
    /// 保存配置文件
    /// </summary>
    /// <returns></returns>
    public static async Task Save()
    {
        var url = Instance.ApiUrl;
        var token = Instance.Token;
        var appid = Instance.AppId;
        var uuid = Instance.Uuid;

        var info = new WeChatInfo()
        {
            Url = url,
            Token = token,
            AppId = appid,
            Uuid = uuid
        };

        var json = JsonConvert.SerializeObject(info, Formatting.Indented);
        await File.WriteAllTextAsync(ValueBox.WE_CHAT_CONFIG_PATH, json);
    }
}