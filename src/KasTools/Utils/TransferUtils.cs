using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Chuan.Core;
using KasTools.Converters;
using Newtonsoft.Json;
using Serilog;

namespace KasTools.Utils;

/// <summary>
/// 
/// </summary>
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
    public ulong? OperationScore { get; set; }

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
    public ulong? NextCursor { get; set; }

    [JsonProperty("previousCursor")]
    public ulong? PreviousCursor { get; set; }

    [JsonProperty("results")]
    public Transfer[]? Transfers { get; set; }
}

public static class TransferUtils
{
    internal const string BASE_URL = "https://api-v2-do.kas.fyi/token/krc20/transactions";

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ticker"></param>
    /// <param name="threadCount"></param>
    /// <param name="start">最新的score</param>
    /// <param name="end">最后一个score</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static async Task<IEnumerable<Transfer>> QueryAll(
        string ticker,
        int threadCount = 32,
        ulong? start = null,
        ulong? end = null)
    {
        ticker = ticker.ToUpper();
        var url = $"{BASE_URL}?ticker={ticker.ToUpper()}";

        if (start is null)
        {
            var callback = JsonConvert.DeserializeObject<TransferCallbackModel>(
                await ClientUtils.ClientInstance.GetStringAsync(url));

            start = (ulong)callback?.NextCursor!;

            if (end == null)
            {
                var right = (ulong)start;
                var left = (ulong)callback?.NextCursor! - 100000000000;
                var mid = (ulong)(left + (right - left) / 2);

                start = right;
                end = left;

                callback = JsonConvert.DeserializeObject<TransferCallbackModel>(
                    await ClientUtils.ClientInstance.GetStringAsync(
                        $"{BASE_URL}?nextCursor={left}&ticker={ticker.ToUpper()}"));

                // 探测最后一个结果
                if (callback?.Transfers?.Length != 50)
                {
                    do
                    {
                        url = $"{BASE_URL}?nextCursor={mid}&ticker={ticker.ToUpper()}";

                        callback = JsonConvert.DeserializeObject<TransferCallbackModel>(
                            await ClientUtils.ClientInstance.GetStringAsync(url));

                        if (callback?.PreviousCursor != null)
                            right = mid;
                        else
                            left = mid;

                        mid = left + (right - left) / 2;

                        Log.Logger.Information("Detect [{count,-2}] -> {url}", callback?.Transfers?.Length, url);

                        if (!(callback!.Transfers!.Length == 0 || callback.Transfers.Length == 50))
                            break;

                    } while (right - left > 0);

                    end = mid;
                }
                else
                {
                    Log.Warning("The history is better than 10000000000");
                }
            }
        }

        Log.Information("{ticker} Query limit -> {start} - {end}", ticker.ToUpper(), start, end);

        // 多线程
        var urlsQueue = new BlockingCollection<string>();
        var hash = new ConcurrentDictionary<ulong, Transfer>();

        var count = threadCount;
        var tasks = new Task[count];

        var interval = (start - end) / (ulong)count;

        using var semaphore = new SemaphoreSlim(threadCount);
        QueryAllBinary(ticker.ToUpper(), (ulong)start!, (ulong)end!, hash, semaphore);

        await Task.Delay(100);
        while (semaphore.CurrentCount != threadCount)
        {
            await Task.Delay(5000);
            Log.Debug("HASH -> {hash,-20}| Lock -> {count,-3} / {all,-3}", hash.Count, semaphore.CurrentCount, threadCount);
        }

        //for (var i = 0; i < count; i++)
        //{
        //    urlsQueue.Add($"{BASE_URL}?nextCursor={start - interval * (ulong)i}&ticker={ticker.ToUpper()}");

        //    var id = i;
        //    tasks[i] = Task.Run(async () =>
        //    {
        //        await QueryCore(ticker.ToUpper(), (ulong)end!, urlsQueue, hash, id);

        //        Log.Information("{complete} / {total}", tasks.Count(t => !t.IsCompleted) - 1, count);
        //    });
        //}

        //await Task.WhenAll(tasks);

        var keys = hash.Keys.Order();
        var trans = keys.Select(item => hash.GetValueOrDefault(item));

        return trans.Select(array => array!);
    }

    internal static async Task QueryCore(
        string name,
       ulong start,
        ulong end,
        BlockingCollection<string> queue,
        ConcurrentDictionary<ulong, Transfer> hash,
        int id)
    {
        while (queue.TryTake(out var url))
        {
            try
            {
                Log.Verbose("[{id}] Try query url -> {url}", Thread.CurrentThread.ManagedThreadId, url);

                if (string.IsNullOrEmpty(url))
                    break;

                var callback = JsonConvert.DeserializeObject<TransferCallbackModel>(
                    await ClientUtils.ClientInstance.GetStringAsync(url));

                // 没有数据返回
                if (callback?.NextCursor is null
                    || callback?.PreviousCursor is null)
                    break;

                var trans = callback.Transfers;
                if (trans is null) break;

                var count = trans.Length;
                var index = 0;

                for (var i = 0; i < count; i++)
                    if (!hash.TryAdd((ulong)trans[i].OperationScore!, trans[i]))
                        index++;

                if (index == count)
                    break;

                queue.Add($"{BASE_URL}?nextCursor={callback.NextCursor}&ticker={name}");

                Log.Debug("[{id,-3}]\tQuery count : {count}", id, hash.Count);
            }
            catch (Exception e)
            {
                Log.Error("[ {id} ] Query url error -> {url}", id, url);
                queue.Add(url);

                await Task.Delay(2000);
            }
            await Task.Delay(500);
        }

        Log.Information("Query url thread quit. -> {id}", id);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="hashTable"></param>
    /// <param name="semaphore"></param>
    /// <returns></returns>
    public static void QueryAllBinary(
        string name,
        ulong start,
        ulong end,
        ConcurrentDictionary<ulong, Transfer> hashTable,
        SemaphoreSlim semaphore)
    {
        if (start <= end)
        {
            Log.Debug("Exit -> Start <= End");
            return;
        }

        var mid = start - (start - end) / 2;

        Log.Debug("{start} - {mid} - {end}", start, mid, end);

        Task.Run(() =>
        {
            semaphore.Wait();
            var leftUrl = $"{BASE_URL}?nextCursor={start}&ticker={name}";

            var str = ClientUtils.SafeGetString(leftUrl).Result;
            semaphore.Release();
            Log.Debug("[{count}] Query url -> {url}", semaphore.CurrentCount, leftUrl);

            if (string.IsNullOrEmpty(str))
            {
                QueryAllBinary(name, start, end, hashTable, semaphore);
                return;
            }

            var leftCallback = JsonConvert.DeserializeObject<TransferCallbackModel>(str);

            if (leftCallback?.NextCursor is null
                || leftCallback.PreviousCursor is null
                || leftCallback.Transfers is null
                || leftCallback.Transfers.Length == 0) return;

            var ll = (ulong)leftCallback.NextCursor!;

            if (ll < start)
            {
                var count = leftCallback.Transfers.Length;
                for (var i = 0; i < count; i++)
                    hashTable.TryAdd((ulong)leftCallback.Transfers[i].OperationScore!,
                        leftCallback.Transfers[i]);

                QueryAllBinary(name, ll, mid - 2, hashTable, semaphore);
            }
            else
            {
                Log.Debug("Exit -> LL < Start");
                return;
            }


        });

        Task.Run(() =>
        {
            semaphore.Wait();
            var rightUrl = $"{BASE_URL}?nextCursor={mid}&ticker={name}";

            var str = ClientUtils.SafeGetString(rightUrl).Result;
            semaphore.Release();
            Log.Debug("[{count}] Query url -> {url}", semaphore.CurrentCount, rightUrl);

            if (string.IsNullOrEmpty(str))
            {
                QueryAllBinary(name, start, end, hashTable, semaphore);
                return;
            }

            var rightCallback = JsonConvert.DeserializeObject<TransferCallbackModel>(str);

            if (rightCallback?.NextCursor is null
                || rightCallback.PreviousCursor is null
                || rightCallback.Transfers is null
                || rightCallback.Transfers.Length == 0) return;

            var rr = (ulong)rightCallback.NextCursor!;

            if (rr > end)
            {
                var count = rightCallback.Transfers.Length;

                for (var i = 0; i < count; i++)
                    hashTable.TryAdd((ulong)rightCallback.Transfers[i].OperationScore!, rightCallback.Transfers[i]);

                QueryAllBinary(name, rr, end, hashTable, semaphore);
            }
            else
            {
                Log.Debug("Exit -> RR > End");
                return;
            }
        });


    }

    public static async Task<IEnumerable<Transfer>> QueryAllMint(string ticker, IEnumerable<Transfer>? transfers = null)
    {
        var trans = transfers ?? await QueryAll(ticker);

        return trans.Where(item => item.Operation == "MINT")
            .Where(item => item.OpAccepted == 1);
    }
}