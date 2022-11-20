using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IO;

namespace LeninSearch.Api
{
    /// <summary>
    /// https://elanderson.net/2019/12/log-requests-and-responses-in-asp-net-core-3/
    /// </summary>
    public class SimpleLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

        public SimpleLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
        }

        public async Task Invoke(HttpContext context)
        {
            await LogRequest(context);
            await LogResponse(context);
        }

        private async Task LogRequest(HttpContext context)
        {
            context.Request.EnableBuffering();
            await using var requestStream = _recyclableMemoryStreamManager.GetStream();
            await context.Request.Body.CopyToAsync(requestStream);

            Debug.WriteLine("Request log:");
            Debug.WriteLine(string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}={h.Value}")));

            context.Request.Body.Position = 0;
        }

        public async Task LogResponse(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;
            await using var responseBody = _recyclableMemoryStreamManager.GetStream();
            context.Response.Body = responseBody;

            await _next(context);

            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var bodyBytes = ReadFully(context.Response.Body);
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            Debug.WriteLine("Response log:");
            Debug.WriteLine($"body size: {bodyBytes.Length}");
            await responseBody.CopyToAsync(originalBodyStream);
        }

        public static byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}