using Kas.Test.Console;
using Newtonsoft.Json;

//var name = "NACHO";
//var interval = "1d"; // 288 673 721

//var url = $"https://api-v2-do.kas.fyi/token/krc20/{name.ToUpper()}/info?includeCharts=true&interval={interval}";

//Console.WriteLine(url);
//var client = new HttpClient();

//var response = await client.GetStringAsync(url);

//File.WriteAllText("kas", response);

var response = File.ReadAllText("kas");
var info = JsonConvert.DeserializeObject<KasInfo>(response)!;
Console.WriteLine(info.StatsHistories.Count);
for (int i = 0; i < info!.StatsHistories.Count; i++)
{
    var history = info.StatsHistories[i];
    Console.WriteLine($"{history.Time}\t" +
                      $"{history.HolderCount}\t" +
                      $"{history.MintCount}\t" +
                      $"{history.TransferCount}");
}

// 931269420002
// 931269250003