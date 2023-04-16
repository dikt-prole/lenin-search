using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LenLib.Standard.Core.Corpus;
using Newtonsoft.Json;

namespace LenLib.Script.Scripts
{
    public class CorpusDownloadTestScript : IScript
    {
        public string Id => "corpus-download";
        public string Arguments => "none";

        public void Execute(params string[] input)
        {
            var tasks = Enumerable.Range(0, 5).Select(i => Task.Run(Download));

            var result = Task.WhenAll(tasks).Result;

            foreach (var elapseMs in result)
            {
                Console.WriteLine($"Max elapsed: {elapseMs}ms");
            }
        }

        private int Download()
        {
            var httpClient = new HttpClient();
            var summaryJson = httpClient.GetStringAsync("http://151.248.121.154:5000/corpus/summary").Result;
            var corpusItems = JsonConvert.DeserializeObject<List<CorpusItem>>(summaryJson);
            var tempFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N").Substring(0, 8));
            Directory.CreateDirectory(tempFolder);
            var maxElapsedMs = 0;

            foreach (var corpusItem in corpusItems)
            {
                var corpusFolder = Path.Combine(tempFolder, corpusItem.Id);
                Directory.CreateDirectory(corpusFolder);
                foreach (var cfi in corpusItem.Files)
                {
                    var link = $"http://151.248.121.154:5000/corpus/file?corpusId={corpusItem.Id}&file={cfi.Path}";
                    var filePath = Path.Combine(corpusFolder, cfi.Path);
                    var sw = new Stopwatch();
                    sw.Start();
                    var response = httpClient.GetAsync(link).Result;
                    sw.Stop();
                    if (sw.ElapsedMilliseconds > maxElapsedMs)
                    {
                        maxElapsedMs = (int) sw.ElapsedMilliseconds;
                    }
                }
            }
            return maxElapsedMs;
        }
    }
}