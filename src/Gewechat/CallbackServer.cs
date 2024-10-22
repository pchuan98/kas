using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Gewechat;

public class CallbackServer
{

    public static async Task RunTCP()
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        Console.WriteLine("Listening...");

        while (true)
        {
            var context = await listener.GetContextAsync();
            if (context.Request.HttpMethod == "POST")
            {
                // 处理请求头
                Console.WriteLine("Headers:");
                foreach (var header in context.Request.Headers.AllKeys)
                {
                    Console.WriteLine($"{header}: {context.Request.Headers[header]}");
                }

                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    var json = await reader.ReadToEndAsync();
                    var data = JsonConvert.DeserializeObject<dynamic>(json);
                    Console.WriteLine("Received JSON:");
                    Console.WriteLine(data);
                }
            }
            else
            {
                Console.WriteLine("note post");
            }
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            context.Response.ContentType = "application/json";
            var responseJson = JsonConvert.SerializeObject(new { Status = "Success" });
            var buffer = Encoding.UTF8.GetBytes(responseJson);
            context.Response.ContentLength64 = buffer.Length;
            await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            context.Response.Close();
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            var stream = client.GetStream();
            var buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received callback: {request}");

            // 回复客户端
            string response = "OK";
            byte[] responseBytes = Encoding.UTF8.GetBytes(response);
            await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
        }
    }

    public static void Run()
    {
        Task.Run(async () =>
        {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://0.0.0.0:5000/callback/");

            listener.Start();
            Console.WriteLine("Listening for callbacks on http://localhost:5000/callback/");

            while (true)
            {
                // 等待客户端请求
                HttpListenerContext context = await listener.GetContextAsync();
                HttpListenerRequest request = context.Request;

                // 处理请求
                if (request.HttpMethod == "POST")
                {
                    using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string json = await reader.ReadToEndAsync();

                        Console.WriteLine("From Server");
                        Console.WriteLine($"{json}");
                    }

                    // 设置响应
                    context.Response.StatusCode = (int)HttpStatusCode.OK;
                    using (var writer = new StreamWriter(context.Response.OutputStream))
                    {
                        await writer.WriteAsync("Callback received successfully.");
                    }
                }
                else
                {
                    // 如果不是POST请求，返回错误
                    context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                    using (var writer = new StreamWriter(context.Response.OutputStream))
                    {
                        await writer.WriteAsync("Only POST method is allowed.");
                    }
                }

                context.Response.Close();
            }

        });
    }
}