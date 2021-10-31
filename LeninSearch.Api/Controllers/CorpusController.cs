using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using LeninSearch.Standard.Core.Api;
using LeninSearch.Standard.Core.Search;
using Newtonsoft.Json;

namespace LeninSearch.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CorpusController : ControllerBase
    {
        private readonly ILogger<CorpusController> _logger;
        private readonly ILsiProvider _lsiProvider;
        private readonly LsSearcher _searcher;

        public CorpusController(ILogger<CorpusController> logger, ILsiProvider lsiProvider)
        {
            _logger = logger;
            _lsiProvider = lsiProvider;
            _searcher = new LsSearcher();
        }

        [HttpPost("search")]
        public async  Task<CorpusSearchResponse> Search([FromBody] CorpusSearchRequest request)
        {
            _logger.LogDebug($"Received request: '{JsonConvert.SerializeObject(request)}'");

            var corpus = _lsiProvider.GetCorpus(request.CorpusVersion);

            var corpusItem = corpus.Items.First(ci => ci.Name == request.CorpusName);

            var dictionary = _lsiProvider.GetDictionary(request.CorpusVersion);

            var searchQuery = SearchQuery.Construct(request.Query, dictionary);

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
                Results = completeSearchResults.Select(ParagraphSearchResponse.From).ToList()
            };

            return response;
        }
    }
}
