using System;
using System.IO;
using System.Linq;
using System.Reflection;
using LenLib.Standard.Core;
using LenLib.Standard.Core.Corpus;
using LenLib.Standard.Core.Corpus.Lsi;
using LenLib.Standard.Core.LsiUtil;
using LenLib.Standard.Core.Search;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LenLib.Api.Services
{
    public class CachedLsiProvider : ILsiProvider
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<CachedLsiProvider> _logger;

        private static readonly TimeSpan CachePeriod = TimeSpan.FromHours(1);

        private string LsiKey(string corpusId, string file) => $"lsi-{corpusId}-{file}";

        private string LsDictionaryKey(string corpusId) => $"ls-dictionary-{corpusId}";

        private string CorpusItemKey(string corpusId) => $"corpus-item-{corpusId}";

        public CachedLsiProvider(IMemoryCache cache, ILogger<CachedLsiProvider> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public LsiData GetLsiData(string corpusId, string file)
        {
            var cacheKey = LsiKey(corpusId, file);
            return _cache.GetOrCreate(cacheKey, cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = CachePeriod;
                _logger.LogInformation($"loading cache for: {cacheKey}");
                var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var lsiPath = Path.Combine(executingDirectory, "corpus", corpusId, file);
                var lsiBytes = File.ReadAllBytes(lsiPath);
                var lsiUtil = LsiUtilLocator.GetLsiUtil(lsiBytes[0]);
                return lsiUtil.FromLsIndexBytes(lsiBytes);
            });
        }

        public LsDictionary GetDictionary(string corpusId)
        {
            var cacheKey = LsDictionaryKey(corpusId);
            return _cache.GetOrCreate(cacheKey, cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = CachePeriod;
                var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var dictionaryPath = Path.Combine(executingDirectory, "corpus", corpusId, "corpus.dic");
                var words = File.ReadAllLines(dictionaryPath).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                return new LsDictionary(words);
            });
        }

        public CorpusItem GetCorpusItem(string corpusId)
        {
            var cacheKey = CorpusItemKey(corpusId);
            return _cache.GetOrCreate(cacheKey, cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = CachePeriod;
                var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var corpusItemPath = Path.Combine(executingDirectory, "corpus", corpusId, "corpus.json");
                var corpusItemJson = File.ReadAllText(corpusItemPath);
                return JsonConvert.DeserializeObject<CorpusItem>(corpusItemJson);
            });
        }
    }
}