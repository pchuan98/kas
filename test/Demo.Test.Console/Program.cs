using KasTools;
using KasTools.Utils;
using Newtonsoft.Json;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateLogger();


//var mints = await MintUtil.QueryAllMint();

//var baseinfo = JsonConvert.DeserializeObject<TickerBaseInfo[]>(await File.ReadAllTextAsync("kasinfo"));

//var result = mints.Where(mint => baseinfo?.FirstOrDefault(item => string.Equals(item.Ticker, mint.Ticker!, StringComparison.CurrentCultureIgnoreCase)) is not null)
//    .Select(mint =>
//    {
//        var info = baseinfo?.FirstOrDefault(item =>
//            string.Equals(item.Ticker, mint.Ticker!, StringComparison.CurrentCultureIgnoreCase))!;

//        var sames = baseinfo?.Where(item => item.DeployedBy == info.DeployedBy).ToArray();

//        return (mint, sames);
//    });

//foreach (var (mint, sames) in result.OrderByDescending(t => t.mint.DeployedAt))
//{
//    Log.Information(sames is null || sames.Length == 0
//        ? $"{mint.Ticker!.ToUpper(),-10}{mint.DeployedAt.ToHumanDateString(),-10}-> none"
//        : $"{mint.Ticker!.ToUpper(),-10}{mint.DeployedAt.ToHumanDateString(),-10}-> {string.Join(" ", sames.Select(item => item.Ticker))}");
//}

var name = "RAKUN";

//var trans = await TransferUtils.QueryAll(name);

//var json = JsonConvert.SerializeObject(trans);

//File.WriteAllText("TRANS", json);

await TransferUtils.QueryAll(name);

// 938115450000 -> 938114450000