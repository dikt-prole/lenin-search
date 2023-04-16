using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LenLib.Standard.Core;
using LenLib.Standard.Core.Corpus;
using LenLib.Standard.Core.Corpus.Lsi;
using LenLib.Standard.Core.LsiUtil;
using LenLib.Standard.Core.Search;
using Newtonsoft.Json;

namespace LenLib.Api.Services
{
    public class PrecachedLsiProvider : ILsiProvider
    {
        private readonly Dictionary<string, LsiData> _lsIndexData = new Dictionary<string, LsiData>();

        private readonly Dictionary<string, LsDictionary> _dictionaries = new Dictionary<string, LsDictionary>();

        private readonly Dictionary<string, CorpusItem> _corpusItems = new Dictionary<string, CorpusItem>();

        private string Key(string corpusId, string file) => $"{corpusId}-{file}";

        public PrecachedLsiProvider Load(int cachedPercentage)
        {
            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var folder = Path.Combine(executingDirectory, "corpus");
            var jsonFiles = Directory.GetFiles(folder, "corpus.json", SearchOption.AllDirectories);
            var corpusItems = jsonFiles.Select(f => JsonConvert.DeserializeObject<CorpusItem>(File.ReadAllText(f))).ToList();
            foreach (var corpusItem in corpusItems)
            {
                _corpusItems.Add(corpusItem.Id, corpusItem);

                var corpusFolder = Path.Combine(folder, corpusItem.Id);
                var lsiFiles = Directory.GetFiles(corpusFolder, "*.lsi").OrderBy(f => Guid.NewGuid()).ToList();
                var cached = lsiFiles.Take(lsiFiles.Count * cachedPercentage / 100).ToList();
                foreach (var lsiFile in cached)
                {
                    var lsiBytes = File.ReadAllBytes(lsiFile);
                    var key = Key(corpusItem.Id, Path.GetFileName(lsiFile));

                    var lsiUtil = LsiUtilLocator.GetLsiUtil(lsiBytes[0]);

                    var lsiData = lsiUtil.FromLsIndexBytes(lsiBytes);
                    _lsIndexData.Add(key, lsiData);
                }

                var words = File.ReadAllLines(Path.Combine(folder, corpusItem.Id, "corpus.dic")).Where(s => !string.IsNullOrEmpty(s)).ToArray();
                var dictionary = new LsDictionary(words);
                _dictionaries.Add(corpusItem.Id, dictionary);
            }

            return this;
        }

        public LsiData GetLsiData(string corpusId, string file)
        {
            var key = Key(corpusId, file);
            if (_lsIndexData.ContainsKey(key))
            {
                return _lsIndexData[key];
            }

            var executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var lsiFile = Path.Combine(executingDirectory, "corpus", corpusId, file);
            var lsiBytes = File.ReadAllBytes(lsiFile);

            var lsiUtil = LsiUtilLocator.GetLsiUtil(lsiBytes[0]);

            return lsiUtil.FromLsIndexBytes(lsiBytes);
        }

        public LsDictionary GetDictionary(string corpusId)
        {
            return _dictionaries[corpusId];
        }

        public CorpusItem GetCorpusItem(string corpusId)
        {
            return _corpusItems[corpusId];
        }
    }
}