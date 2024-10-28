using KasTools.Converters;
using Newtonsoft.Json;
using Serilog;

namespace KasTools.Utils;

public class Transfer
{
    [JsonProperty("amount")]
    public decimal? Amount { get; set; }

    [JsonProperty("createdAt")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? CreateAt { get; set; }

    [JsonProperty("fromAddress")]
    public string? FromAddress { get; set; }

    [JsonProperty("opAccepted")]
    public int? OpAccepted { get; set; }

    [JsonProperty("opError")]
    public string? OpError { get; set; }

    [JsonProperty("operation")]
    public string? Operation { get; set; }

    [JsonProperty("operationScore")]
    public decimal? OperationScore { get; set; }

    [JsonProperty("ticker")]
    public string? Ticker { get; set; }

    [JsonProperty("toAddress")]
    public string? ToAddress { get; set; }

    [JsonProperty("updatedAt")]
    [JsonConverter(typeof(UnixTimestampConverter))]
    public DateTime? UpdatedAt { get; set; }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public static bool operator ==(Transfer? u1, Transfer? u2)
        => (u1 is not null && u2 is not null)
            && u1.Amount == u2.Amount
            && u1.CreateAt == u2.CreateAt
            && u1.FromAddress == u2.FromAddress
            && u1.OpAccepted == u2.OpAccepted
            && u1.OpError == u2.OpError
            && u1.OperationScore == u2.OperationScore
            && u1.Ticker == u2.Ticker
            && u1.ToAddress == u2.ToAddress
            && u1.UpdatedAt == u2.UpdatedAt;

    public static bool operator !=(Transfer? u1, Transfer? u2)
        => !(u1 == u2);
}

public class TransferCallbackModel
{
    [JsonProperty("nextCursor")]
    public decimal? NextCursor { get; set; }

    [JsonProperty("previousCursor")]
    public decimal? PreviousCursor { get; set; }

    [JsonProperty("results")]
    public Transfer[]? Transfers { get; set; }
}

public static class TransferUtils
{
    internal const string BaseUrl = "https://api-v2-do.kas.fyi/token/krc20/transactions";

    public static async Task<IEnumerable<Transfer>> QueryAll(string ticker)
    {
        ticker = ticker.ToUpper();
        var url = $"{BaseUrl}?ticker={ticker.ToUpper()}";

        var callback = JsonConvert.DeserializeObject<TransferCallbackModel>(
            await GlobalUtils.Client.GetStringAsync(url));

        var right = (decimal)callback?.NextCursor!;
        var left = (decimal)(callback?.NextCursor - 1000000000)!;
        var mid = Math.Truncate(left + (right - left) / 2);

        // 探测最后一个结果
        do
        {
            url = $"{BaseUrl}?nextCursor={mid}&ticker={ticker.ToUpper()}";

            callback = JsonConvert.DeserializeObject<TransferCallbackModel>(
                await GlobalUtils.Client.GetStringAsync(url));

            if (callback?.PreviousCursor != null)
                right = mid;
            else
                left = mid;

            mid = Math.Truncate(left + (right - left) / 2);

            Console.WriteLine($" {callback?.Transfers?.Length} -> {url}");


        } while (right - left > 50);

        Console.WriteLine(mid);



        throw new Exception();

        //try
        //{
        //    do
        //    {
        //        if (string.IsNullOrEmpty(callback?.NextCursor)
        //            || callback?.Transfers is null) break;

        //        if (callback.Transfers!.Any(item => item.Ticker == ticker) is not true)
        //        {
        //            transfers.AddRange(callback.Transfers.Where(item => item.Ticker == ticker));
        //            break;
        //        }

        //        transfers.AddRange(callback.Transfers);

        //        Log.Information("Query transfer : {count} {pre} {next}", callback.Transfers?.Length ?? 0,
        //            callback.PreviousCursor, callback.NextCursor);

        //        url =
        //            $"https://api-v2-do.kas.fyi/token/krc20/transactions?nextCursor={callback.NextCursor}&ticker={ticker.ToUpper()}";

        //        Log.Verbose($"query mint url => {url}", url);

        //        callback = JsonConvert.DeserializeObject<TransferCallbackModel>(
        //            await GlobalUtils.Client.GetStringAsync(url));
        //    } while (!string.IsNullOrEmpty(callback?.NextCursor));
        //}
        //catch (Exception e)
        //{
        //    Log.Error($"{url}\n{e.Message}");
        //}
    }

    public static async Task<IEnumerable<Transfer>> QueryAllMint(string ticker, IEnumerable<Transfer>? transfers = null)
    {
        var trans = transfers ?? await QueryAll(ticker);

        return trans.Where(item => item.Operation == "MINT")
            .Where(item => item.OpAccepted == 1);
    }
}