using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using LeninSearch.Api.Dto.V1;
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
    public class TgController : ControllerBase
    {
        private readonly ILogger<TgController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly ICorpusSearch _corpusSearch;
        private readonly ILsiProvider _lsiProvider;
        private readonly SearchQueryCleaner _cleaner;

        public TgController(ILogger<TgController> logger, IMemoryCache memoryCache, ICorpusSearch corpusSearch, ILsiProvider lsiProvider)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _corpusSearch = corpusSearch;
            _lsiProvider = lsiProvider;
            _cleaner = new SearchQueryCleaner();
        }

        [HttpGet("search")]
        public async Task<TgSearchResponse> Search([FromQuery] TgSearchRequest request)
        {
            request.Query = _cleaner.Clean(request.Query);

            var cacheKey = $"{request.CorpusId}-{request.Query}-{request.Mode}";

            var searchResult = _memoryCache.TryGetValue(cacheKey, out var responseCache)
                ? responseCache as SearchResult
                : await _corpusSearch.SearchAsync(request.CorpusId, request.Query, request.Mode);

            var searchResponse = TgSearchResponse.FromSearchResult(searchResult, request.Page, request.PageSize);

            _memoryCache.Set(cacheKey, searchResult, TimeSpan.FromHours(1));

            return searchResponse;
        }

        [HttpGet("paragraph")]
        public TgParagraphResponse GetParagraph([FromQuery] TgParagraphRequest request)
        {
            var lsiData = _lsiProvider.GetLsiData(request.CorpusId, request.Path);
            var dictionary = _lsiProvider.GetDictionary(request.CorpusId);

            return new TgParagraphResponse
            {
                Text = lsiData.Paragraphs[request.ParagraphIndex].GetText(dictionary.Words),
                ParagraphIndex = request.ParagraphIndex
            };
        }

        [HttpGet("corpus-items")]
        public TgCorpusItemResponse[] GetCorpusItems()
        {
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var folder = Path.Combine(executingDirectory, "corpus");
            var jsonFiles = Directory.GetFiles(folder, "corpus.json", SearchOption.AllDirectories);
            var corpusItems = jsonFiles.Select(f => JsonConvert.DeserializeObject<CorpusItem>(System.IO.File.ReadAllText(f))).ToList();

            var tgCorpusItems = corpusItems.Where(ci => ci.LsiVersion >= 2)
                .Select(TgCorpusItemResponse.FromCorpusItem).ToArray();

            return tgCorpusItems;
        }

        [HttpGet("icon")]
        public async Task<IActionResult> GetIcon(string corpusId)
        {
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var iconFile = Path.Combine(executingDirectory, "corpus", corpusId, "icon.png");
            var iconBytes = await System.IO.File.ReadAllBytesAsync(iconFile);
            return File(iconBytes, "image/png");
        }
    }
}
