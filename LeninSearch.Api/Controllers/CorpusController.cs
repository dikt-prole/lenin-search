using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Api;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Search;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace LeninSearch.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CorpusController : ControllerBase
    {
        private readonly ILogger<CorpusController> _logger;
        private readonly ILsiProvider _lsiProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly ISearchQueryFactory _searchQueryFactory;
        private readonly LsSearcher _searcher;
        private readonly SearchQueryCleaner _cleaner;

        public CorpusController(ILogger<CorpusController> logger, ILsiProvider lsiProvider, IMemoryCache memoryCache, ISearchQueryFactory searchQueryFactory)
        {
            _logger = logger;
            _lsiProvider = lsiProvider;
            _memoryCache = memoryCache;
            _searchQueryFactory = searchQueryFactory;
            _searcher = new LsSearcher(int.MaxValue, 100);
            _cleaner = new SearchQueryCleaner();
        }

        [HttpPost("ls-search")]
        public async Task<CorpusSearchResponse> Search([FromBody] CorpusSearchRequest request)
        {
            request.Query = _cleaner.Clean(request.Query);

            var cacheKey = JsonConvert.SerializeObject(request, Formatting.None);

            _logger.LogInformation($"received search request: {cacheKey}");

            if (_memoryCache.TryGetValue(cacheKey, out var responseCache))
            {
                _logger.LogInformation($"using cached response for request: {JsonConvert.SerializeObject(request, Formatting.Indented)}");
                return responseCache as CorpusSearchResponse;;
            }

            var validator = new CorpusSearchRequestValidator();

            var validationMessage = validator.Validate(request);
            if (validationMessage != null)
            {
                _logger.LogError($"validation error for request '{JsonConvert.SerializeObject(request, Formatting.Indented)}': {validationMessage}");
                throw new ValidationException(validationMessage);
            }

            try
            {
                var corpusItem = _lsiProvider.GetCorpusItem(request.CorpusId);
                var dictionary = _lsiProvider.GetDictionary(request.CorpusId);

                var searchQueries = _searchQueryFactory.Construct(request.Query, dictionary.Words, request.Mode).ToList();

                var lsiFiles = corpusItem.Files.Where(f => f.Path.EndsWith(".lsi")).ToList();

                var tasks = lsiFiles.Select(cfi => Task.Run(() => InnerSearch(request.CorpusId, cfi.Path, searchQueries, dictionary)));

                var unitTuples = (await Task.WhenAll(tasks)).SelectMany(ut => ut);

                var response = new CorpusSearchResponse { Units = new Dictionary<string, Dictionary<string, List<SearchResultUnitResponse>>>() };

                foreach (var unitTuple in unitTuples)
                {
                    if (!response.Units.ContainsKey(unitTuple.File))
                    {
                        response.Units.Add(unitTuple.File, new Dictionary<string, List<SearchResultUnitResponse>>());
                    }

                    if (!response.Units[unitTuple.File].ContainsKey(unitTuple.Query.Text))
                    {
                        response.Units[unitTuple.File][unitTuple.Query.Text] = new List<SearchResultUnitResponse>();
                    }

                    response.Units[unitTuple.File][unitTuple.Query.Text].Add(SearchResultUnitResponse.FromSearchResultUnit(unitTuple.Unit));
                }

                _memoryCache.Set(cacheKey, response, TimeSpan.FromHours(1));

                return response;
            }
            catch (Exception exc)
            {
                _logger.LogError($"search exception: '{exc}'");
                throw;
            }
        }

        private List<(SearchUnit Unit, string File, SearchQuery Query)> InnerSearch(string corpusId, string file, List<SearchQuery> queries, LsDictionary dictionary)
        {
            var result = new List<(SearchUnit Unit, string File, SearchQuery Query)>();
            var lsiData = _lsiProvider.GetLsiData(corpusId, file);
            var corpusFileItem = _lsiProvider.GetCorpusItem(corpusId).GetFileByPath(file);
            var excludedParagraphs = new HashSet<ushort>();
            foreach (var searchQuery in queries)
            {
                var units = _searcher.Search(lsiData, searchQuery, excludedParagraphs);

                foreach (var unit in units)
                {
                    unit.SetPreviewAndTitle(lsiData, corpusFileItem, dictionary);
                }

                excludedParagraphs.UnionWith(units.Select(u => u.ParagraphIndex));
            }

            return result;
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
