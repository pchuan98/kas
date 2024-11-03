using KasTools.Utils;
using OpenCvSharp;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Verbose()
    .WriteTo.Console()
    .CreateLogger();

while (true)
{
    var res = await TokenUtil.QueryAll();

    Log.Debug(res.Length.ToString());
}





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



//var info = MintManager.ParseMint("lickin", tokens: tokens).Result;

//Console.WriteLine(info);


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