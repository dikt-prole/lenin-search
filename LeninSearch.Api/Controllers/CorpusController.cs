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
using LeninSearch.Standard.Core.Corpus.Json;
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
        private readonly LsSearcher _searcher;

        public CorpusController(ILogger<CorpusController> logger, ILsiProvider lsiProvider, IMemoryCache memoryCache)
        {
            _logger = logger;
            _lsiProvider = lsiProvider;
            _memoryCache = memoryCache;
            _searcher = new LsSearcher(int.MaxValue, 100);
        }

        [HttpPost("lssearch")]
        public async Task<CorpusSearchResponse> Search([FromBody] CorpusSearchRequestNew request)
        {
            var requestJson = JsonConvert.SerializeObject(request);

            _logger.LogInformation($"received search request: {requestJson}");

            if (_memoryCache.TryGetValue(requestJson, out var responseCache))
            {
                _logger.LogInformation($"using cached response for request: {requestJson}");

                return responseCache as CorpusSearchResponse;;
            }

            var validator = new CorpusSearchRequestValidator();

            var validationMessage = validator.Validate(request);

            if (validationMessage != null)
            {
                _logger.LogError($"validation error for request '{requestJson}': {validationMessage}");
                throw new ValidationException(validationMessage);
            }

            try
            {
                var corpusItem = _lsiProvider.GetCorpusItem(request.CorpusId);

                var dictionary = _lsiProvider.GetDictionary(request.CorpusId);

                var searchQuery = SearchQuery.Construct(request.Query, dictionary.Words);

                var lsiFiles = corpusItem.Files.Where(f => f.Path.EndsWith(".lsi")).ToList();

                var tasks = lsiFiles.Select(cfi => Task.Run(() =>
                {
                    var lsi = _lsiProvider.GetLsiData(request.CorpusId, cfi.Path);

                    var searchResults = searchQuery.QueryType == SearchQueryType.Heading
                        ? _searcher.SearchHeadings(lsi, searchQuery)
                        : _searcher.SearchParagraphs(lsi, searchQuery);

                    foreach (var searchResult in searchResults)
                    {
                        searchResult.File = cfi.Path;
                        if (searchQuery.QueryType == SearchQueryType.Heading)
                        {
                            searchResult.Text = lsi.Paragraphs[searchResult.ParagraphIndex].GetText(dictionary.Words);
                        }
                    }

                    return searchResults;
                }));

                var results = await Task.WhenAll(tasks);

                var completeSearchResults = results.SelectMany(r => r).ToList();

                var response = new CorpusSearchResponse
                {
                    Results = completeSearchResults.Select(r => ParagraphSearchResponse.From(r, corpusItem)).ToList()
                };

                _memoryCache.Set(requestJson, response, TimeSpan.FromHours(1));

                return response;
            }
            catch (Exception exc)
            {
                _logger.LogError($"search exception: '{exc}'");
                throw;
            }
        }

        [HttpPost("search")]
        public async Task<CorpusSearchResponse> Search([FromBody] CorpusSearchRequest request)
        {
            string corpusId = null;
            switch (request.CorpusName)
            {
                case "Ленин ПСС":
                    corpusId = "lenin-v1";
                    break;
                case "Сталин ПСС":
                    corpusId = "stalin-v1";
                    break;
                case "Маркс-Энгельс ПСС":
                    corpusId = "marx-engels-v1";
                    break;
                case "Гегель Наука Логики":
                    corpusId = "hegel-v1";
                    break;
            }

            var lsRequest = new CorpusSearchRequestNew
            {
                CorpusId = corpusId,
                Query = request.Query
            };

            return await Search(lsRequest);
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
