using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Api;
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
            _searcher = new LsSearcher(int.MaxValue, 50);
        }

        [ResponseCache(Duration = 24 * 3600)]
        [HttpPost("search")]
        public async Task<CorpusSearchResponse> Search([FromBody] CorpusSearchRequest request)
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
                var corpus = _lsiProvider.GetCorpus(request.CorpusVersion);

                var corpusItem = corpus.Items.First(ci => ci.Name == request.CorpusName);

                var dictionary = _lsiProvider.GetDictionary(request.CorpusVersion);

                var searchQuery = SearchQuery.Construct(request.Query, dictionary.Words);

                var tasks = corpusItem.Files.Select(cfi => Task.Run(() =>
                {
                    var lsi = _lsiProvider.GetLsiData(request.CorpusVersion, cfi.Path);

                    var searchResults = searchQuery.QueryType == SearchQueryType.Heading
                        ? _searcher.SearchHeadings(lsi, searchQuery)
                        : _searcher.SearchParagraphs(lsi, searchQuery);

                    foreach (var searchResult in searchResults)
                    {
                        searchResult.File = cfi.Path;
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
    }
}
