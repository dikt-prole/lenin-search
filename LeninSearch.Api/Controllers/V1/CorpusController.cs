using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Api;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Standard.Core.Search.CorpusSearching;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace LeninSearch.Api.Controllers.V1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CorpusController : ControllerBase
    {
        private readonly ILogger<CorpusController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ICorpusSearch _corpusSearch;
        private readonly SearchQueryCleaner _cleaner;

        public CorpusController(ILogger<CorpusController> logger, IMemoryCache memoryCache, ICorpusSearch corpusSearch)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _corpusSearch = corpusSearch;
            _cleaner = new SearchQueryCleaner();
        }

        [HttpGet("search")]
        public async Task<SearchResponse> Search([FromQuery] SearchRequest request)
        {
            request.Query = _cleaner.Clean(request.Query);

            var cacheKey = JsonConvert.SerializeObject(request, Formatting.None);

            _logger.LogInformation($"received search request: {cacheKey}");

            if (_memoryCache.TryGetValue(cacheKey, out var responseCache))
            {
                _logger.LogInformation($"using cached response for request: {JsonConvert.SerializeObject(request, Formatting.Indented)}");
                return responseCache as SearchResponse; ;
            }

            var validator = new SearchRequestValidator();

            var validationMessage = validator.Validate(request);
            if (validationMessage != null)
            {
                _logger.LogError($"validation error for request '{JsonConvert.SerializeObject(request, Formatting.Indented)}': {validationMessage}");
                throw new ValidationException(validationMessage);
            }

            var sw = new Stopwatch();
            sw.Start();

            var searchResult = await _corpusSearch.SearchAsync(request.CorpusId, request.Query, request.Mode);

            sw.Stop();
            _logger.LogInformation($"search elapsed: {sw.Elapsed}");

            var searchResponse = SearchResponse.FromSearchResult(searchResult);

            _memoryCache.Set(cacheKey, searchResponse, TimeSpan.FromHours(1));

            return searchResponse;
        }

        [HttpGet("search-compressed")]
        [EnableCompression]
        public async Task<IActionResult> SearchCompressed([FromQuery] SearchRequest request)
        {
            var searchResponse = await Search(request);
            var searchResponseJson = JsonConvert.SerializeObject(searchResponse);
            var searchResponseBytes = Encoding.UTF8.GetBytes(searchResponseJson);

            await using var brotliInputStream = new MemoryStream(searchResponseBytes);
            await using var brotliOutputStream = new MemoryStream();
            await using var brotli = new BrotliStream(brotliOutputStream, CompressionLevel.Fastest);
            await brotliInputStream.CopyToAsync(brotli);
            await brotli.FlushAsync();
            var brotliBytes = brotliOutputStream.ToArray();

            return File(brotliBytes, "text/json");
        }

        [HttpGet("summary")]
        public List<CorpusItem> GetCorpusSummary()
        {
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var folder = Path.Combine(executingDirectory, "corpus");
            var jsonFiles = Directory.GetFiles(folder, "corpus.json", SearchOption.AllDirectories);
            var summary = jsonFiles.Select(f => JsonConvert.DeserializeObject<CorpusItem>(System.IO.File.ReadAllText(f))).ToList();
            return summary;
        }

        [HttpGet("file")]
        public async Task<IActionResult> GetCorpusFile(string corpusId, string file)
        {
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(executingDirectory, "corpus", corpusId, file);
            var content = await System.IO.File.ReadAllBytesAsync(path);
            var contentType = "APPLICATION/octet-stream";
            return File(content, contentType, file);
        }

        [HttpGet("file-compressed")]
        public async Task<IActionResult> GetCorpusFileCompressed(string corpusId, string file)
        {
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(executingDirectory, "corpus", corpusId, file);
            var content = await System.IO.File.ReadAllBytesAsync(path);

            await using var brotliInputStream = new MemoryStream(content);
            await using var brotliOutputStream = new MemoryStream();
            await using var brotli = new BrotliStream(brotliOutputStream, CompressionLevel.Fastest);
            await brotliInputStream.CopyToAsync(brotli);
            await brotli.FlushAsync();
            var brotliBytes = brotliOutputStream.ToArray();

            return File(brotliBytes, "APPLICATION/octet-stream", file);
        }
    }
}
