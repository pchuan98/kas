using Demo.Test;
using KasTools.Models.Enhance;
using KasTools.Utils;
using OpenCvSharp;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateLogger();

var tokens = (await TokenUtil.QueryAll())
    //.Where(item => item.Status == "Minting")
    .Select(item => $"{item.Ticker:10} {item.Status} {item.MintPersent * 100:F2}");

Console.WriteLine(string.Join("\n", tokens));

return;

args = ["nacho", "kdao", "ghoad", "gdfi", "kasper", "konan", "burt",
    "koak", "koka", "mark", "kstr", "rakun", "somps", "phant", "fomoon",
    "kango", "swamp", "baka", "koon", "nicha", "badboy", "kimi", "stick",
    "kengy", "pintl", "kasy", "shit", "keiro"];

var msg = $"[TIME] {DateTime.Now:MM-dd HH:mm:ss}\n\n";

var charts = await TickerUtils.QueryCharts(args!);

//var marketInfo = await MarketUtils.Query();

if (!System.IO.File.Exists("TokensIds"))
    await System.IO.File.WriteAllTextAsync("TokensIds", "");

var oks = (await System.IO.File.ReadAllLinesAsync("TokensIds"))
    .SkipWhile(string.IsNullOrEmpty)
    .Where(item => item.Contains(","))
    .Select(item => (item.Split([','])[0], double.Parse(item.Split([','])[1])))
    .ToHashSet();

var oksStr = oks.Select(item => item.Item1.ToUpper()).ToHashSet();
var argsStr = args.Select(item => item.ToUpper());

var diff = argsStr.Except(oksStr).ToList();
if (diff.Any())
{
    Console.WriteLine("有新数据待加载，请稍等");
    var result = await TransferUtils.QueryPerMintCounts(diff);

    foreach (var item in result)
        await System.IO.File.AppendAllTextAsync("TokensIds", $"{item.Key.ToUpper()},{item.Value}\n");
}

oks = (await System.IO.File.ReadAllLinesAsync("TokensIds"))
    .SkipWhile(string.IsNullOrEmpty)
    .Where(item => item.Contains(","))
    .Select(item => (item.Split([','])[0], double.Parse(item.Split([','])[1])))
    .ToHashSet();

foreach (var arg in args!)
{
    try
    {
        var chart = charts.FirstOrDefault(item => string.Equals(arg.Trim(), item.Ticker, StringComparison.OrdinalIgnoreCase));

        var count = oks.FirstOrDefault(item => item.Item1 == arg.ToUpper()).Item2;
        var mintPrice = 1.1 / count;

        var price = chart?.PriceHistories?[^1]?.Price;

        if (price is null) continue;

        var ticker = chart?.Ticker?.ToUpper()?.Trim();

        msg += mintPrice > 0.00000000000001
            ? $"[烟花] {ticker,-10} ({price / mintPrice:F2}) : {price:F8} KAS\n"
            : $"[烟花] {ticker,-10} : {price:F8} KAS\n";

    }
    catch (Exception e)
    {
        Serilog.Log.Error(e.Message);
    }
}

Console.WriteLine(msg);

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


//var name = "burt";


//var trans = await TransferUtils.QueryAll(name, 256);
//File.WriteAllText("burt.txt", JsonConvert.SerializeObject(trans.OrderByDescending(item => item.OperationScore)));

//var current = DateTime.Now;
//var mint = trans.Where(item => item is { Operation: "MINT" })
//    .GroupBy(item => (int)(current - item.UpdatedAt)!.Value.TotalMinutes / 60)
//    .Select(item => new { Key = item.Key, Items = item.ToArray() });

//foreach (var m in mint)
//{
//    Console.WriteLine($"{m.Key,-10}" +
//                      $"{m.Items.Where(item => item.OpAccepted == 1).Sum(item => item.Amount) / 100000000000,-15}" +
//                      $"{m.Items.Sum(item => item.Amount) / 100000000000,-15}" +
//                      $"{m.Items.Count(item => item.OpAccepted == 1),-15}" +
//                      $"{m.Items.Count(item => item.OpAccepted != 1),-15}");
//}


//DrawCurve(mint.Select(item => (double)item.Key).ToArray()
//    , mint.Select(item => (double)item.Items.Length).ToArray());



// todo
//var trending = await TrendingUtils.QueryAll();

//var transfers = await TickerUtils.QueryCharts(
//    trending?.MostTransferred
//        ?.Take(20)
//        .Select(item => item.Ticker)!,
//    ChartInterval.Y1,
//    threadCount: 32);

//var mintsTrans = await TickerUtils.QueryCharts(
//    trending?.MostMinted?.Select(item => item.Ticker)!);

//foreach (var mint in trending?.MostMinted!)
//{
//    var ticker = mintsTrans.FirstOrDefault(item => item.Ticker == mint.Ticker);
//    var time = ticker!.DeployedAt.ToHumanDateString();
//    var holders = ticker.Holders.Select(item => item.Address)?.Where(item => !string.IsNullOrEmpty(item));

//    var back = string.Join(",", transfers
//        .Where(
//            item => holders != null
//                    && item!.Holders
//                        .OrderByDescending(holder => holder.Amount)
//                        .Take(10)
//                        !.Select(holder => holder.Address)
//                        .Intersect(holders)
//                        .Any()
//        )
//        .Select(item => item.Ticker)
//    );

//    Console.WriteLine($"{mint.Ticker,-10}\t{time,10}\t{mint.Count,-10}\t{string.Join(",", back)}");
//}



//var tokens = await TokenUtil.QueryAll();



//var info = MintManager.ParseMint("phant", tokens: tokens).Result;

//Console.WriteLine(info);

//var info = await TickerUtils.QueryChart("fomoon");
//var marketInfo = await MarketUtils.Query();


//foreach (var market in info.MarketsDatas)
//{
//    Console.WriteLine(market.Name);

//    Console.WriteLine(market.MarketData.PriceInUsd / marketInfo.Price);
//    Console.WriteLine(market.MarketData.VolumeInUsd);
//    Console.WriteLine(market.Metadata.Name);
//    Console.WriteLine(market.Metadata.IconUrl);
//    Console.WriteLine(market.Metadata.Url);

//    Console.WriteLine(new string('-', 100));
//}

//var tokens = await TokenUtil.QueryAll();



static Mat DrawNormalizedCurve(IEnumerable<(IEnumerable<double> x, IEnumerable<double> y)> sets)
{
    var image = new Mat(800, 1500, MatType.CV_8UC3);

    foreach (var (x, y) in sets)
    {
        var xvals = x as double[] ?? x.ToArray();
        var yvals = y as double[] ?? y.ToArray();

        var xmin = xvals.Min();
        var xmax = xvals.Max();
        var ymin = yvals.Min();
        var ymax = yvals.Max();

        var points = new List<Point>();
        for (var i = 0; i < xvals.Length; i++)
        {
            var normX = 10 + (int)((xvals[i] - xmin) / (xmax - xmin + 1) * (image.Width - 20));
            var normY = image.Height - 10 - (int)((yvals[i] - ymin) / (ymax - ymin + 1) * (image.Height - 20));
            points.Add(new Point(normX, normY));
        }

        Cv2.Polylines(image,
            [points],
            false,
            new Scalar(Random.Shared.NextInt64(100, 255), Random.Shared.NextInt64(100, 255), Random.Shared.NextInt64(100, 255)),
            2);
    }

    return image;
}