using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Api;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Search;
using LeninSearch.Standard.Core.Search.CorpusSearching;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace LeninSearch.Api.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
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

            var searchResult = await _corpusSearch.SearchAsync(request.CorpusId, request.Query, request.Mode);

            var searchResponse = SearchResponse.FromSearchResult(searchResult);

            _memoryCache.Set(cacheKey, searchResponse, TimeSpan.FromHours(1));

            return searchResponse;
        }

        [HttpGet("search-compressed")]
        [EnableCompression]
        public Task<SearchResponse> SearchCompressed([FromQuery] SearchRequest request)
        {
            return Search(request);
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
    }
}
