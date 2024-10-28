using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chuan.Core.Models;

public class RegisterModel(string name = "", string url = "")
{
    /// <summary>
    /// 命令的url地址
    /// </summary>
    public string Url { get; set; } = url;

    /// <summary>
    /// 命令激活的名称
    /// </summary>
    public string Name { get; set; } = name;


    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}