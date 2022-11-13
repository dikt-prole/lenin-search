using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using LeninSearch.Standard.Core;
using LeninSearch.Standard.Core.Corpus;
using LeninSearch.Standard.Core.Corpus.Lsi;
using LeninSearch.Standard.Core.LsiUtil;
using LeninSearch.Standard.Core.Search;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace LeninSearch.Api.Services
{
    public class CachedLsiProvider : ILsiProvider
    {
        private readonly IMemoryCache _cache;

        private static readonly TimeSpan CachePeriod = TimeSpan.FromHours(1);

        private string LsiKey(string corpusId, string file) => $"lsi-{corpusId}-{file}";

        private string LsDictionaryKey(string corpusId) => $"ls-dictionary-{corpusId}";

        private string CorpusItemKey(string corpusId) => $"corpus-item-{corpusId}";

        private readonly ConcurrentDictionary<string, object> _locks = new ConcurrentDictionary<string, object>();

        public CachedLsiProvider(IMemoryCache cache)
        {
            _cache = cache;
        }

        public LsiData GetLsiData(string corpusId, string file)
        {
            var cacheKey = LsiKey(corpusId, file);
            var @lock = _locks.GetOrAdd(cacheKey, k => new object());
            LsiData lsiData = null;

            lock (@lock)
            {
                if (!_cache.TryGetValue(cacheKey, out lsiData))
                {
                    var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var lsiPath = Path.Combine(executingDirectory, "corpus", corpusId, file);
                    var lsiBytes = File.ReadAllBytes(lsiPath);
                    var lsiUtil = LsiUtilLocator.GetLsiUtil(lsiBytes[0]);
                    lsiData = lsiUtil.FromLsIndexBytes(lsiBytes);
                    _cache.Set(cacheKey, lsiData, CachePeriod);
                }
            }

            return lsiData;
        }

        public LsDictionary GetDictionary(string corpusId)
        {
            var cacheKey = LsDictionaryKey(corpusId);
            var @lock = _locks.GetOrAdd(cacheKey, k => new object());
            LsDictionary lsDictionary = null;

            lock (@lock)
            {
                if (!_cache.TryGetValue(cacheKey, out lsDictionary))
                {
                    var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var dictionaryPath = Path.Combine(executingDirectory, "corpus", corpusId, "corpus.dic");
                    var words = File.ReadAllLines(dictionaryPath).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                    lsDictionary = new LsDictionary(words);
                    _cache.Set(cacheKey, lsDictionary, CachePeriod);
                }
            }

            return lsDictionary;
        }

        public CorpusItem GetCorpusItem(string corpusId)
        {
            var cacheKey = CorpusItemKey(corpusId);
            var @lock = _locks.GetOrAdd(cacheKey, k => new object());
            CorpusItem corpusItem = null;

            lock (@lock)
            {
                if (!_cache.TryGetValue(cacheKey, out corpusItem))
                {
                    var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    var corpusItemPath = Path.Combine(executingDirectory, "corpus", corpusId, "corpus.json");
                    var corpusItemJson = File.ReadAllText(corpusItemPath);
                    corpusItem = JsonConvert.DeserializeObject<CorpusItem>(corpusItemJson);
                    _cache.Set(cacheKey, corpusItem, CachePeriod);
                }
            }

            return corpusItem;
        }
    }
}